using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence;
using MES.Contracts.Phase3;

namespace MES.Services.ProcessControl.Tests;

[Trait("Category", "UnitTest")]
[Trait("Phase", "3")]
public class ProcessParameterServiceTests
{
    private readonly Mock<ILogger<ProcessParameterService>> _mockLogger = new();

    private MesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MesDbContext(options);
    }

    [Fact]
    public async Task CreateParameterSetAsync_WithValidData_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ProcessParameterService(context, _mockLogger.Object);
        var request = new CreateProcessParameterSetRequest
        {
            ProcessCode = "WIRE_BOND",
            ProcessName = "Wire Bonding",
            ProductId = "PROD-001",
            ProductName = "Test Product",
            EquipmentType = "WB-100",
            Version = "1.0",
            Description = "Test parameter set",
            Items = new List<CreateParameterItemRequest>
            {
                new CreateParameterItemRequest
                {
                    ParameterCode = "TEMP",
                    ParameterName = "Temperature",
                    ParameterType = "Numeric",
                    Unit = "C",
                    StandardValue = 250m,
                    LowerLimit = 240m,
                    UpperLimit = 260m,
                    IsRequired = true
                }
            }
        };

        // Act
        var result = await service.CreateParameterSetAsync(request, "testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("WIRE_BOND", result.ProcessCode);
        Assert.Equal("Draft", result.Status);
        Assert.Equal(1, result.ItemCount);
        Assert.NotNull(result.ParameterSetId);
        Assert.StartsWith("PPS-", result.ParameterSetId);
    }

    [Fact]
    public async Task CreateParameterSetAsync_WithNoItems_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ProcessParameterService(context, _mockLogger.Object);
        var request = new CreateProcessParameterSetRequest
        {
            ProcessCode = "CURE",
            ProcessName = "Curing",
            Version = "1.0"
        };

        // Act
        var result = await service.CreateParameterSetAsync(request, "testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CURE", result.ProcessCode);
        Assert.Equal(0, result.ItemCount);
    }

    [Fact]
    public async Task QueryParameterSetsAsync_WithNoData_ReturnsEmptyResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ProcessParameterService(context, _mockLogger.Object);
        var query = new ProcessParameterQuery { PageIndex = 1, PageSize = 10 };

        // Act
        var result = await service.QueryParameterSetsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task QueryParameterSetsAsync_WithData_ReturnsPagedResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ProcessParameterService(context, _mockLogger.Object);

        await service.CreateParameterSetAsync(new CreateProcessParameterSetRequest
        {
            ProcessCode = "WIRE_BOND",
            ProcessName = "Wire Bonding",
            Version = "1.0"
        }, "testuser");

        var query = new ProcessParameterQuery { PageIndex = 1, PageSize = 10 };

        // Act
        var result = await service.QueryParameterSetsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task QueryParameterSetsAsync_FilterByProcessCode_ReturnsFilteredResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ProcessParameterService(context, _mockLogger.Object);

        await service.CreateParameterSetAsync(new CreateProcessParameterSetRequest
        {
            ProcessCode = "WIRE_BOND",
            ProcessName = "Wire Bonding",
            Version = "1.0"
        }, "testuser");

        await service.CreateParameterSetAsync(new CreateProcessParameterSetRequest
        {
            ProcessCode = "CURE",
            ProcessName = "Curing",
            Version = "1.0"
        }, "testuser");

        var query = new ProcessParameterQuery { PageIndex = 1, PageSize = 10, ProcessCode = "WIRE_BOND" };

        // Act
        var result = await service.QueryParameterSetsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("WIRE_BOND", result.Items[0].ProcessCode);
    }

    [Fact]
    public async Task GetParameterSetAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ProcessParameterService(context, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetParameterSetAsync("NONEXISTENT"));
    }

    [Fact]
    public async Task GetParameterSetAsync_WhenFound_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ProcessParameterService(context, _mockLogger.Object);

        var createResult = await service.CreateParameterSetAsync(new CreateProcessParameterSetRequest
        {
            ProcessCode = "WIRE_BOND",
            ProcessName = "Wire Bonding",
            Version = "1.0"
        }, "testuser");

        // Act
        var result = await service.GetParameterSetAsync(createResult.ParameterSetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createResult.ParameterSetId, result.ParameterSetId);
        Assert.Equal("WIRE_BOND", result.ProcessCode);
    }

    [Fact]
    public async Task ActivateParameterSetAsync_WithValidData_UpdatesStatus()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ProcessParameterService(context, _mockLogger.Object);

        var createResult = await service.CreateParameterSetAsync(new CreateProcessParameterSetRequest
        {
            ProcessCode = "WIRE_BOND",
            ProcessName = "Wire Bonding",
            Version = "1.0"
        }, "testuser");

        var activateRequest = new ActivateParameterSetRequest
        {
            EffectiveDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(6)
        };

        // Act
        var result = await service.ActivateParameterSetAsync(createResult.ParameterSetId, activateRequest, "testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Active", result.Status);
        Assert.NotNull(result.EffectiveDate);
    }

    [Fact]
    public async Task ActivateParameterSetAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ProcessParameterService(context, _mockLogger.Object);
        var activateRequest = new ActivateParameterSetRequest { EffectiveDate = DateTime.UtcNow };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.ActivateParameterSetAsync("NONEXISTENT", activateRequest, "testuser"));
    }

    [Fact]
    public async Task GetParameterItemsAsync_WithValidData_ReturnsItems()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ProcessParameterService(context, _mockLogger.Object);

        var createResult = await service.CreateParameterSetAsync(new CreateProcessParameterSetRequest
        {
            ProcessCode = "WIRE_BOND",
            ProcessName = "Wire Bonding",
            Version = "1.0",
            Items = new List<CreateParameterItemRequest>
            {
                new CreateParameterItemRequest
                {
                    ParameterCode = "TEMP",
                    ParameterName = "Temperature",
                    StandardValue = 250m,
                    SortOrder = 1
                },
                new CreateParameterItemRequest
                {
                    ParameterCode = "TIME",
                    ParameterName = "Time",
                    StandardValue = 60m,
                    SortOrder = 2
                }
            }
        }, "testuser");

        // Act
        var items = await service.GetParameterItemsAsync(createResult.ParameterSetId);

        // Assert
        Assert.NotNull(items);
        Assert.Equal(2, items.Count);
        Assert.Equal("TEMP", items[0].ParameterCode);
    }
}
