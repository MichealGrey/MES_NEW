using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;
using MES.Domain.Production;

namespace MES.Modules.Production.Services;

public class ProductionDataService : IProductionDataService
{
    private readonly IRepository<ProdLot> _lotRepo;
    private readonly IRepository<ProdWorkOrder> _woRepo;
    private readonly IRepository<ProdLotStep> _lotStepRepo;
    private readonly IRepository<ProdHoldRecord> _holdRepo;
    private readonly IRepository<ProdLotArchive> _archiveRepo;

    public ProductionDataService(
        IRepository<ProdLot> lotRepo,
        IRepository<ProdWorkOrder> woRepo,
        IRepository<ProdLotStep> lotStepRepo,
        IRepository<ProdHoldRecord> holdRepo,
        IRepository<ProdLotArchive> archiveRepo)
    {
        _lotRepo = lotRepo;
        _woRepo = woRepo;
        _lotStepRepo = lotStepRepo;
        _holdRepo = holdRepo;
        _archiveRepo = archiveRepo;
    }

    public async Task EnsureSeededAsync()
    {
        var existingLots = await _lotRepo.GetAllAsync();
        if (existingLots.Count > 0) return;

        // Seed data is now managed via SQL migration scripts.
        // This method serves as a placeholder for future programmatic seeding.
        // Refer to docs/sql/mes_mock_data.sql for seed data.
    }

    // --- WorkOrder CRUD ---

    public async Task<List<WorkOrderInfo>> GetAllWorkOrdersAsync()
    {
        var entities = await _woRepo.GetAllAsync();
        return entities.Select(MapToWorkOrderInfo).ToList();
    }

    public async Task<WorkOrderInfo?> GetWorkOrderAsync(string orderId)
    {
        var entity = await _woRepo.GetByIdAsync(orderId);
        return entity is null ? null : MapToWorkOrderInfo(entity);
    }

    public async Task SaveWorkOrderAsync(WorkOrderInfo workOrder)
    {
        var entity = MapToEntity(workOrder);
        var existing = await _woRepo.GetByIdAsync(workOrder.OrderId);
        if (existing is null)
            await _woRepo.AddAsync(entity);
        else
            await _woRepo.UpdateAsync(entity);
    }

    public async Task DeleteWorkOrderAsync(string orderId)
    {
        var entity = await _woRepo.GetByIdAsync(orderId);
        if (entity is not null)
            await _woRepo.DeleteAsync(entity);
    }

    public async Task UpdateWorkOrderStatusAsync(string orderId, ProcessStatus status)
    {
        var entity = await _woRepo.GetByIdAsync(orderId);
        if (entity is null) return;

        entity.Status = status.ToString();
        entity.UpdatedAt = DateTime.UtcNow;

        if (status == ProcessStatus.Completed)
            entity.ActualEndDate = DateTime.UtcNow;
        else if (status == ProcessStatus.InProgress && !entity.ActualStartDate.HasValue)
            entity.ActualStartDate = DateTime.UtcNow;

        await _woRepo.UpdateAsync(entity);
    }

    public async Task HoldWorkOrderAsync(string orderId, string reason, string? remark = null)
    {
        var entity = await _woRepo.GetByIdAsync(orderId);
        if (entity is null) return;

        entity.Status = "Hold";
        entity.Remark = remark;
        entity.UpdatedAt = DateTime.UtcNow;
        await _woRepo.UpdateAsync(entity);
    }

    public async Task ReleaseHoldWorkOrderAsync(string orderId)
    {
        var entity = await _woRepo.GetByIdAsync(orderId);
        if (entity is null) return;

        entity.Status = "Released";
        entity.UpdatedAt = DateTime.UtcNow;
        await _woRepo.UpdateAsync(entity);
    }

    // --- Lot queries ---

    public async Task<List<LotInfo>> GetAllLotsAsync()
    {
        var lots = await _lotRepo.GetAllAsync();
        return lots.Select(MapToLotInfo).ToList();
    }

    public async Task<List<LotInfo>> GetAllHoldLotsAsync()
    {
        var lots = await _lotRepo.GetWhereAsync(l => l.Status == "Hold");
        return lots.Select(MapToLotInfo).ToList();
    }

    public async Task<List<LotInfo>> GetLotsByStageAsync(string processStage)
    {
        var lots = await _lotRepo.GetWhereAsync(l => l.ProcessStage == processStage);
        return lots.Select(MapToLotInfo).ToList();
    }

    public async Task HoldLotAsync(LotInfo lot)
    {
        var entity = await _lotRepo.GetByIdAsync(lot.LotId);
        if (entity is null) return;

        entity.Status = "Hold";
        entity.HoldReason = lot.HoldReason;
        entity.HoldTime = lot.HoldTime;
        entity.HoldOperator = lot.HoldOperator;
        entity.HoldCategory = lot.HoldCategory.ToString();
        entity.ReleaseCondition = lot.ReleaseCondition;
        entity.IsUnderMrb = lot.IsUnderMRB;
        entity.MrbReference = lot.MRBReference;
        entity.MrbDisposition = lot.MRBDisposition;
        await _lotRepo.UpdateAsync(entity);
    }

    public async Task ReleaseLotAsync(string lotId)
    {
        var entity = await _lotRepo.GetByIdAsync(lotId);
        if (entity is null) return;

        entity.Status = "InProgress";
        entity.HoldReason = null;
        entity.HoldTime = null;
        entity.HoldOperator = null;
        entity.HoldCategory = null;
        entity.ReleaseCondition = null;
        entity.IsUnderMrb = false;
        entity.MrbReference = null;
        entity.MrbDisposition = null;
        await _lotRepo.UpdateAsync(entity);
    }

    public async Task BatchReleaseAsync()
    {
        var holdLots = await _lotRepo.GetWhereAsync(l => l.Status == "Hold");
        foreach (var lot in holdLots)
        {
            lot.Status = "InProgress";
            lot.HoldReason = null;
            lot.HoldTime = null;
            lot.HoldOperator = null;
            lot.HoldCategory = null;
            lot.ReleaseCondition = null;
            lot.IsUnderMrb = false;
            lot.MrbReference = null;
            lot.MrbDisposition = null;
            await _lotRepo.UpdateAsync(lot);
        }
    }

    // --- Mapping helpers ---

    private static WorkOrderInfo MapToWorkOrderInfo(ProdWorkOrder e) => new()
    {
        OrderId = e.OrderId,
        ProductId = e.ProductId,
        ProductName = e.ProductName,
        PlannedQty = e.PlannedQty,
        CompletedQty = e.CompletedQty,
        Status = Enum.TryParse<ProcessStatus>(e.Status, true, out var s) ? s : ProcessStatus.Created,
        Priority = Enum.TryParse<WorkOrderPriority>(e.Priority, true, out var p) ? p : WorkOrderPriority.Normal,
        CreatedAt = e.CreatedAt,
        RouteId = e.RouteId,
        RouteName = e.RouteName,
        DieName = e.DieName ?? string.Empty,
        PackageType = Enum.TryParse<PackageType>(e.PackageType, true, out var pt) ? pt : PackageType.QFP,
        WaferQty = e.WaferQty,
        UnitQty = e.UnitQty,
        CustomerId = e.CustomerId ?? string.Empty,
        CustomerName = e.CustomerName ?? string.Empty,
        CustomerPN = e.CustomerPn ?? string.Empty,
        InternalPN = e.InternalPn ?? string.Empty,
        Creator = e.Creator,
        PlannedStartDate = e.PlannedStartDate,
        PlannedEndDate = e.PlannedEndDate,
        ActualStartDate = e.ActualStartDate,
        ActualEndDate = e.ActualEndDate,
        TargetCPYield = (double)(e.TargetCpYield ?? 99.0m),
        TargetFTYield = (double)(e.TargetFtYield ?? 98.0m),
        YieldTarget = (double)(e.TargetFtYield ?? 98.0m),
        Remark = e.Remark,
    };

    private static ProdWorkOrder MapToEntity(WorkOrderInfo m) => new()
    {
        OrderId = m.OrderId,
        ProductId = m.ProductId,
        ProductName = m.ProductName,
        RouteId = m.RouteId,
        RouteName = m.RouteName,
        DieName = m.DieName,
        PackageType = m.PackageType.ToString(),
        PlannedQty = m.PlannedQty,
        CompletedQty = m.CompletedQty,
        WaferQty = m.WaferQty,
        UnitQty = m.UnitQty,
        CustomerId = m.CustomerId,
        CustomerName = m.CustomerName,
        CustomerPn = m.CustomerPN,
        InternalPn = m.InternalPN,
        Priority = m.Priority.ToString(),
        Status = m.Status.ToString(),
        Creator = m.Creator,
        PlannedStartDate = m.PlannedStartDate,
        PlannedEndDate = m.PlannedEndDate,
        ActualStartDate = m.ActualStartDate,
        ActualEndDate = m.ActualEndDate,
        TargetCpYield = (decimal)m.TargetCPYield,
        TargetFtYield = (decimal)m.TargetFTYield,
        Remark = m.Remark,
        UpdatedAt = DateTime.UtcNow,
    };

    private static LotInfo MapToLotInfo(ProdLot e) => new()
    {
        LotId = e.LotId,
        OrderId = e.OrderId,
        ProductId = e.ProductId,
        ProductName = e.ProductName,
        DieName = e.DieName ?? string.Empty,
        CurrentStep = e.CurrentStepCode ?? string.Empty,
        Status = e.Status,
        UnitCount = e.UnitCount,
        StripCount = e.StripCount,
        Priority = e.Priority,
        RouteId = e.RouteId,
        RouteVersion = e.RouteVersion,
        CurrentStepSeq = e.CurrentStepSeq,
        IsPartialLot = e.IsPartialLot,
        MotherLotId = e.MotherLotId,
        SplitReason = e.SplitReason,
        SplitTime = e.SplitTime,
        SplitQty = e.SplitQty,
        IsReworkLot = e.IsReworkLot,
        OriginalRouteId = e.OriginalRouteId,
        ReworkRouteId = e.ReworkRouteId,
        ReworkCount = e.ReworkCount,
        ReworkReason = e.ReworkReason,
        IsArchived = e.IsArchived,
        IsUnderMRB = e.IsUnderMrb,
        MRBReference = e.MrbReference,
        MRBDisposition = e.MrbDisposition,
        Grade = e.Grade,
        OriginalLotId = e.OriginalLotId,
        WaferLotId = e.WaferLotId,
        OriginalQty = e.OriginalQty,
        TotalPassQty = e.TotalPassQty,
        TotalScrapQty = e.TotalScrapQty,
        TotalReworkQty = e.TotalReworkQty,
        TotalHoldQty = e.TotalHoldQty,
        CarrierType = Enum.TryParse<CarrierType>(e.CarrierType, true, out var ct) ? ct : CarrierType.Strip,
        CarrierId = e.CarrierId ?? string.Empty,
        BinResult = e.BinResult,
        TestResult = e.TestResult,
        QtyPass = e.QtyPass,
        QtyFail = e.QtyFail,
        HoldCategory = Enum.TryParse<HoldType>(e.HoldCategory, true, out var ht) ? ht : HoldType.Engineering,
        HoldReason = e.HoldReason,
        HoldTime = e.HoldTime,
        HoldOperator = e.HoldOperator,
        ReleaseCondition = e.ReleaseCondition,
        ProcessStage = MapProcessStage(e.ProcessStage),
    };

    private static ProcessStage MapProcessStage(string? dbValue) => dbValue switch
    {
        "Frontend" => ProcessStage.Assemble,
        "Backend" => ProcessStage.Test,
        _ => ProcessStage.Assemble
    };

    // --- Archive queries ---

    public async Task<List<ProdLotArchive>> GetArchivedLotsAsync()
    {
        return await _archiveRepo.GetAllAsync();
    }

    public async Task<List<ProdLotArchive>> GetArchivedLotsByStageAsync(string processStage)
    {
        return await _archiveRepo.GetWhereAsync(a => a.ProcessStage == processStage);
    }
}
