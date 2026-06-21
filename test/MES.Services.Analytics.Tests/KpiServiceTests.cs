using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics.Tests;

[Trait("Category", "UnitTest")]
[Trait("Phase", "5")]
public class KpiServiceTests
{
    private readonly Mock<ILogger<KpiService>> _mockLogger = new();

    private MesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MesDbContext(options);
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsKpiMetrics()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new KpiService(context, _mockLogger.Object);

        // Seed some data
        await service.CaptureMetricAsync(new KpiCaptureRequest
        {
            MetricCode = "YIELD_RATE",
            MetricName = "Yield Rate",
            MetricValue = 98.5m,
            TargetValue = 95m,
            Unit = "%",
            PeriodType = "Daily"
        }, "system");

        // Act
        var result = await service.GetDashboardAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Metrics);
        Assert.NotNull(result.OverallStatus);
    }

    [Fact]
    public async Task GetDashboardAsync_WithData_ReturnsPopulatedMetrics()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new KpiService(context, _mockLogger.Object);

        await service.CaptureMetricAsync(new KpiCaptureRequest
        {
            MetricCode = "YIELD_RATE",
            MetricName = "Yield Rate",
            MetricValue = 98.5m,
            TargetValue = 95m,
            Unit = "%",
            PeriodType = "Daily"
        }, "system");

        // Act
        var result = await service.GetDashboardAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Metrics);
        Assert.Equal("YIELD_RATE", result.Metrics[0].MetricCode);
    }

    [Fact]
    public async Task CaptureMetricAsync_WithValidData_SavesMetric()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new KpiService(context, _mockLogger.Object);
        var request = new KpiCaptureRequest
        {
            MetricCode = "OEE",
            MetricName = "Overall Equipment Effectiveness",
            MetricValue = 85.2m,
            TargetValue = 80m,
            Unit = "%",
            Trend = "Up",
            PeriodType = "Daily"
        };

        // Act
        var result = await service.CaptureMetricAsync(request, "operator1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OEE", result.MetricCode);
        Assert.Equal(85.2m, result.MetricValue);
        Assert.Equal("Normal", result.Status);
        Assert.NotNull(result.SnapshotId);
        Assert.StartsWith("KPI-", result.SnapshotId);
    }

    [Fact]
    public async Task CaptureMetricAsync_WhenValueBelowTarget_ReturnsWarningStatus()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new KpiService(context, _mockLogger.Object);
        var request = new KpiCaptureRequest
        {
            MetricCode = "QUALITY",
            MetricName = "Quality Rate",
            MetricValue = 75m,
            TargetValue = 95m,
            Unit = "%",
            PeriodType = "Daily"
        };

        // Act
        var result = await service.CaptureMetricAsync(request, "operator1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Warning", result.Status);
    }

    [Fact]
    public async Task GetMetricDetailAsync_WhenFound_ReturnsMetric()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new KpiService(context, _mockLogger.Object);

        await service.CaptureMetricAsync(new KpiCaptureRequest
        {
            MetricCode = "THROUGHPUT",
            MetricName = "Throughput",
            MetricValue = 1000m,
            Unit = "units/hour",
            PeriodType = "Hourly"
        }, "system");

        // Act
        var result = await service.GetMetricDetailAsync("THROUGHPUT");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("THROUGHPUT", result.MetricCode);
    }

    [Fact]
    public async Task GetMetricDetailAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new KpiService(context, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetMetricDetailAsync("NONEXISTENT"));
    }
}
