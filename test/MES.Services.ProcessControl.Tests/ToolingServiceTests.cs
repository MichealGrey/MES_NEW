using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence;
using MES.Contracts.Phase3;

namespace MES.Services.ProcessControl.Tests;

[Trait("Category", "UnitTest")]
[Trait("Phase", "3")]
public class ToolingServiceTests
{
    private readonly Mock<ILogger<ToolingService>> _mockLogger = new();

    private MesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MesDbContext(options);
    }

    [Fact]
    public async Task CreateToolingAsync_WithValidData_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ToolingService(context, _mockLogger.Object);
        var request = new CreateToolingRequest
        {
            ToolingCode = "TOOL-001",
            ToolingName = "Wire Bond Wedge",
            ToolingType = "Wedge",
            Specification = "15mm",
            Manufacturer = "Kulicke & Soffa",
            Model = "KSW-1234",
            ExpectedLifespan = 1000,
            LifespanUnit = "Hours",
            Location = "Line-A",
            AssociatedProcess = "WIRE_BOND"
        };

        // Act
        var result = await service.CreateToolingAsync(request, "testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TOOL-001", result.ToolingCode);
        Assert.Equal("Available", result.Status);
        Assert.Equal(0, result.CurrentUsage);
        Assert.NotNull(result.ToolingId);
        Assert.StartsWith("TOOL-", result.ToolingId);
    }

    [Fact]
    public async Task CreateToolingAsync_WithoutOptionalFields_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ToolingService(context, _mockLogger.Object);
        var request = new CreateToolingRequest
        {
            ToolingCode = "TOOL-002",
            ToolingName = "Basic Tool",
            ToolingType = "General"
        };

        // Act
        var result = await service.CreateToolingAsync(request, "testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TOOL-002", result.ToolingCode);
        Assert.Equal("Available", result.Status);
    }

    [Fact]
    public async Task GetToolingAsync_WhenFound_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ToolingService(context, _mockLogger.Object);

        var createResult = await service.CreateToolingAsync(new CreateToolingRequest
        {
            ToolingCode = "TOOL-TEST",
            ToolingName = "Test Tool",
            ToolingType = "Test"
        }, "testuser");

        // Act
        var result = await service.GetToolingAsync(createResult.ToolingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createResult.ToolingId, result.ToolingId);
        Assert.Equal("TOOL-TEST", result.ToolingCode);
    }

    [Fact]
    public async Task GetToolingAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ToolingService(context, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetToolingAsync("NONEXISTENT"));
    }

    [Fact]
    public async Task QueryToolingsAsync_ReturnsPagedResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ToolingService(context, _mockLogger.Object);

        await service.CreateToolingAsync(new CreateToolingRequest
        {
            ToolingCode = "TOOL-001",
            ToolingName = "Tool A",
            ToolingType = "Wedge"
        }, "testuser");

        await service.CreateToolingAsync(new CreateToolingRequest
        {
            ToolingCode = "TOOL-002",
            ToolingName = "Tool B",
            ToolingType = "Ball"
        }, "testuser");

        var query = new ToolingQuery { PageIndex = 1, PageSize = 10 };

        // Act
        var result = await service.QueryToolingsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task LogUsageAsync_UpdatesCurrentUsage()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ToolingService(context, _mockLogger.Object);

        var tooling = await service.CreateToolingAsync(new CreateToolingRequest
        {
            ToolingCode = "TOOL-USAGE",
            ToolingName = "Usage Test Tool",
            ToolingType = "Wedge",
            ExpectedLifespan = 1000,
            LifespanUnit = "Hours"
        }, "testuser");

        var usageRequest = new ToolingUsageLogRequest
        {
            ToolingId = tooling.ToolingId,
            LotId = "LOT-001",
            StepCode = "WIRE_BOND",
            StepSeq = 1,
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = DateTime.UtcNow,
            UsageDuration = 60,
            UsageDurationUnit = "Minutes"
        };

        // Act
        var result = await service.LogUsageAsync(usageRequest, "operator1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tooling.ToolingId, result.ToolingId);

        // Verify usage was updated
        var updatedTooling = await service.GetToolingAsync(tooling.ToolingId);
        Assert.True(updatedTooling.CurrentUsage > 0);
    }

    [Fact]
    public async Task LogUsageAsync_WhenUsageExceedsThreshold_UpdatesStatus()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ToolingService(context, _mockLogger.Object);

        var tooling = await service.CreateToolingAsync(new CreateToolingRequest
        {
            ToolingCode = "TOOL-LIMIT",
            ToolingName = "Limit Test Tool",
            ToolingType = "Wedge",
            ExpectedLifespan = 100,
            LifespanUnit = "Hours"
        }, "testuser");

        // Log multiple usages to exceed threshold
        for (int i = 0; i < 2; i++)
        {
            var usageRequest = new ToolingUsageLogRequest
            {
                ToolingId = tooling.ToolingId,
                LotId = $"LOT-{i:000}",
                StepCode = "WIRE_BOND",
                StepSeq = 1,
                StartTime = DateTime.UtcNow.AddHours(-10),
                EndTime = DateTime.UtcNow,
                UsageDuration = 60,
                UsageDurationUnit = "Minutes"
            };
            await service.LogUsageAsync(usageRequest, "operator1");
        }

        // Act
        var updatedTooling = await service.GetToolingAsync(tooling.ToolingId);

        // Assert
        Assert.True(updatedTooling.CurrentUsage >= 100);
        Assert.Equal("Expired", updatedTooling.Status);
    }

    [Fact]
    public async Task LogUsageAsync_WhenToolingNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new ToolingService(context, _mockLogger.Object);
        var usageRequest = new ToolingUsageLogRequest
        {
            ToolingId = "NONEXISTENT",
            LotId = "LOT-001",
            StepCode = "WIRE_BOND",
            StepSeq = 1,
            StartTime = DateTime.UtcNow,
            UsageDuration = 60
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.LogUsageAsync(usageRequest, "operator1"));
    }
}
