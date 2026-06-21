using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence;
using MES.Contracts.Phase3;

namespace MES.Services.ProcessControl.Tests;

[Trait("Category", "UnitTest")]
[Trait("Phase", "3")]
public class BinServiceTests
{
    private readonly Mock<ILogger<BinService>> _mockLogger = new();

    private MesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MesDbContext(options);
    }

    [Fact]
    public async Task CreateBinDefinitionAsync_WithValidData_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BinService(context, _mockLogger.Object);
        var request = new CreateBinDefinitionRequest
        {
            BinCode = "BIN-GOOD-01",
            BinName = "Good Bin",
            BinCategory = "Good",
            BinNo = 1,
            Description = "Standard good bin",
            Color = "#00FF00",
            IsDefault = false,
            IsActive = true,
            ProcessCode = "FT",
            TestType = "FT",
            SortOrder = 1
        };

        // Act
        var result = await service.CreateBinDefinitionAsync(request, "testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("BIN-GOOD-01", result.BinCode);
        Assert.Equal("Good Bin", result.BinName);
        Assert.Equal("Good", result.BinCategory);
        Assert.True(result.IsActive);
        Assert.NotNull(result.BinId);
        Assert.StartsWith("BIN-", result.BinId);
    }

    [Fact]
    public async Task CreateBinDefinitionAsync_WithFailCategory_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BinService(context, _mockLogger.Object);
        var request = new CreateBinDefinitionRequest
        {
            BinCode = "BIN-FAIL-01",
            BinName = "Fail Bin",
            BinCategory = "Fail",
            BinNo = 255,
            Color = "#FF0000",
            IsDefault = false,
            IsActive = true,
            ProcessCode = "CP",
            TestType = "CP",
            SortOrder = 255
        };

        // Act
        var result = await service.CreateBinDefinitionAsync(request, "testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Fail", result.BinCategory);
        Assert.Equal(255, result.BinNo);
    }

    [Fact]
    public async Task GetBinDefinitionAsync_WhenFound_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BinService(context, _mockLogger.Object);

        var createResult = await service.CreateBinDefinitionAsync(new CreateBinDefinitionRequest
        {
            BinCode = "BIN-TEST-01",
            BinName = "Test Bin",
            BinCategory = "Good",
            BinNo = 1,
            IsActive = true
        }, "testuser");

        // Act
        var result = await service.GetBinDefinitionAsync(createResult.BinId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createResult.BinId, result.BinId);
        Assert.Equal("BIN-TEST-01", result.BinCode);
    }

    [Fact]
    public async Task GetBinDefinitionAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BinService(context, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetBinDefinitionAsync("NONEXISTENT"));
    }

    [Fact]
    public async Task QueryBinDefinitionsAsync_ReturnsPagedResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BinService(context, _mockLogger.Object);

        await service.CreateBinDefinitionAsync(new CreateBinDefinitionRequest
        {
            BinCode = "BIN-01",
            BinName = "Good Bin 1",
            BinCategory = "Good",
            BinNo = 1,
            ProcessCode = "FT",
            TestType = "FT",
            IsActive = true
        }, "testuser");

        await service.CreateBinDefinitionAsync(new CreateBinDefinitionRequest
        {
            BinCode = "BIN-02",
            BinName = "Good Bin 2",
            BinCategory = "Good",
            BinNo = 2,
            ProcessCode = "CP",
            TestType = "CP",
            IsActive = true
        }, "testuser");

        var query = new BinQuery { PageIndex = 1, PageSize = 10 };

        // Act
        var result = await service.QueryBinDefinitionsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task QueryBinDefinitionsAsync_WithNoData_ReturnsEmptyResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BinService(context, _mockLogger.Object);
        var query = new BinQuery { PageIndex = 1, PageSize = 10 };

        // Act
        var result = await service.QueryBinDefinitionsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task QueryBinDefinitionsAsync_FilterByBinCategory_ReturnsFilteredResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BinService(context, _mockLogger.Object);

        await service.CreateBinDefinitionAsync(new CreateBinDefinitionRequest
        {
            BinCode = "BIN-GOOD",
            BinName = "Good Bin",
            BinCategory = "Good",
            BinNo = 1,
            IsActive = true
        }, "testuser");

        await service.CreateBinDefinitionAsync(new CreateBinDefinitionRequest
        {
            BinCode = "BIN-FAIL",
            BinName = "Fail Bin",
            BinCategory = "Fail",
            BinNo = 255,
            IsActive = true
        }, "testuser");

        var query = new BinQuery { PageIndex = 1, PageSize = 10, BinCategory = "Good" };

        // Act
        var result = await service.QueryBinDefinitionsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Good", result.Items[0].BinCategory);
    }

    [Fact]
    public async Task GetBinSummaryAsync_WithValidLotId_ReturnsStatistics()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new BinService(context, _mockLogger.Object);

        var query = new BinSummaryQuery { LotId = "LOT-001" };

        // Act
        var result = await service.GetBinSummaryAsync(query);

        // Assert
        Assert.NotNull(result);
        // Empty data returns empty list (no exception thrown)
        Assert.IsType<List<BinStatisticsResponse>>(result);
    }
}
