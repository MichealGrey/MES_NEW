using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics.Tests;

[Trait("Category", "UnitTest")]
[Trait("Phase", "5")]
public class YieldServiceTests
{
    private readonly Mock<ILogger<YieldService>> _mockLogger = new();

    private MesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MesDbContext(options);
    }

    [Fact]
    public async Task RecordYieldAsync_WithValidData_SavesYield()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new YieldService(context, _mockLogger.Object);

        // Act
        var result = await service.RecordYieldAsync(
            lotId: "LOT-001",
            stepCode: "WIRE_BOND",
            inputQty: 100,
            outputQty: 95,
            statDate: DateOnly.FromDateTime(DateTime.UtcNow));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("LOT-001", result.LotId);
        Assert.Equal("WIRE_BOND", result.StepCode);
        Assert.Equal(100, result.InputQty);
        Assert.Equal(95, result.OutputQty);
        Assert.Equal(95.0m, result.YieldRate);
        Assert.Equal(5, result.ScrapQty);
        Assert.NotNull(result.StatId);
        Assert.StartsWith("YIELD-", result.StatId);
    }

    [Fact]
    public async Task GetProcessYieldAsync_WithValidLotId_ReturnsYieldData()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new YieldService(context, _mockLogger.Object);

        await service.RecordYieldAsync("LOT-001", "WIRE_BOND", 100, 95, DateOnly.FromDateTime(DateTime.UtcNow));
        await service.RecordYieldAsync("LOT-001", "CURE", 95, 90, DateOnly.FromDateTime(DateTime.UtcNow));

        var query = new YieldQuery { PageIndex = 1, PageSize = 10, LotId = "LOT-001" };

        // Act
        var result = await service.GetProcessYieldAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetProcessYieldAsync_WithNoData_ReturnsEmptyResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new YieldService(context, _mockLogger.Object);
        var query = new YieldQuery { PageIndex = 1, PageSize = 10, LotId = "LOT-EMPTY" };

        // Act
        var result = await service.GetProcessYieldAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetParetoAsync_ReturnsDefectPareto()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new YieldService(context, _mockLogger.Object);

        await service.RecordYieldAsync("LOT-001", "WIRE_BOND", 100, 95, DateOnly.FromDateTime(DateTime.UtcNow), "{\"defect\":\"scratches\"}");
        await service.RecordYieldAsync("LOT-002", "CURE", 80, 75, DateOnly.FromDateTime(DateTime.UtcNow), "{\"defect\":\"cracks\"}");

        // Act
        var result = await service.GetParetoAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.True(result.Items.Count > 0);
    }

    [Fact]
    public async Task GetCumulativeYieldAsync_WithValidLotId_ReturnsCumulativeYield()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new YieldService(context, _mockLogger.Object);

        await service.RecordYieldAsync("LOT-001", "STEP1", 100, 95, DateOnly.FromDateTime(DateTime.UtcNow));
        await service.RecordYieldAsync("LOT-001", "STEP2", 95, 90, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        // Act
        var result = await service.GetCumulativeYieldAsync("LOT-001");

        // Assert
        Assert.True(result > 0);
        // Should be approximately 95 * 90 / 100 = 85.5%
        Assert.True(result > 80 && result <= 100);
    }

    [Fact]
    public async Task GetCumulativeYieldAsync_WithNoData_ReturnsZero()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new YieldService(context, _mockLogger.Object);

        // Act
        var result = await service.GetCumulativeYieldAsync("LOT-NONEXISTENT");

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetYieldTrendAsync_WithValidLotId_ReturnsTrend()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new YieldService(context, _mockLogger.Object);

        await service.RecordYieldAsync("LOT-TREND", "STEP1", 100, 95, DateOnly.FromDateTime(DateTime.UtcNow));
        await service.RecordYieldAsync("LOT-TREND", "STEP2", 95, 90, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        // Act
        var result = await service.GetYieldTrendAsync("LOT-TREND");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("LOT-TREND", result.LotId);
        Assert.Equal(2, result.Points.Count);
    }

    [Fact]
    public async Task GetDppmAsync_WithValidData_ReturnsDppm()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new YieldService(context, _mockLogger.Object);

        var periodStart = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var periodEnd = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        await service.RecordYieldAsync("LOT-DPPM-001", "STEP1", 100, 95, DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        var result = await service.GetDppmAsync("PROD-001", periodStart, periodEnd);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PROD-001", result.ProductId);
    }
}
