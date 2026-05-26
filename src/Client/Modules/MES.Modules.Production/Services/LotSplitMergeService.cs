using System.Text.Json;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class LotSplitMergeService : ILotSplitMergeService
{
    private readonly IRepository<ProdLot> _lotRepo;
    private readonly IRepository<ProdLotSplit> _splitRepo;
    private readonly IRepository<ProdLotMerge> _mergeRepo;
    private readonly IGenealogyService _genealogyService;

    public LotSplitMergeService(
        IRepository<ProdLot> lotRepo,
        IRepository<ProdLotSplit> splitRepo,
        IRepository<ProdLotMerge> mergeRepo,
        IGenealogyService genealogyService)
    {
        _lotRepo = lotRepo;
        _splitRepo = splitRepo;
        _mergeRepo = mergeRepo;
        _genealogyService = genealogyService;
    }

    public async Task<LotSplitRecord> SplitLotAsync(string motherLotId, int splitQty,
        string splitReason, string splitType, string operatorId, string operatorName)
    {
        var motherLot = await _lotRepo.GetByIdAsync(motherLotId);
        if (motherLot is null)
            throw new InvalidOperationException($"母批次 {motherLotId} 不存在");

        if (motherLot.UnitCount < splitQty)
            throw new InvalidOperationException($"拆分数量 {splitQty} 超过可用数量 {motherLot.UnitCount}");

        var childLotId = $"{motherLotId}-S{Guid.NewGuid().ToString("N")[..4]}";

        var childLot = new ProdLot
        {
            LotId = childLotId,
            OrderId = motherLot.OrderId,
            ProductId = motherLot.ProductId,
            ProductName = motherLot.ProductName,
            DieName = motherLot.DieName ?? string.Empty,
            PackageType = motherLot.PackageType,
            RouteId = motherLot.RouteId,
            RouteVersion = motherLot.RouteVersion,
            CurrentStepCode = motherLot.CurrentStepCode,
            CurrentStepSeq = motherLot.CurrentStepSeq,
            Status = "Waiting",
            OriginalQty = splitQty,
            UnitCount = splitQty,
            StripCount = Math.Max(1, splitQty / (motherLot.UnitCount / Math.Max(1, motherLot.StripCount))),
            Priority = motherLot.Priority,
            CarrierType = motherLot.CarrierType,
            CarrierId = string.Empty,
            WaferLotId = motherLot.WaferLotId,
            IsPartialLot = true,
            MotherLotId = motherLotId,
            SplitReason = splitReason,
            SplitTime = DateTime.UtcNow,
            SplitQty = splitQty,
        };

        await _lotRepo.AddAsync(childLot);

        motherLot.UnitCount -= splitQty;
        motherLot.UpdatedAt = DateTime.UtcNow;
        await _lotRepo.UpdateAsync(motherLot);

        var record = new ProdLotSplit
        {
            SplitId = Guid.NewGuid().ToString("N"),
            MotherLotId = motherLotId,
            ChildLotId = childLotId,
            SplitQty = splitQty,
            SplitReason = splitReason,
            SplitType = splitType,
            StepCode = motherLot.CurrentStepCode ?? string.Empty,
            StepSeq = motherLot.CurrentStepSeq,
            OperatorId = operatorId,
            SplitTime = DateTime.UtcNow,
        };

        await _splitRepo.AddAsync(record);

        await _genealogyService.RecordRelationAsync(new LotGenealogy
        {
            ParentLotId = motherLotId,
            ChildLotId = childLotId,
            RelationType = "Split",
            StepCode = motherLot.CurrentStepCode ?? string.Empty,
            StepSeq = motherLot.CurrentStepSeq,
            Qty = splitQty,
            OperatorId = operatorId,
            ReasonCode = splitType,
            Remark = splitReason,
        });

        return MapToSplitRecord(record);
    }

    public async Task<List<LotSplitRecord>> GradeSplitAsync(string lotId,
        Dictionary<string, int> gradeQtyMap, string operatorId, string operatorName)
    {
        var lot = await _lotRepo.GetByIdAsync(lotId);
        if (lot is null)
            throw new InvalidOperationException($"批次 {lotId} 不存在");

        var totalSplitQty = gradeQtyMap.Values.Sum();
        if (totalSplitQty > lot.UnitCount)
            throw new InvalidOperationException($"拆分总数量 {totalSplitQty} 超过可用数量 {lot.UnitCount}");

        var records = new List<LotSplitRecord>();

        foreach (var (grade, qty) in gradeQtyMap)
        {
            var childLotId = $"{lotId}-G{grade}";

            var childLot = new ProdLot
            {
                LotId = childLotId,
                OrderId = lot.OrderId,
                ProductId = lot.ProductId,
                ProductName = lot.ProductName,
                DieName = lot.DieName ?? string.Empty,
                PackageType = lot.PackageType,
                RouteId = lot.RouteId,
                RouteVersion = lot.RouteVersion,
                CurrentStepCode = lot.CurrentStepCode,
                CurrentStepSeq = lot.CurrentStepSeq,
                Status = "Waiting",
                Grade = grade,
                OriginalLotId = lotId,
                OriginalQty = qty,
                UnitCount = qty,
                StripCount = Math.Max(1, qty / (lot.UnitCount / Math.Max(1, lot.StripCount))),
                Priority = lot.Priority,
                CarrierType = lot.CarrierType,
                CarrierId = string.Empty,
                WaferLotId = lot.WaferLotId,
                IsPartialLot = true,
                MotherLotId = lotId,
                SplitReason = $"Grade Split: {grade}",
                SplitTime = DateTime.UtcNow,
                SplitQty = qty,
            };

            await _lotRepo.AddAsync(childLot);

            var record = new ProdLotSplit
            {
                SplitId = Guid.NewGuid().ToString("N"),
                MotherLotId = lotId,
                ChildLotId = childLotId,
                SplitQty = qty,
                SplitReason = $"Grade Split: {grade}",
                SplitType = "Grade",
                StepCode = lot.CurrentStepCode ?? string.Empty,
                StepSeq = lot.CurrentStepSeq,
                OperatorId = operatorId,
                SplitTime = DateTime.UtcNow,
            };

            await _splitRepo.AddAsync(record);

            await _genealogyService.RecordRelationAsync(new LotGenealogy
            {
                ParentLotId = lotId,
                ChildLotId = childLotId,
                RelationType = "GradeSplit",
                StepCode = lot.CurrentStepCode ?? string.Empty,
                StepSeq = lot.CurrentStepSeq,
                Qty = qty,
                Grade = grade,
                OperatorId = operatorId,
                Remark = $"Grade Split: {grade}",
            });

            records.Add(MapToSplitRecord(record));
        }

        lot.Status = "Completed";
        lot.UpdatedAt = DateTime.UtcNow;
        await _lotRepo.UpdateAsync(lot);

        return records;
    }

    public async Task<LotMergeRecord> MergeLotsAsync(string targetLotId, List<string> sourceLotIds,
        string mergeReason, string operatorId, string operatorName)
    {
        var targetLot = await _lotRepo.GetByIdAsync(targetLotId);
        if (targetLot is null)
            throw new InvalidOperationException($"目标批次 {targetLotId} 不存在");

        int totalMergedQty = 0;
        var mergeId = Guid.NewGuid().ToString("N");

        foreach (var sourceId in sourceLotIds)
        {
            var sourceLot = await _lotRepo.GetByIdAsync(sourceId);
            if (sourceLot is null)
                throw new InvalidOperationException($"源批次 {sourceId} 不存在");

            if (sourceLot.RouteId != targetLot.RouteId)
                throw new InvalidOperationException($"批次 {sourceId} 路线 {sourceLot.RouteId} 与目标批次 {targetLot.RouteId} 不一致");

            if (sourceLot.CurrentStepSeq != targetLot.CurrentStepSeq)
                throw new InvalidOperationException($"批次 {sourceId} 工序 {sourceLot.CurrentStepSeq} 与目标批次 {targetLot.CurrentStepSeq} 不一致");

            var sourceQty = sourceLot.UnitCount;
            totalMergedQty += sourceQty;

            sourceLot.Status = "Merged";
            sourceLot.UnitCount = 0;
            sourceLot.UpdatedAt = DateTime.UtcNow;
            await _lotRepo.UpdateAsync(sourceLot);

            var mergeRecord = new ProdLotMerge
            {
                MergeId = mergeId,
                TargetLotId = targetLotId,
                SourceLotId = sourceId,
                MergeQty = sourceQty,
                MergeReason = mergeReason,
                StepCode = targetLot.CurrentStepCode ?? string.Empty,
                StepSeq = targetLot.CurrentStepSeq,
                OperatorId = operatorId,
                MergeTime = DateTime.UtcNow,
            };

            await _mergeRepo.AddAsync(mergeRecord);

            await _genealogyService.RecordRelationAsync(new LotGenealogy
            {
                ParentLotId = sourceId,
                ChildLotId = targetLotId,
                RelationType = "Merge",
                StepCode = targetLot.CurrentStepCode ?? string.Empty,
                StepSeq = targetLot.CurrentStepSeq,
                Qty = sourceQty,
                OperatorId = operatorId,
                Remark = mergeReason,
            });
        }

        targetLot.UnitCount += totalMergedQty;
        targetLot.UpdatedAt = DateTime.UtcNow;
        await _lotRepo.UpdateAsync(targetLot);

        return new LotMergeRecord
        {
            MergeId = mergeId,
            TargetLotId = targetLotId,
            SourceLotIds = sourceLotIds,
            MergedQty = totalMergedQty,
            MergeReason = mergeReason,
            StepCode = targetLot.CurrentStepCode ?? string.Empty,
            StepSeq = targetLot.CurrentStepSeq,
            OperatorId = operatorId,
            MergeTime = DateTime.UtcNow,
        };
    }

    public async Task<List<LotInfo>> GetChildLotsAsync(string motherLotId)
    {
        var splits = await _splitRepo.GetWhereAsync(s => s.MotherLotId == motherLotId);
        var childLotIds = splits.Select(s => s.ChildLotId).ToList();

        var children = new List<LotInfo>();
        foreach (var childId in childLotIds)
        {
            var childLot = await _lotRepo.GetByIdAsync(childId);
            if (childLot is not null)
            {
                children.Add(MapToLotInfo(childLot));
            }
        }
        return children;
    }

    public async Task<List<LotSplitRecord>> GetSplitRecordsAsync(string lotId)
    {
        var records = await _splitRepo.GetWhereAsync(s => s.MotherLotId == lotId);
        return records.Select(MapToSplitRecord).ToList();
    }

    public async Task<List<LotMergeRecord>> GetMergeRecordsAsync(string lotId)
    {
        var records = await _mergeRepo.GetWhereAsync(m => m.TargetLotId == lotId);

        // Group by MergeId since each source lot creates a separate record
        var grouped = records.GroupBy(m => m.MergeId).Select(g =>
        {
            var first = g.First();
            return new LotMergeRecord
            {
                MergeId = g.Key,
                TargetLotId = first.TargetLotId,
                SourceLotIds = g.Select(m => m.SourceLotId).ToList(),
                MergedQty = g.Sum(m => m.MergeQty),
                MergeReason = first.MergeReason,
                StepCode = first.StepCode,
                StepSeq = first.StepSeq,
                OperatorId = first.OperatorId,
                MergeTime = first.MergeTime,
                ApprovedBy = first.ApprovedBy,
                SignatureId = first.SignatureId,
            };
        }).ToList();

        return grouped;
    }

    private static LotSplitRecord MapToSplitRecord(ProdLotSplit entity) => new()
    {
        SplitId = entity.SplitId,
        MotherLotId = entity.MotherLotId,
        ChildLotId = entity.ChildLotId,
        SplitQty = entity.SplitQty,
        SplitReason = entity.SplitReason,
        SplitType = entity.SplitType,
        StepCode = entity.StepCode,
        StepSeq = entity.StepSeq,
        OperatorId = entity.OperatorId,
        SplitTime = entity.SplitTime,
        ApprovedBy = entity.ApprovedBy,
        SignatureId = entity.SignatureId,
    };

    private static LotInfo MapToLotInfo(ProdLot entity) => new()
    {
        LotId = entity.LotId,
        OrderId = entity.OrderId,
        ProductId = entity.ProductId,
        ProductName = entity.ProductName,
        DieName = entity.DieName,
        PackageType = Enum.TryParse<MES.Domain.Production.PackageType>(entity.PackageType, true, out var pt) ? pt : MES.Domain.Production.PackageType.QFP,
        RouteId = entity.RouteId,
        RouteVersion = entity.RouteVersion,
        CurrentStep = entity.CurrentStepCode ?? string.Empty,
        CurrentStepSeq = entity.CurrentStepSeq,
        Status = entity.Status,
        UnitCount = entity.UnitCount,
        StripCount = entity.StripCount,
        Priority = entity.Priority,
        CarrierType = Enum.TryParse<MES.Domain.Production.CarrierType>(entity.CarrierType, true, out var ct) ? ct : MES.Domain.Production.CarrierType.Strip,
        CarrierId = entity.CarrierId ?? string.Empty,
        WaferLotId = entity.WaferLotId,
        OriginalQty = entity.OriginalQty,
        TotalPassQty = entity.TotalPassQty,
        TotalScrapQty = entity.TotalScrapQty,
        TotalReworkQty = entity.TotalReworkQty,
        IsPartialLot = entity.IsPartialLot,
        MotherLotId = entity.MotherLotId,
        SplitReason = entity.SplitReason,
        SplitTime = entity.SplitTime,
        SplitQty = entity.SplitQty,
        IsReworkLot = entity.IsReworkLot,
        ReworkCount = entity.ReworkCount,
        Grade = entity.Grade,
        OriginalLotId = entity.OriginalLotId,
    };
}
