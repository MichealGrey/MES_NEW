using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics.Tests;

[Trait("Category", "UnitTest")]
[Trait("Phase", "5")]
public class CostServiceTests
{
    private readonly Mock<ILogger<CostService>> _mockLogger = new();

    private MesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MesDbContext(options);
    }

    [Fact]
    public async Task RecordCostAsync_WithValidData_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new CostService(context, _mockLogger.Object);
        var request = new CostCalculationRequest
        {
            WorkOrderId = "WO-001",
            ProductId = "PROD-001",
            DirectMaterialCost = 150.50m,
            DirectLaborCost = 75.25m,
            ManufacturingOverhead = 50.00m
        };

        // Act
        var result = await service.RecordCostAsync(request, "operator1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("WO-001", result.WorkOrderId);
        Assert.Equal("PROD-001", result.ProductId);
        Assert.Equal("DirectMaterial", result.CostType);
        Assert.Equal(150.50m, result.Amount);
        Assert.Equal("CNY", result.Currency);
        Assert.NotNull(result.CostId);
        Assert.StartsWith("COST-", result.CostId);
    }

    [Fact]
    public async Task GetProductAnalysisAsync_ReturnsCostBreakdown()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new CostService(context, _mockLogger.Object);

        await service.RecordCostAsync(new CostCalculationRequest
        {
            WorkOrderId = "WO-001",
            ProductId = "PROD-001",
            DirectMaterialCost = 100m,
            DirectLaborCost = 50m,
            ManufacturingOverhead = 30m
        }, "operator1");

        await service.RecordCostAsync(new CostCalculationRequest
        {
            WorkOrderId = "WO-002",
            ProductId = "PROD-001",
            DirectMaterialCost = 200m,
            DirectLaborCost = 100m,
            ManufacturingOverhead = 60m
        }, "operator1");

        // Act
        var result = await service.GetProductAnalysisAsync("PROD-001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PROD-001", result.ProductId);
        Assert.True(result.TotalCost > 0);
        Assert.True(result.DirectMaterialCost > 0);
    }

    [Fact]
    public async Task GetProductAnalysisAsync_WithDateRange_ReturnsFilteredBreakdown()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new CostService(context, _mockLogger.Object);

        await service.RecordCostAsync(new CostCalculationRequest
        {
            WorkOrderId = "WO-001",
            ProductId = "PROD-002",
            DirectMaterialCost = 100m,
            DirectLaborCost = 50m,
            ManufacturingOverhead = 30m
        }, "operator1");

        // Act
        var result = await service.GetProductAnalysisAsync("PROD-002",
            startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
            endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PROD-002", result.ProductId);
    }

    [Fact]
    public async Task GetWorkOrderAnalysisAsync_WithValidData_ReturnsAnalysis()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new CostService(context, _mockLogger.Object);

        await service.RecordCostAsync(new CostCalculationRequest
        {
            WorkOrderId = "WO-ANALYSIS",
            ProductId = "PROD-001",
            DirectMaterialCost = 500m,
            DirectLaborCost = 200m,
            ManufacturingOverhead = 100m
        }, "operator1");

        // Act
        var result = await service.GetWorkOrderAnalysisAsync("WO-ANALYSIS");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("WO-ANALYSIS", result.WorkOrderId);
        Assert.True(result.TotalCost > 0);
    }

    [Fact]
    public async Task GetWorkOrderAnalysisAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new CostService(context, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetWorkOrderAnalysisAsync("NONEXISTENT"));
    }

    [Fact]
    public async Task CalculateCostsAsync_WithValidData_CreatesMultipleRecords()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new CostService(context, _mockLogger.Object);
        var request = new CostCalculationRequest
        {
            WorkOrderId = "WO-001",
            ProductId = "PROD-001",
            DirectMaterialCost = 100m,
            DirectLaborCost = 50m,
            ManufacturingOverhead = 25m
        };

        // Act
        var results = await service.CalculateCostsAsync(request, "operator1");

        // Assert
        Assert.NotNull(results);
        Assert.Equal(3, results.Count);
        Assert.Contains(results, r => r.CostType == "DirectMaterial");
        Assert.Contains(results, r => r.CostType == "DirectLabor");
        Assert.Contains(results, r => r.CostType == "ManufacturingOverhead");
    }

    [Fact]
    public async Task GetVarianceAnalysisAsync_ReturnsOrderedRecords()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new CostService(context, _mockLogger.Object);

        await service.RecordCostAsync(new CostCalculationRequest
        {
            WorkOrderId = "WO-VAR",
            ProductId = "PROD-001",
            DirectMaterialCost = 100m,
            DirectLaborCost = 50m,
            ManufacturingOverhead = 25m
        }, "operator1");

        // Act
        var results = await service.GetVarianceAnalysisAsync("WO-VAR");

        // Assert
        Assert.NotNull(results);
        Assert.NotEmpty(results);
    }
}
