using System.Collections.Generic;
using System.Threading.Tasks;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Moq;
using Xunit;

namespace MES.Modules.Production.Tests;

public class TrackServiceTests
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

    private static Mock<ILotRepository> SetupLotRepoWithLot(LotInfo lot)
    {
        var repo = new Mock<ILotRepository>();
        repo.Setup(r => r.GetByIdAsync(lot.LotId))
            .ReturnsAsync(new MES.Infrastructure.Persistence.Entities.ProdLot { LotId = lot.LotId, Status = lot.Status, RouteId = lot.RouteId });
        return repo;
    }

    private static Mock<IRouteService> SetupRouteService()
    {
        var routeService = new Mock<IRouteService>();
        routeService.Setup(r => r.GetStepsAsync(It.IsAny<string>(), It.IsAny<string?>()))
                    .ReturnsAsync(new List<RouteStep>());
        return routeService;
    }

    private static Mock<ICarrierService> SetupCarrierService()
    {
        var carrierService = new Mock<ICarrierService>();
        carrierService.Setup(c => c.BindCarrierAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                      .ReturnsAsync(new LotCarrierBinding());
        return carrierService;
    }

    private static Mock<IQuantityService> SetupQuantityService()
    {
        var quantityService = new Mock<IQuantityService>();
        quantityService.Setup(q => q.ValidateTrackOutQuantityAsync(It.IsAny<TrackOutRequest>()))
                       .ReturnsAsync(new QuantityValidationResult());
        quantityService.Setup(q => q.RecordTransactionAsync(It.IsAny<QuantityTransaction>()))
                       .Returns(Task.CompletedTask);
        return quantityService;
    }

    private static Mock<IOperationHistoryService> SetupOpHistoryService()
    {
        var opHistoryService = new Mock<IOperationHistoryService>();
        opHistoryService.Setup(o => o.WriteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(Task.CompletedTask);
        return opHistoryService;
    }

    private static Mock<IAuditService> SetupAuditService()
    {
        var auditService = new Mock<IAuditService>();
        auditService.Setup(a => a.WriteAsync(It.IsAny<AuditTrail>()))
                    .Returns(Task.CompletedTask);
        return auditService;
    }

    private static Mock<IYieldService> SetupYieldService(double yield = 95.0, bool autoHold = false)
    {
        var yieldService = new Mock<IYieldService>();
        yieldService.Setup(y => y.CalculateStepYieldAsync(It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(yield);
        yieldService.Setup(y => y.ShouldAutoHoldAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
                    .ReturnsAsync(autoHold);
        return yieldService;
    }

    private static Mock<IGenealogyService> SetupGenealogyService()
    {
        var genealogyService = new Mock<IGenealogyService>();
        genealogyService.Setup(g => g.RecordRelationAsync(It.IsAny<LotGenealogy>()))
                        .Returns(Task.CompletedTask);
        return genealogyService;
    }

    [Fact]
    public async Task TrackInAsync_LotNotFound_ReturnsError()
    {
        var lotRepo = new Mock<ILotRepository>();
        lotRepo.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
               .ReturnsAsync((MES.Infrastructure.Persistence.Entities.ProdLot?)null);

        var service = CreateService(lotRepo: lotRepo);
        var request = new TrackInRequest { LotId = "LOT-999", StepCode = "S1", StepSeq = 1 };

        var result = await service.TrackInAsync(request);

        Assert.False(result.Success);
        Assert.Contains("不存在", result.Message);
    }

    [Fact]
    public async Task TrackInAsync_LotAlreadyCompleted_ReturnsError()
    {
        var lot = new LotInfo { LotId = "LOT-001", Status = "Completed" };
        var lotRepo = SetupLotRepoWithLot(lot);
        var routeService = SetupRouteService();
        var carrierService = SetupCarrierService();
        var opHistoryService = SetupOpHistoryService();
        var auditService = SetupAuditService();

        var service = CreateService(
            lotRepo: lotRepo, routeService: routeService,
            carrierService: carrierService, opHistoryService: opHistoryService,
            auditService: auditService);
        var request = new TrackInRequest { LotId = "LOT-001", StepCode = "S1", StepSeq = 1 };

        var result = await service.TrackInAsync(request);

        Assert.False(result.Success);
        Assert.Contains("已完成", result.Message);
    }

    [Fact]
    public async Task TrackInAsync_LotOnHold_ReturnsError()
    {
        var lot = new LotInfo { LotId = "LOT-001", Status = "Hold" };
        var lotRepo = SetupLotRepoWithLot(lot);
        var routeService = SetupRouteService();
        var carrierService = SetupCarrierService();
        var opHistoryService = SetupOpHistoryService();
        var auditService = SetupAuditService();

        var service = CreateService(
            lotRepo: lotRepo, routeService: routeService,
            carrierService: carrierService, opHistoryService: opHistoryService,
            auditService: auditService);
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
               .ReturnsAsync((MES.Infrastructure.Persistence.Entities.ProdLot?)null);

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
    public async Task ValidateTrackInAsync_NullStepCode_ReturnsError()
    {
        var service = CreateService();
        var request = new TrackInRequest { LotId = "LOT-001", StepCode = string.Empty, StepSeq = 1 };

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
