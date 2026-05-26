using System.Collections.Generic;
using System.Threading.Tasks;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Moq;
using Xunit;

namespace MES.Modules.Production.Tests;

public class TrackIntegrationTests
{
    private TrackService CreateService(
        Mock<ILotRepository>? lotRepo = null,
        Mock<IRouteRepository>? routeRepo = null,
        Mock<IRouteService>? routeService = null,
        Mock<IQuantityService>? quantityService = null,
        Mock<IOperationHistoryService>? opHistoryService = null,
        Mock<IAuditService>? auditService = null,
        Mock<IYieldService>? yieldService = null,
        Mock<IGenealogyService>? genealogyService = null,
        Mock<ICarrierService>? carrierService = null,
        Mock<IEquipmentGateway>? equipmentGateway = null,
        Mock<IRecipeGateway>? recipeGateway = null,
        Mock<IQualityGateway>? qualityGateway = null,
        Mock<IWarehouseGateway>? warehouseGateway = null,
        Mock<IAlarmService>? alarmService = null,
        Mock<ICarrierBindingRepository>? carrierBindingRepo = null,
        Mock<IQuantityTransactionRepository>? qtyTxnRepo = null)
    {
        return new TrackService(
            lotRepo?.Object ?? Mock.Of<ILotRepository>(),
            routeRepo?.Object ?? Mock.Of<IRouteRepository>(),
            routeService?.Object ?? Mock.Of<IRouteService>(),
            quantityService?.Object ?? Mock.Of<IQuantityService>(),
            opHistoryService?.Object ?? Mock.Of<IOperationHistoryService>(),
            auditService?.Object ?? Mock.Of<IAuditService>(),
            yieldService?.Object ?? Mock.Of<IYieldService>(),
            genealogyService?.Object ?? Mock.Of<IGenealogyService>(),
            carrierService?.Object ?? Mock.Of<ICarrierService>(),
            equipmentGateway?.Object ?? Mock.Of<IEquipmentGateway>(),
            recipeGateway?.Object ?? Mock.Of<IRecipeGateway>(),
            qualityGateway?.Object ?? Mock.Of<IQualityGateway>(),
            warehouseGateway?.Object ?? Mock.Of<IWarehouseGateway>(),
            alarmService?.Object ?? Mock.Of<IAlarmService>(),
            carrierBindingRepo?.Object ?? Mock.Of<ICarrierBindingRepository>(),
            qtyTxnRepo?.Object ?? Mock.Of<IQuantityTransactionRepository>());
    }

    private static Mock<ILotRepository> SetupLotRepo(string lotId, string status, string? routeId = null)
    {
        var repo = new Mock<ILotRepository>();
        repo.Setup(r => r.GetByIdAsync(lotId))
            .ReturnsAsync(new ProdLot { LotId = lotId, Status = status, RouteId = routeId ?? string.Empty });
        return repo;
    }

    [Fact]
    public async Task TrackInAsync_LotNotFound_ReturnsError()
    {
        var lotRepo = new Mock<ILotRepository>();
        lotRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((ProdLot?)null);

        var service = CreateService(lotRepo: lotRepo);
        var request = new TrackInRequest { LotId = "LOT-999", StepCode = "S1", StepSeq = 1 };

        var result = await service.TrackInAsync(request);

        Assert.False(result.Success);
        Assert.Contains("不存在", result.Message);
    }

    [Fact]
    public async Task TrackInAsync_LotAlreadyCompleted_ReturnsError()
    {
        var lotRepo = SetupLotRepo("LOT-001", "Completed");
        var service = CreateService(lotRepo: lotRepo);
        var request = new TrackInRequest { LotId = "LOT-001", StepCode = "S1", StepSeq = 1 };

        var result = await service.TrackInAsync(request);

        Assert.False(result.Success);
        Assert.Contains("已完成", result.Message);
    }

    [Fact]
    public async Task TrackInAsync_LotOnHold_ReturnsError()
    {
        var lotRepo = SetupLotRepo("LOT-001", "Hold");
        var service = CreateService(lotRepo: lotRepo);
        var request = new TrackInRequest { LotId = "LOT-001", StepCode = "S1", StepSeq = 1 };

        var result = await service.TrackInAsync(request);

        Assert.False(result.Success);
        Assert.Contains("Hold", result.Message);
    }

    [Fact]
    public async Task TrackOutAsync_LotNotFound_ReturnsError()
    {
        var lotRepo = new Mock<ILotRepository>();
        lotRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((ProdLot?)null);

        var service = CreateService(lotRepo: lotRepo);
        var request = new TrackOutRequest { LotId = "LOT-999", StepCode = "S1", PassQty = 10, InputQty = 10 };

        var result = await service.TrackOutAsync(request);

        Assert.False(result.Success);
        Assert.Contains("未进站", result.Message);
    }

    [Fact]
    public async Task ValidateTrackInAsync_NullLotId_ReturnsError()
    {
        var service = CreateService();
        var request = new TrackInRequest { LotId = string.Empty, StepCode = "S1", StepSeq = 1 };

        var result = await service.ValidateTrackInAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ValidateTrackOutAsync_NegativePassQty_ReturnsError()
    {
        var service = CreateService();
        var request = new TrackOutRequest { LotId = "LOT-001", StepCode = "S1", PassQty = -1, InputQty = 10 };

        var result = await service.ValidateTrackOutAsync(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ValidateTrackOutAsync_PassQtyExceedsInputQty_ReturnsError()
    {
        var service = CreateService();
        var request = new TrackOutRequest { LotId = "LOT-001", StepCode = "S1", PassQty = 20, InputQty = 10 };

        var result = await service.ValidateTrackOutAsync(request);

        Assert.False(result.IsValid);
    }
}
