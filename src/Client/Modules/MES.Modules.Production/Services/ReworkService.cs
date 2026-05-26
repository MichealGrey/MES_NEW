using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class ReworkService : IReworkService
{
    private readonly IRepository<ProdLot> _lotRepo;
    private readonly IRepository<ProdReworkRecord> _reworkRepo;
    private readonly IRepository<ProdLotSplit> _splitRepo;
    private readonly IRepository<MasterRoute> _routeRepo;
    private readonly IGenealogyService _genealogyService;

    public ReworkService(
        IRepository<ProdLot> lotRepo,
        IRepository<ProdReworkRecord> reworkRepo,
        IRepository<ProdLotSplit> splitRepo,
        IRepository<MasterRoute> routeRepo,
        IGenealogyService genealogyService)
    {
        _lotRepo = lotRepo;
        _reworkRepo = reworkRepo;
        _splitRepo = splitRepo;
        _routeRepo = routeRepo;
        _genealogyService = genealogyService;
    }

    public async Task<ReworkRecord> CreateReworkLotAsync(string lotId, string reworkRouteId,
        string fromStepCode, string targetStepCode, int reworkQty, string reason,
        string operatorId, string operatorName)
    {
        var lot = await _lotRepo.GetByIdAsync(lotId);
        if (lot is null)
            throw new InvalidOperationException($"批次 {lotId} 不存在");

        // 获取重工路线
        var reworkRoute = await _routeRepo.GetByIdAsync(reworkRouteId);
        if (reworkRoute is null)
            throw new InvalidOperationException($"重工路线 {reworkRouteId} 不存在");

        var firstStep = reworkRoute.Steps.OrderBy(s => s.StepSeq).FirstOrDefault();
        if (firstStep is null)
            throw new InvalidOperationException($"重工路线 {reworkRouteId} 没有工序");

        if (lot.UnitCount < reworkQty)
            throw new InvalidOperationException($"重工数量 {reworkQty} 超过可用数量 {lot.UnitCount}");

        // 创建重工子批次
        var reworkLotId = $"{lotId}-RW{Guid.NewGuid().ToString("N")[..4]}";

        var reworkLot = new ProdLot
        {
            LotId = reworkLotId,
            OrderId = lot.OrderId,
            ProductId = lot.ProductId,
            ProductName = lot.ProductName,
            DieName = lot.DieName ?? string.Empty,
            PackageType = lot.PackageType,
            RouteId = lot.RouteId,
            RouteVersion = lot.RouteVersion,
            ReworkRouteId = reworkRouteId,
            CurrentStepCode = firstStep.StepCode,
            CurrentStepSeq = 1,
            Status = "Waiting",
            IsReworkLot = true,
            OriginalRouteId = lot.RouteId,
            ReworkCount = (lot.ReworkCount ?? 0) + 1,
            ReworkReason = reason,
            OriginalQty = reworkQty,
            UnitCount = reworkQty,
            StripCount = Math.Max(1, reworkQty / (lot.UnitCount / Math.Max(1, lot.StripCount))),
            Priority = lot.Priority,
            CarrierType = lot.CarrierType,
            CarrierId = string.Empty,
            WaferLotId = lot.WaferLotId,
            IsPartialLot = true,
            MotherLotId = lotId,
            SplitReason = $"Rework: {reason}",
            SplitTime = DateTime.UtcNow,
            SplitQty = reworkQty,
        };

        await _lotRepo.AddAsync(reworkLot);

        // 减少母批次数量
        lot.UnitCount -= reworkQty;
        lot.TotalReworkQty += reworkQty;
        lot.UpdatedAt = DateTime.UtcNow;
        await _lotRepo.UpdateAsync(lot);

        // 创建重工记录
        var record = new ProdReworkRecord
        {
            ReworkId = Guid.NewGuid().ToString("N"),
            LotId = lotId,
            OriginalRouteId = lot.RouteId,
            ReworkRouteId = reworkRouteId,
            FromStepCode = fromStepCode,
            TargetStepCode = targetStepCode,
            ReworkQty = reworkQty,
            ReworkReason = reason,
            OperatorId = operatorId,
            ReworkCount = reworkLot.ReworkCount.Value,
            CreatedAt = DateTime.UtcNow,
        };

        await _reworkRepo.AddAsync(record);

        // 记录分批次关系
        var splitRecord = new ProdLotSplit
        {
            SplitId = Guid.NewGuid().ToString("N"),
            MotherLotId = lotId,
            ChildLotId = reworkLotId,
            SplitQty = reworkQty,
            SplitReason = $"Rework: {reason}",
            SplitType = "Rework",
            StepCode = fromStepCode,
            StepSeq = lot.CurrentStepSeq,
            OperatorId = operatorId,
            SplitTime = DateTime.UtcNow,
        };

        await _splitRepo.AddAsync(splitRecord);

        // 记录谱系
        await _genealogyService.RecordRelationAsync(new LotGenealogy
        {
            ParentLotId = lotId,
            ChildLotId = reworkLotId,
            RelationType = "Rework",
            StepCode = fromStepCode,
            Qty = reworkQty,
            OperatorId = operatorId,
            Remark = reason,
        });

        return MapToModel(record);
    }

    public async Task CompleteReworkAsync(string reworkLotId, string operatorId, string operatorName)
    {
        var reworkLot = await _lotRepo.GetByIdAsync(reworkLotId);
        if (reworkLot is null)
            throw new InvalidOperationException($"重工批次 {reworkLotId} 不存在");

        if (!reworkLot.IsReworkLot)
            throw new InvalidOperationException($"批次 {reworkLotId} 不是重工批次");

        // 更新重工批次状态
        reworkLot.Status = "Completed";
        reworkLot.TotalPassQty += reworkLot.UnitCount;
        reworkLot.UpdatedAt = DateTime.UtcNow;
        await _lotRepo.UpdateAsync(reworkLot);

        // 更新重工记录
        var motherLotId = reworkLot.MotherLotId ?? string.Empty;
        var records = await _reworkRepo.GetWhereAsync(r => r.LotId == motherLotId && !r.CompletedAt.HasValue);
        var record = records.FirstOrDefault();
        if (record is not null)
        {
            record.CompletedAt = DateTime.UtcNow;
            await _reworkRepo.UpdateAsync(record);
        }
    }

    public async Task<List<ReworkRecord>> GetReworkRecordsAsync(string lotId)
    {
        var records = await _reworkRepo.GetWhereAsync(r => r.LotId == lotId);
        return records.Select(MapToModel).ToList();
    }

    private static ReworkRecord MapToModel(ProdReworkRecord entity) => new()
    {
        ReworkId = entity.ReworkId,
        LotId = entity.LotId,
        OriginalRouteId = entity.OriginalRouteId,
        ReworkRouteId = entity.ReworkRouteId,
        FromStepCode = entity.FromStepCode,
        TargetStepCode = entity.TargetStepCode,
        ReworkQty = entity.ReworkQty,
        ReworkReason = entity.ReworkReason,
        OperatorId = entity.OperatorId,
        ReworkCount = entity.ReworkCount,
        CreatedAt = entity.CreatedAt,
        CompletedAt = entity.CompletedAt,
        ApprovedBy = entity.ApprovedBy,
        SignatureId = entity.SignatureId,
    };
}
