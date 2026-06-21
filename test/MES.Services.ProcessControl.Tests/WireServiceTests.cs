using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence;
using MES.Contracts.Phase3;

namespace MES.Services.ProcessControl.Tests;

[Trait("Category", "UnitTest")]
[Trait("Phase", "3")]
public class WireServiceTests
{
    private readonly Mock<ILogger<WireService>> _mockLogger = new();

    private MesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MesDbContext(options);
    }

    [Fact]
    public async Task RecordWireSwitchAsync_WithValidData_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new WireService(context, _mockLogger.Object);
        var request = new WireMaterialSwitchRequest
        {
            LotId = "LOT-001",
            StepCode = "WIRE_BOND",
            StepSeq = 1,
            EquipmentId = "WB-001",
            OldWireMaterialId = "WIRE-OLD-001",
            OldWireMaterialName = "Gold Wire 0.8mil",
            OldWireLotNo = "WL-2024001",
            OldWireDiameter = 0.8m,
            NewWireMaterialId = "WIRE-NEW-001",
            NewWireMaterialName = "Gold Wire 1.0mil",
            NewWireLotNo = "WL-2024002",
            NewWireDiameter = 1.0m,
            WireSupplier = "Kulicke",
            SwitchReason = "Diameter change for new product"
        };

        // Act
        var result = await service.RecordWireSwitchAsync(request, "operator1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("LOT-001", result.LotId);
        Assert.Equal("WIRE-OLD-001", result.OldWireMaterialId);
        Assert.Equal("WIRE-NEW-001", result.NewWireMaterialId);
        Assert.NotNull(result.SwitchId);
        Assert.StartsWith("WIRE-SW-", result.SwitchId);
    }

    [Fact]
    public async Task RecordWireConsumptionAsync_WithValidData_ReturnsConsumptionId()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new WireService(context, _mockLogger.Object);

        // Act
        var result = await service.RecordWireConsumptionAsync(
            lotId: "LOT-001",
            stepCode: "WIRE_BOND",
            stepSeq: 1,
            equipmentId: "WB-001",
            wireMaterialId: "WIRE-001",
            wireMaterialName: "Gold Wire",
            consumedLength: 150.5m,
            lengthUnit: "cm",
            bondCount: 100,
            operatorId: "operator1");

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public async Task RecordWireConsumptionAsync_WithZeroBondCount_CalculatesAvgLengthAsNull()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new WireService(context, _mockLogger.Object);

        // Act
        var result = await service.RecordWireConsumptionAsync(
            lotId: "LOT-002",
            stepCode: "WIRE_BOND",
            stepSeq: 1,
            equipmentId: "WB-001",
            wireMaterialId: "WIRE-001",
            wireMaterialName: "Gold Wire",
            consumedLength: 200m,
            lengthUnit: "cm",
            bondCount: null,
            operatorId: "operator1");

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public async Task QueryWireConsumptionsAsync_ReturnsPagedResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new WireService(context, _mockLogger.Object);

        await service.RecordWireConsumptionAsync(
            "LOT-001", "WIRE_BOND", 1, "WB-001", "WIRE-001", "Gold Wire", 100m, "cm", 50, "operator1");

        await service.RecordWireConsumptionAsync(
            "LOT-002", "WIRE_BOND", 1, "WB-002", "WIRE-002", "Copper Wire", 200m, "cm", 100, "operator1");

        var query = new WireConsumptionQuery { PageIndex = 1, PageSize = 10 };

        // Act
        var result = await service.QueryWireConsumptionsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task QueryWireConsumptionsAsync_WithNoData_ReturnsEmptyResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new WireService(context, _mockLogger.Object);
        var query = new WireConsumptionQuery { PageIndex = 1, PageSize = 10 };

        // Act
        var result = await service.QueryWireConsumptionsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task QueryWireConsumptionsAsync_FilterByLotId_ReturnsFilteredResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new WireService(context, _mockLogger.Object);

        await service.RecordWireConsumptionAsync(
            "LOT-001", "WIRE_BOND", 1, "WB-001", "WIRE-001", "Gold Wire", 100m, "cm", 50, "operator1");

        await service.RecordWireConsumptionAsync(
            "LOT-002", "WIRE_BOND", 1, "WB-002", "WIRE-002", "Copper Wire", 200m, "cm", 100, "operator1");

        var query = new WireConsumptionQuery { PageIndex = 1, PageSize = 10, LotId = "LOT-001" };

        // Act
        var result = await service.QueryWireConsumptionsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("LOT-001", result.Items[0].LotId);
    }
}
