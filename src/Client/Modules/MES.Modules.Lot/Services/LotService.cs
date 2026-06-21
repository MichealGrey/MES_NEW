using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Domain.Production;

namespace MES.Modules.Lot.Services;

public interface ILotService
{
    Task<List<LotInfo>> GetAllLotsAsync();
    Task<LotInfo?> GetLotAsync(string lotId);
    Task<List<LotInfo>> GetLotsByStatusAsync(string status);
    Task<List<LotInfo>> GetHoldLotsAsync();
    Task<List<LotInfo>> GetArchivedLotsAsync();
    Task SaveLotAsync(LotInfo lot);
    Task UpdateLotStatusAsync(string lotId, string status);
    Task HoldLotAsync(string lotId, string reason, string? category = null, string? remark = null);
    Task ReleaseLotAsync(string lotId);
    Task ArchiveLotAsync(string lotId);
    Task DeleteLotAsync(string lotId);
}

public class LotInfo
{
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string DieName { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int UnitCount { get; set; }
    public int StripCount { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public int RouteVersion { get; set; }
    public int CurrentStepSeq { get; set; }
    public bool IsPartialLot { get; set; }
    public string? MotherLotId { get; set; }
    public string? SplitReason { get; set; }
    public DateTime? SplitTime { get; set; }
    public int? SplitQty { get; set; }
    public bool IsReworkLot { get; set; }
    public string? OriginalRouteId { get; set; }
    public string? ReworkRouteId { get; set; }
    public int ReworkCount { get; set; }
    public string? ReworkReason { get; set; }
    public bool IsArchived { get; set; }
    public bool IsUnderMRB { get; set; }
    public string? MRBReference { get; set; }
    public string? MRBDisposition { get; set; }
    public string? Grade { get; set; }
    public string? OriginalLotId { get; set; }
    public string? WaferLotId { get; set; }
    public int OriginalQty { get; set; }
    public int TotalPassQty { get; set; }
    public int TotalScrapQty { get; set; }
    public int TotalReworkQty { get; set; }
    public int TotalHoldQty { get; set; }
    public string CarrierType { get; set; } = string.Empty;
    public string CarrierId { get; set; } = string.Empty;
    public string? BinResult { get; set; }
    public string? TestResult { get; set; }
    public int QtyPass { get; set; }
    public int QtyFail { get; set; }
    public string? HoldCategory { get; set; }
    public string? HoldReason { get; set; }
    public DateTime? HoldTime { get; set; }
    public string? HoldOperator { get; set; }
    public string? ReleaseCondition { get; set; }
    public string ProcessStage { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Remark { get; set; }
}

public class LotService : ILotService
{
    private readonly IRepository<ProdLot> _lotRepo;

    public LotService(IRepository<ProdLot> lotRepo)
    {
        _lotRepo = lotRepo;
    }

    public async Task<List<LotInfo>> GetAllLotsAsync()
    {
        var entities = await _lotRepo.GetAllAsync();
        return entities.Select(MapToLotInfo).ToList();
    }

    public async Task<LotInfo?> GetLotAsync(string lotId)
    {
        var entity = await _lotRepo.GetByIdAsync(lotId);
        return entity is null ? null : MapToLotInfo(entity);
    }

    public async Task<List<LotInfo>> GetLotsByStatusAsync(string status)
    {
        var entities = await _lotRepo.GetWhereAsync(l => l.Status == status);
        return entities.Select(MapToLotInfo).ToList();
    }

    public async Task<List<LotInfo>> GetHoldLotsAsync()
    {
        var entities = await _lotRepo.GetWhereAsync(l => l.Status == "Hold");
        return entities.Select(MapToLotInfo).ToList();
    }

    public async Task<List<LotInfo>> GetArchivedLotsAsync()
    {
        var entities = await _lotRepo.GetWhereAsync(l => l.IsArchived);
        return entities.Select(MapToLotInfo).ToList();
    }

    public async Task SaveLotAsync(LotInfo lot)
    {
        var entity = MapToEntity(lot);
        var existing = await _lotRepo.GetByIdAsync(lot.LotId);
        if (existing is null)
            await _lotRepo.AddAsync(entity);
        else
            await _lotRepo.UpdateAsync(entity);
    }

    public async Task UpdateLotStatusAsync(string lotId, string status)
    {
        var entity = await _lotRepo.GetByIdAsync(lotId);
        if (entity is null) return;

        entity.Status = status;
        entity.UpdatedAt = DateTime.UtcNow;
        await _lotRepo.UpdateAsync(entity);
    }

    public async Task HoldLotAsync(string lotId, string reason, string? category = null, string? remark = null)
    {
        var entity = await _lotRepo.GetByIdAsync(lotId);
        if (entity is null) return;

        entity.Status = "Hold";
        entity.HoldReason = reason;
        entity.HoldCategory = category ?? "Engineering";
        entity.HoldTime = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
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
        entity.UpdatedAt = DateTime.UtcNow;
        await _lotRepo.UpdateAsync(entity);
    }

    public async Task ArchiveLotAsync(string lotId)
    {
        var entity = await _lotRepo.GetByIdAsync(lotId);
        if (entity is null) return;

        entity.IsArchived = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _lotRepo.UpdateAsync(entity);
    }

    public async Task DeleteLotAsync(string lotId)
    {
        var entity = await _lotRepo.GetByIdAsync(lotId);
        if (entity is not null)
            await _lotRepo.DeleteAsync(entity);
    }

    private static LotInfo MapToLotInfo(ProdLot e) => new()
    {
        LotId = e.LotId,
        OrderId = e.OrderId,
        ProductId = e.ProductId,
        ProductName = e.ProductName,
        DieName = e.DieName ?? string.Empty,
        CurrentStep = e.CurrentStepCode ?? string.Empty,
        Status = e.Status ?? "Created",
        UnitCount = e.UnitCount,
        StripCount = e.StripCount,
        Priority = e.Priority ?? "Normal",
        RouteId = e.RouteId,
        RouteVersion = int.TryParse(e.RouteVersion, out var rv) ? rv : 1,
        CurrentStepSeq = e.CurrentStepSeq,
        IsPartialLot = e.IsPartialLot,
        MotherLotId = e.MotherLotId,
        SplitReason = e.SplitReason,
        SplitTime = e.SplitTime,
        SplitQty = e.SplitQty ?? 0,
        IsReworkLot = e.IsReworkLot,
        OriginalRouteId = e.OriginalRouteId,
        ReworkRouteId = e.ReworkRouteId,
        ReworkCount = e.ReworkCount ?? 0,
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
        CarrierType = e.CarrierType ?? string.Empty,
        CarrierId = e.CarrierId ?? string.Empty,
        BinResult = e.BinResult,
        TestResult = e.TestResult,
        QtyPass = e.QtyPass,
        QtyFail = e.QtyFail,
        HoldCategory = e.HoldCategory,
        HoldReason = e.HoldReason,
        HoldTime = e.HoldTime,
        HoldOperator = e.HoldOperator,
        ReleaseCondition = e.ReleaseCondition,
        ProcessStage = e.ProcessStage ?? string.Empty,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };

    private static ProdLot MapToEntity(LotInfo m) => new()
    {
        LotId = m.LotId,
        OrderId = m.OrderId,
        ProductId = m.ProductId,
        ProductName = m.ProductName,
        DieName = m.DieName,
        CurrentStepCode = m.CurrentStep,
        Status = m.Status,
        UnitCount = m.UnitCount,
        StripCount = m.StripCount,
        Priority = m.Priority,
        RouteId = m.RouteId,
        RouteVersion = m.RouteVersion.ToString(),
        CurrentStepSeq = m.CurrentStepSeq,
        IsPartialLot = m.IsPartialLot,
        MotherLotId = m.MotherLotId,
        SplitReason = m.SplitReason,
        SplitTime = m.SplitTime,
        SplitQty = m.SplitQty,
        IsReworkLot = m.IsReworkLot,
        OriginalRouteId = m.OriginalRouteId,
        ReworkRouteId = m.ReworkRouteId,
        ReworkCount = m.ReworkCount,
        ReworkReason = m.ReworkReason,
        IsArchived = m.IsArchived,
        IsUnderMrb = m.IsUnderMRB,
        MrbReference = m.MRBReference,
        MrbDisposition = m.MRBDisposition,
        Grade = m.Grade,
        OriginalLotId = m.OriginalLotId,
        WaferLotId = m.WaferLotId,
        OriginalQty = m.OriginalQty,
        TotalPassQty = m.TotalPassQty,
        TotalScrapQty = m.TotalScrapQty,
        TotalReworkQty = m.TotalReworkQty,
        TotalHoldQty = m.TotalHoldQty,
        CarrierType = m.CarrierType,
        CarrierId = m.CarrierId,
        BinResult = m.BinResult,
        TestResult = m.TestResult,
        QtyPass = m.QtyPass,
        QtyFail = m.QtyFail,
        HoldCategory = m.HoldCategory,
        HoldReason = m.HoldReason,
        HoldTime = m.HoldTime,
        HoldOperator = m.HoldOperator,
        ReleaseCondition = m.ReleaseCondition,
        ProcessStage = m.ProcessStage,
        UpdatedAt = DateTime.UtcNow,
    };
}
