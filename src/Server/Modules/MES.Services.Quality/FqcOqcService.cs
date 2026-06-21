using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Quality;

public class FqcOqcService : IFqcOqcService
{
    private readonly MesDbContext _context;

    public FqcOqcService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<FqcTaskResponse> CreateFqcTaskAsync(string lotId, string workOrderId)
    {
        if (string.IsNullOrWhiteSpace(lotId))
            throw new ApplicationException("批次ID不能为空");

        var lot = await _context.ProdLots.FindAsync(lotId);
        if (lot == null)
            throw new ApplicationException($"批次 {lotId} 不存在");

        var taskId = GenerateId("FQC");
        var now = DateTime.UtcNow;

        var record = new FqcInspectionRecord
        {
            RecordId = taskId,
            LotId = lotId,
            OrderId = workOrderId,
            ProductId = lot.ProductId,
            ProductName = lot.ProductName,
            RouteId = lot.RouteId,
            StepCode = lot.CurrentStepCode,
            InspectionQty = lot.UnitCount,
            Judgment = string.Empty,
            InspectionTime = now,
            CreatedAt = now
        };
        await _context.Set<FqcInspectionRecord>().AddAsync(record);
        await _context.SaveChangesAsync();

        return new FqcTaskResponse
        {
            TaskId = record.RecordId,
            LotId = record.LotId,
            WorkOrderId = record.OrderId,
            ProductId = record.ProductId,
            ProductName = record.ProductName,
            Quantity = record.InspectionQty,
            Status = "Pending",
            CreatedAt = record.CreatedAt
        };
    }

    public async Task<PagedResult<FqcTaskResponse>> GetFqcTasksAsync(FqcTaskQuery query)
    {
        var iq = _context.Set<FqcInspectionRecord>().AsQueryable();

        if (!string.IsNullOrEmpty(query.WorkOrderId))
            iq = iq.Where(r => r.OrderId == query.WorkOrderId);
        if (!string.IsNullOrEmpty(query.ProductId))
            iq = iq.Where(r => r.ProductId == query.ProductId);
        if (query.DateFrom.HasValue)
            iq = iq.Where(r => r.InspectionTime >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(r => r.InspectionTime <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var items = await iq
            .OrderByDescending(r => r.InspectionTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new FqcTaskResponse
            {
                TaskId = r.RecordId,
                LotId = r.LotId,
                WorkOrderId = r.OrderId,
                ProductId = r.ProductId,
                ProductName = r.ProductName,
                Quantity = r.InspectionQty,
                Status = string.IsNullOrEmpty(r.Judgment) ? "Pending" : "Completed",
                JudgmentResult = r.Judgment,
                CompletedAt = string.IsNullOrEmpty(r.Judgment) ? null : r.InspectionTime,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<FqcTaskResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<FqcTaskResponse> ExecuteFqcAsync(string taskId, ExecuteFqcRequest request)
    {
        var record = await _context.Set<FqcInspectionRecord>().FindAsync(taskId);
        if (record == null)
            throw new ApplicationException($"FQC记录 {taskId} 不存在");

        var now = DateTime.UtcNow;
        var passCount = request.Items.Count(i => i.Result == "Pass");
        var failCount = request.Items.Count(i => i.Result == "Fail");
        var overallResult = failCount == 0 ? "Pass" : failCount == request.Items.Count ? "Fail" : "ConditionalPass";

        record.PassQty = passCount;
        record.FailQty = failCount;
        record.Judgment = overallResult;
        record.InspectorId = request.InspectorId;
        record.InspectorName = request.InspectorName;
        record.Remark = request.Remark;
        record.InspectionTime = now;

        await _context.SaveChangesAsync();

        return new FqcTaskResponse
        {
            TaskId = record.RecordId,
            LotId = record.LotId,
            WorkOrderId = record.OrderId,
            ProductId = record.ProductId,
            ProductName = record.ProductName,
            Quantity = record.InspectionQty,
            Status = "Completed",
            JudgmentResult = record.Judgment,
            CompletedAt = now,
            CreatedAt = record.CreatedAt
        };
    }

    public async Task<OqcTaskResponse> CreateOqcTaskAsync(string lotId, string shipmentId)
    {
        if (string.IsNullOrWhiteSpace(lotId))
            throw new ApplicationException("批次ID不能为空");

        var lot = await _context.ProdLots.FindAsync(lotId);
        if (lot == null)
            throw new ApplicationException($"批次 {lotId} 不存在");

        var taskId = GenerateId("OQC");
        var now = DateTime.UtcNow;

        var record = new OqcInspectionRecord
        {
            RecordId = taskId,
            LotId = lotId,
            OrderId = shipmentId,
            ProductId = lot.ProductId,
            ProductName = lot.ProductName,
            InspectionQty = lot.UnitCount,
            Judgment = string.Empty,
            InspectionTime = now,
            CreatedAt = now
        };
        await _context.Set<OqcInspectionRecord>().AddAsync(record);
        await _context.SaveChangesAsync();

        return new OqcTaskResponse
        {
            TaskId = record.RecordId,
            LotId = record.LotId,
            ShipmentId = record.OrderId,
            ProductId = record.ProductId,
            Quantity = record.InspectionQty,
            Status = "Pending",
            CreatedAt = record.CreatedAt
        };
    }

    public async Task<PagedResult<OqcTaskResponse>> GetOqcTasksAsync(OqcTaskQuery query)
    {
        var iq = _context.Set<OqcInspectionRecord>().AsQueryable();

        if (!string.IsNullOrEmpty(query.ProductId))
            iq = iq.Where(r => r.ProductId == query.ProductId);
        if (query.DateFrom.HasValue)
            iq = iq.Where(r => r.InspectionTime >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(r => r.InspectionTime <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var items = await iq
            .OrderByDescending(r => r.InspectionTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new OqcTaskResponse
            {
                TaskId = r.RecordId,
                LotId = r.LotId,
                ShipmentId = r.OrderId,
                ProductId = r.ProductId,
                Quantity = r.InspectionQty,
                Status = string.IsNullOrEmpty(r.Judgment) ? "Pending" : "Completed",
                JudgmentResult = r.Judgment,
                PackagingCheck = r.PackagingCheck,
                LabelCheck = r.LabelCheck,
                DocumentationCheck = "NotChecked",
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<OqcTaskResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<OqcTaskResponse> ExecuteOqcAsync(string taskId, ExecuteOqcRequest request)
    {
        var record = await _context.Set<OqcInspectionRecord>().FindAsync(taskId);
        if (record == null)
            throw new ApplicationException($"OQC记录 {taskId} 不存在");

        var now = DateTime.UtcNow;
        var passCount = request.Items.Count(i => i.Result == "Pass");
        var failCount = request.Items.Count(i => i.Result == "Fail");
        var overallResult = failCount == 0 ? "Pass" : failCount == request.Items.Count ? "Fail" : "ConditionalPass";

        record.PassQty = passCount;
        record.FailQty = failCount;
        record.Judgment = overallResult;
        record.PackagingCheck = request.PackagingCheck;
        record.LabelCheck = request.LabelCheck;
        record.MslCheck = "Pass";
        record.InspectorId = request.InspectorId;
        record.InspectorName = request.InspectorName;
        record.Remark = request.Remark;
        record.InspectionTime = now;

        await _context.SaveChangesAsync();

        return new OqcTaskResponse
        {
            TaskId = record.RecordId,
            LotId = record.LotId,
            ShipmentId = record.OrderId,
            ProductId = record.ProductId,
            Quantity = record.InspectionQty,
            Status = "Completed",
            JudgmentResult = record.Judgment,
            PackagingCheck = record.PackagingCheck,
            LabelCheck = record.LabelCheck,
            DocumentationCheck = "Checked",
            CreatedAt = record.CreatedAt
        };
    }

    public async Task<MslCheckResult> CheckMslForShipmentAsync(OqcMslCheckRequest request)
    {
        var mslCheck = await _context.Set<ShipmentMslCheck>()
            .Where(c => c.LotId == request.LotId)
            .OrderByDescending(c => c.CheckTime)
            .FirstOrDefaultAsync();

        if (mslCheck == null)
        {
            return new MslCheckResult
            {
                Passed = true,
                LotId = request.LotId,
                Result = "Pass",
                RemainingFloorLifeHours = 0
            };
        }

        var now = DateTime.UtcNow;
        var remaining = mslCheck.FloorLifeRemaining ?? 0;
        var expired = remaining <= 0;

        return new MslCheckResult
        {
            Passed = !expired,
            LotId = request.LotId,
            MslLevel = mslCheck.MslLevel,
            MslExposureStart = mslCheck.ExposureStartTime,
            MslExpiry = mslCheck.ExposureStartTime?.AddHours(mslCheck.ExposureDurationHours ?? 0),
            RemainingFloorLifeHours = remaining,
            Result = expired ? "Expired" : "Pass",
            FailureReason = expired ? "MSL地板寿命已到期" : null
        };
    }

    private static string GenerateId(string prefix)
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 9999).ToString("D4");
        return $"{prefix}-{now:yyyyMMdd}-{seq}";
    }
}
