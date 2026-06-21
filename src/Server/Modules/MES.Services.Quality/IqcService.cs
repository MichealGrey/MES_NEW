using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Quality;

public class IqcService : IIqcService
{
    private readonly MesDbContext _context;

    public IqcService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<IqcTaskResponse> CreateTaskAsync(CreateIqcTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MaterialId))
            throw new ApplicationException("物料ID不能为空");
        if (request.Quantity <= 0)
            throw new ApplicationException("数量必须大于0");

        var now = DateTime.UtcNow;
        var taskId = GenerateId("IQC");
        var batchId = GenerateId("IB");

        // Create incoming batch
        var batch = new IqcIncomingBatch
        {
            BatchId = batchId,
            PoNumber = request.PurchaseOrderNo,
            SupplierId = request.SupplierId,
            SupplierName = request.SupplierName,
            MaterialId = request.MaterialId,
            MaterialName = request.MaterialName,
            ReceivedQty = request.Quantity,
            Unit = request.Unit,
            LotNumber = request.SupplierBatchNo,
            ReceivedDate = now,
            Status = "PendingInspection",
            Remark = request.Remark,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<IqcIncomingBatch>().AddAsync(batch);

        // Create inspection task
        var task = new IqcInspectionTask
        {
            TaskId = taskId,
            BatchId = batchId,
            MaterialId = request.MaterialId,
            SupplierId = request.SupplierId,
            InspectionType = "Incoming",
            InspectionQty = request.Quantity,
            Status = "Pending",
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<IqcInspectionTask>().AddAsync(task);

        await _context.SaveChangesAsync();

        return MapToTaskResponse(task, batch);
    }

    public async Task<PagedResult<IqcTaskResponse>> GetTasksAsync(IqcTaskQuery query)
    {
        var iqQuery = _context.Set<IqcInspectionTask>().AsQueryable();
        var batchQuery = _context.Set<IqcIncomingBatch>().AsQueryable();

        if (!string.IsNullOrEmpty(query.Status))
            iqQuery = iqQuery.Where(t => t.Status == query.Status);
        if (!string.IsNullOrEmpty(query.MaterialId))
            iqQuery = iqQuery.Where(t => t.MaterialId == query.MaterialId);
        if (!string.IsNullOrEmpty(query.SupplierId))
            iqQuery = iqQuery.Where(t => t.SupplierId == query.SupplierId);
        if (query.DateFrom.HasValue)
            iqQuery = iqQuery.Where(t => t.CreatedAt >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iqQuery = iqQuery.Where(t => t.CreatedAt <= query.DateTo.Value);

        var totalCount = await iqQuery.CountAsync();

        var tasks = await iqQuery
            .OrderByDescending(t => t.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var batchIds = tasks.Select(t => t.BatchId).ToList();
        var batches = await batchQuery
            .Where(b => batchIds.Contains(b.BatchId))
            .ToDictionaryAsync(b => b.BatchId);

        var items = tasks.Select(t =>
        {
            var batch = batches.GetValueOrDefault(t.BatchId);
            return MapToTaskResponse(t, batch);
        }).ToList();

        return new PagedResult<IqcTaskResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<IqcTaskDetailResponse> GetTaskDetailAsync(string taskId)
    {
        var task = await _context.Set<IqcInspectionTask>().FindAsync(taskId);
        if (task == null)
            throw new ApplicationException($"检验任务 {taskId} 不存在");

        var batch = await _context.Set<IqcIncomingBatch>().FindAsync(task.BatchId);
        var results = await _context.Set<IqcInspectionResult>()
            .Where(r => r.TaskId == taskId)
            .ToListAsync();

        var response = new IqcTaskDetailResponse
        {
            TaskId = task.TaskId,
            BatchId = task.BatchId,
            MaterialId = task.MaterialId,
            MaterialName = batch?.MaterialName ?? string.Empty,
            SupplierId = task.SupplierId,
            SupplierName = batch?.SupplierName ?? string.Empty,
            SupplierBatchNo = batch?.LotNumber ?? string.Empty,
            Quantity = batch?.ReceivedQty ?? 0,
            Status = task.Status,
            InspectionStatus = results.Count > 0 ? "Inspected" : "NotInspected",
            JudgmentResult = task.Status == "Completed" ? GetJudgmentFromResults(results) : null,
            CreatedAt = task.CreatedAt,
            Items = results.Select(r => new InspectionItemInput
            {
                ItemCode = r.StandardId,
                ItemName = r.InspectionItem,
                StandardValue = r.StandardValue,
                ActualValue = r.ActualValue,
                LowerLimit = decimal.TryParse(r.LowerLimit, out var ll) ? ll : null,
                UpperLimit = decimal.TryParse(r.UpperLimit, out var ul) ? ul : null,
                Result = r.Judgment,
                Unit = r.Unit,
                MeasuringEquipment = r.Remark
            }).ToList()
        };

        return response;
    }

    public async Task<InspectionResultResponse> ExecuteInspectionAsync(string taskId, ExecuteInspectionRequest request)
    {
        var task = await _context.Set<IqcInspectionTask>().FindAsync(taskId);
        if (task == null)
            throw new ApplicationException($"检验任务 {taskId} 不存在");
        if (task.Status != "Pending" && task.Status != "Inspecting")
            throw new ApplicationException($"任务状态为 {task.Status}，无法执行检验");

        var now = DateTime.UtcNow;
        var resultIds = new List<string>();

        foreach (var item in request.Items)
        {
            var resultId = GenerateId("IIR");
            var result = new IqcInspectionResult
            {
                ResultId = resultId,
                TaskId = taskId,
                BatchId = task.BatchId,
                StandardId = item.ItemCode,
                InspectionItem = item.ItemName,
                StandardValue = item.StandardValue,
                UpperLimit = item.UpperLimit?.ToString(),
                LowerLimit = item.LowerLimit?.ToString(),
                ActualValue = item.ActualValue,
                Judgment = item.Result,
                Unit = item.Unit,
                InspectorId = request.InspectorId,
                InspectorName = request.InspectorName,
                InspectionTime = now,
                Remark = item.MeasuringEquipment,
                CreatedAt = now
            };
            await _context.Set<IqcInspectionResult>().AddAsync(result);
            resultIds.Add(resultId);
        }

        // Update task status to Inspecting
        task.Status = "Inspecting";
        task.InspectorId = request.InspectorId;
        task.InspectorName = request.InspectorName;
        task.UpdatedAt = now;

        await _context.SaveChangesAsync();

        var passCount = request.Items.Count(i => i.Result == "Pass");
        var failCount = request.Items.Count(i => i.Result == "Fail");

        return new InspectionResultResponse
        {
            TaskId = taskId,
            ResultIds = resultIds,
            IsComplete = true,
            ItemCount = request.Items.Count,
            PassCount = passCount,
            FailCount = failCount
        };
    }

    public async Task<IqcTaskResponse> JudgeAsync(string taskId, JudgeRequest request)
    {
        var task = await _context.Set<IqcInspectionTask>().FindAsync(taskId);
        if (task == null)
            throw new ApplicationException($"检验任务 {taskId} 不存在");
        if (task.Status != "Inspecting")
            throw new ApplicationException($"任务状态为 {task.Status}，无法判定");

        var validJudgments = new[] { "Pass", "Fail", "ConditionalPass" };
        if (!validJudgments.Contains(request.Judgment))
            throw new ApplicationException($"无效的判定结果: {request.Judgment}");

        var batch = await _context.Set<IqcIncomingBatch>().FindAsync(task.BatchId);
        var now = DateTime.UtcNow;

        // Update task
        task.Status = "Completed";
        task.CompletedTime = now;
        task.Remark = request.JudgeComment;
        task.UpdatedAt = now;

        // Update batch status
        batch.Status = request.Judgment == "Pass" ? "Passed" : request.Judgment == "Fail" ? "Failed" : "ConditionalPass";
        batch.UpdatedAt = now;

        // If Failed, auto-create NCR
        string? ncrId = null;
        if (request.Judgment == "Fail")
        {
            ncrId = GenerateId("NCR");
            var ncr = new NonconformingRecord
            {
                NcrId = ncrId,
                LotId = task.BatchId,
                ProductId = task.MaterialId,
                ProductName = batch?.MaterialName,
                SourceType = "IQC",
                DefectQty = batch?.ReceivedQty ?? 0,
                DefectDescription = $"IQC检验判定不合格 - {request.JudgeComment}",
                Severity = "Major",
                Status = "Open",
                ReportTime = now,
                CreatedAt = now,
                UpdatedAt = now
            };
            await _context.Set<NonconformingRecord>().AddAsync(ncr);
        }

        await _context.SaveChangesAsync();

        return MapToTaskResponse(task, batch);
    }

    public async Task<bool> IsolateBatchAsync(string batchId, string operatorId)
    {
        var batch = await _context.Set<IqcIncomingBatch>().FindAsync(batchId);
        if (batch == null)
            throw new ApplicationException($"批次 {batchId} 不存在");

        batch.Status = "Frozen";
        batch.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReleaseBatchAsync(string batchId, string operatorId)
    {
        var batch = await _context.Set<IqcIncomingBatch>().FindAsync(batchId);
        if (batch == null)
            throw new ApplicationException($"批次 {batchId} 不存在");

        batch.Status = batch.Status == "Frozen" ? "PendingInspection" : "Passed";
        batch.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IqcStatisticsResponse> GetStatisticsAsync(IqcStatisticsQuery query)
    {
        var batches = _context.Set<IqcIncomingBatch>().AsQueryable();

        if (!string.IsNullOrEmpty(query.SupplierId))
            batches = batches.Where(b => b.SupplierId == query.SupplierId);
        if (!string.IsNullOrEmpty(query.MaterialId))
            batches = batches.Where(b => b.MaterialId == query.MaterialId);
        if (query.DateFrom.HasValue)
            batches = batches.Where(b => b.ReceivedDate >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            batches = batches.Where(b => b.ReceivedDate <= query.DateTo.Value);

        var allBatches = await batches.ToListAsync();

        var totalBatches = allBatches.Count;
        var passedBatches = allBatches.Count(b => b.Status == "Passed");
        var failedBatches = allBatches.Count(b => b.Status == "Failed");
        var passRate = totalBatches > 0 ? Math.Round((decimal)passedBatches / totalBatches * 100, 2) : 0m;

        var supplierStats = allBatches
            .GroupBy(b => b.SupplierId)
            .Select(g => new SupplierStatItem
            {
                SupplierId = g.Key,
                SupplierName = g.First().SupplierName,
                TotalBatches = g.Count(),
                PassedBatches = g.Count(b => b.Status == "Passed"),
                PassRate = g.Count() > 0 ? Math.Round((decimal)g.Count(b => b.Status == "Passed") / g.Count() * 100, 2) : 0m
            })
            .OrderByDescending(s => s.TotalBatches)
            .ToList();

        return new IqcStatisticsResponse
        {
            TotalBatches = totalBatches,
            PassedBatches = passedBatches,
            FailedBatches = failedBatches,
            PassRate = passRate,
            SupplierStats = supplierStats
        };
    }

    private static IqcTaskResponse MapToTaskResponse(IqcInspectionTask task, IqcIncomingBatch? batch)
    {
        return new IqcTaskResponse
        {
            TaskId = task.TaskId,
            BatchId = task.BatchId,
            MaterialId = task.MaterialId,
            MaterialName = batch?.MaterialName ?? string.Empty,
            SupplierId = task.SupplierId,
            SupplierName = batch?.SupplierName ?? string.Empty,
            SupplierBatchNo = batch?.LotNumber ?? string.Empty,
            Quantity = batch?.ReceivedQty ?? 0,
            Status = task.Status,
            InspectionStatus = task.Status == "Completed" ? "Inspected" : "NotInspected",
            JudgmentResult = task.Status == "Completed" ? (batch?.Status == "Passed" ? "Pass" : batch?.Status == "Failed" ? "Fail" : null) : null,
            CreatedAt = task.CreatedAt
        };
    }

    private static string GetJudgmentFromResults(List<IqcInspectionResult> results)
    {
        if (results.Count == 0) return null;
        var failCount = results.Count(r => r.Judgment == "Fail");
        return failCount == 0 ? "Pass" : failCount == results.Count ? "Fail" : "ConditionalPass";
    }

    private static string GenerateId(string prefix)
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 9999).ToString("D4");
        return $"{prefix}-{now:yyyyMMdd}-{seq}";
    }
}
