using MES.Contracts.Common;
using MES.Contracts.Production;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Production;

public interface ILotService
{
    Task<PagedResult<LotDto>> GetPagedAsync(int pageIndex, int pageSize, string? status = null, string? orderId = null, string? keyword = null);
    Task<LotDto?> GetByIdAsync(string lotId);
    Task<List<LotDto>> GetByOrderIdAsync(string orderId);
    Task<List<LotStepDto>> GetLotStepsAsync(string lotId);
    Task<int> GetDashboardStatsAsync();
    Task<LotDetailResponse?> GetLotDetailAsync(string lotId);
    Task<List<LotTrackRecord>> GetLotTrackingAsync(string lotId);
    Task<LotStatsResponse> GetLotStatsAsync();
}

public class LotService : ILotService
{
    private readonly MesDbContext _context;

    public LotService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<LotDto>> GetPagedAsync(int pageIndex, int pageSize, string? status = null, string? orderId = null, string? keyword = null)
    {
        var query = _context.ProdLots.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);

        if (!string.IsNullOrEmpty(orderId))
            query = query.Where(x => x.OrderId == orderId);

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(x => x.LotId.Contains(keyword) || x.ProductName.Contains(keyword));

        query = query.OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new LotDto
            {
                LotId = x.LotId,
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                DieName = x.DieName,
                PackageType = x.PackageType,
                RouteId = x.RouteId,
                RouteVersion = x.RouteVersion,
                CurrentStepSeq = x.CurrentStepSeq,
                CurrentStepCode = x.CurrentStepCode,
                Status = x.Status,
                ProcessStage = x.ProcessStage,
                UnitCount = x.UnitCount,
                StripCount = x.StripCount,
                Priority = x.Priority,
                OriginalQty = x.OriginalQty,
                TotalPassQty = x.TotalPassQty,
                TotalScrapQty = x.TotalScrapQty,
                TotalHoldQty = x.TotalHoldQty,
                QtyPass = x.QtyPass,
                QtyFail = x.QtyFail,
                WaferLotId = x.WaferLotId,
                CarrierType = x.CarrierType,
                CarrierId = x.CarrierId,
                Grade = x.Grade,
                BinResult = x.BinResult,
                TestResult = x.TestResult,
                IsReworkLot = x.IsReworkLot,
                IsUnderMrb = x.IsUnderMrb,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        return new PagedResult<LotDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<LotDto?> GetByIdAsync(string lotId)
    {
        var lot = await _context.ProdLots.FindAsync(lotId);
        if (lot == null) return null;

        return MapToDto(lot);
    }

    public async Task<List<LotDto>> GetByOrderIdAsync(string orderId)
    {
        var items = await _context.ProdLots
            .Where(x => x.OrderId == orderId)
            .OrderBy(x => x.LotId)
            .ToListAsync();

        return items.Select(MapToDto).ToList();
    }

    public async Task<List<LotStepDto>> GetLotStepsAsync(string lotId)
    {
        var steps = await _context.ProdLotSteps
            .Where(x => x.LotId == lotId)
            .OrderBy(x => x.StepSeq)
            .ToListAsync();

        return steps.Select(x => new LotStepDto
        {
            RecordId = x.RecordId,
            LotId = x.LotId,
            RouteId = x.RouteId,
            StepSeq = x.StepSeq,
            StepCode = x.StepCode,
            StepName = x.StepName,
            Status = x.Status,
            TrackInEquipment = x.TrackInEquipment,
            TrackInRecipe = x.TrackInRecipe,
            TrackInTime = x.TrackInTime,
            TrackInOperator = x.TrackInOperator,
            TrackOutTime = x.TrackOutTime,
            TrackOutOperator = x.TrackOutOperator,
            InputQty = x.InputQty,
            PassQty = x.PassQty,
            FailQty = x.FailQty,
            ScrapQty = x.ScrapQty,
            ReworkQty = x.ReworkQty,
            HoldQty = x.HoldQty,
            PendingQty = x.PendingQty,
            Remark = x.Remark,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task<int> GetDashboardStatsAsync()
    {
        return await _context.ProdLots.CountAsync(x => x.Status == "InProduction");
    }

    public async Task<LotDetailResponse?> GetLotDetailAsync(string lotId)
    {
        var lot = await _context.ProdLots.FindAsync(lotId);
        if (lot == null) return null;

        var stepRecords = await _context.ProdLotSteps
            .Where(x => x.LotId == lotId)
            .OrderBy(x => x.StepSeq)
            .ToListAsync();

        var operationRecords = await _context.ProdOperationHistories
            .Where(x => x.LotId == lotId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return new LotDetailResponse
        {
            Lot = MapToDto(lot),
            StepHistory = stepRecords.Select(x => new LotStepHistory
            {
                RecordId = x.RecordId,
                StepSeq = x.StepSeq,
                StepCode = x.StepCode,
                StepName = x.StepName,
                Status = x.Status,
                TrackInEquipment = x.TrackInEquipment,
                TrackInCarrier = x.TrackInCarrier,
                TrackInRecipe = x.TrackInRecipe,
                TrackInTime = x.TrackInTime,
                TrackInOperator = x.TrackInOperator,
                TrackOutTime = x.TrackOutTime,
                TrackOutOperator = x.TrackOutOperator,
                InputQty = x.InputQty,
                PassQty = x.PassQty,
                FailQty = x.FailQty,
                ScrapQty = x.ScrapQty,
                ReworkQty = x.ReworkQty,
                HoldQty = x.HoldQty,
                PendingQty = x.PendingQty,
                Remark = x.Remark,
                CreatedAt = x.CreatedAt
            }).ToList(),
            TrackRecords = operationRecords.Select(x => new LotTrackRecord
            {
                OperationId = x.OperationId,
                OperationType = x.OperationType,
                StepCode = x.StepCode,
                StepSeq = x.StepSeq,
                EquipmentId = x.EquipmentId,
                CarrierId = x.CarrierId,
                RecipeId = x.RecipeId,
                OperatorId = x.OperatorId,
                OperatorName = x.OperatorName,
                InputQty = x.InputQty,
                OutputQty = x.OutputQty,
                ScrapQty = x.ScrapQty,
                Detail = x.Detail,
                Remark = x.Remark,
                CreatedAt = x.CreatedAt
            }).ToList()
        };
    }

    public async Task<List<LotTrackRecord>> GetLotTrackingAsync(string lotId)
    {
        var records = await _context.ProdOperationHistories
            .Where(x => x.LotId == lotId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return records.Select(x => new LotTrackRecord
        {
            OperationId = x.OperationId,
            OperationType = x.OperationType,
            StepCode = x.StepCode,
            StepSeq = x.StepSeq,
            EquipmentId = x.EquipmentId,
            CarrierId = x.CarrierId,
            RecipeId = x.RecipeId,
            OperatorId = x.OperatorId,
            OperatorName = x.OperatorName,
            InputQty = x.InputQty,
            OutputQty = x.OutputQty,
            ScrapQty = x.ScrapQty,
            Detail = x.Detail,
            Remark = x.Remark,
            CreatedAt = x.CreatedAt
        }).ToList();
    }

    public async Task<LotStatsResponse> GetLotStatsAsync()
    {
        var stats = await _context.ProdLots
            .GroupBy(x => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Waiting = g.Count(x => x.Status == "Waiting"),
                InProduction = g.Count(x => x.Status == "InProduction"),
                Hold = g.Count(x => x.Status == "Hold"),
                Completed = g.Count(x => x.Status == "Completed"),
                Scraped = g.Count(x => x.Status == "Scraped")
            })
            .FirstOrDefaultAsync();

        return new LotStatsResponse
        {
            TotalLots = stats?.Total ?? 0,
            WaitingLots = stats?.Waiting ?? 0,
            InProductionLots = stats?.InProduction ?? 0,
            HoldLots = stats?.Hold ?? 0,
            CompletedLots = stats?.Completed ?? 0,
            ScrapedLots = stats?.Scraped ?? 0
        };
    }

    private static LotDto MapToDto(ProdLot lot) => new()
    {
        LotId = lot.LotId,
        OrderId = lot.OrderId,
        ProductId = lot.ProductId,
        ProductName = lot.ProductName,
        DieName = lot.DieName,
        PackageType = lot.PackageType,
        RouteId = lot.RouteId,
        RouteVersion = lot.RouteVersion,
        CurrentStepSeq = lot.CurrentStepSeq,
        CurrentStepCode = lot.CurrentStepCode,
        Status = lot.Status,
        ProcessStage = lot.ProcessStage,
        UnitCount = lot.UnitCount,
        StripCount = lot.StripCount,
        Priority = lot.Priority,
        OriginalQty = lot.OriginalQty,
        TotalPassQty = lot.TotalPassQty,
        TotalScrapQty = lot.TotalScrapQty,
        TotalHoldQty = lot.TotalHoldQty,
        QtyPass = lot.QtyPass,
        QtyFail = lot.QtyFail,
        WaferLotId = lot.WaferLotId,
        CarrierType = lot.CarrierType,
        CarrierId = lot.CarrierId,
        Grade = lot.Grade,
        BinResult = lot.BinResult,
        TestResult = lot.TestResult,
        IsReworkLot = lot.IsReworkLot,
        IsUnderMrb = lot.IsUnderMrb,
        CreatedAt = lot.CreatedAt,
        UpdatedAt = lot.UpdatedAt
    };
}
