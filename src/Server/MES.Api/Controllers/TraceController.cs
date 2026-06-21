using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MES.Contracts.Common;
using MES.Infrastructure.Persistence;

namespace MES.Api.Controllers;

/// <summary>
/// 追溯查询 API 控制器 - 提供批次正向追溯、反向追溯、谱系树、影响分析和客户报告。
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TraceController : ControllerBase
{
    private readonly MesDbContext _dbContext;
    private readonly ILogger<TraceController> _logger;

    public TraceController(MesDbContext dbContext, ILogger<TraceController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    // ============================================================================
    // DTO Records
    // ============================================================================

    public record LotTraceDto(
        string Id,
        string LotId,
        string Product,
        string CurrentStep,
        string Status,
        DateTime CreatedTime,
        DateTime? CompletedTime,
        string Customer,
        string WorkOrderNo);

    public record ForwardTraceDto(
        string StepNo,
        string Operation,
        string EquipmentId,
        DateTime StartTime,
        DateTime EndTime,
        string Operator,
        string RecipeId,
        string Result);

    public record BackwardTraceDto(
        string LotId,
        string WaferId,
        string Supplier,
        string MaterialLot,
        DateTime ReceiveDate,
        string InspectionResult);

    public record GenealogyDto(
        string Id,
        string LotId,
        string ParentLotId,
        string ChildLotId,
        string Product,
        string Operation,
        DateTime Time,
        string EquipmentId,
        string Operator);

    public record ImpactAnalysisDto(
        string AffectedLotId,
        string RootCause,
        string ImpactType,
        int AffectedQty,
        string RecommendedAction,
        DateTime AnalysisDate);

    public record CustomerTraceReportDto(
        string ReportId,
        string LotId,
        string CustomerName,
        DateTime ReportDate,
        string ProductName,
        int TotalQty,
        string TraceResult);

    // ============================================================================
    // GET /api/trace/lots - Get all lot traces
    // ============================================================================

    /// <summary>
    /// 获取所有批次追溯列表（含产品信息、客户信息、当前工序）。
    /// GET /api/trace/lots
    /// </summary>
    [HttpGet("lots")]
    public async Task<ApiResponse<List<LotTraceDto>>> GetLotTraces()
    {
        var query = from lot in _dbContext.ProdLots
                    join product in _dbContext.MasterProducts on lot.ProductId equals product.ProductId into pp
                    from product in pp.DefaultIfEmpty()
                    join customer in _dbContext.MasterCustomers on product.CustomerId equals customer.CustomerId into cp
                    from customer in cp.DefaultIfEmpty()
                    select new LotTraceDto(
                        lot.LotId,
                        lot.LotId,
                        lot.ProductName ?? string.Empty,
                        lot.CurrentStepCode ?? string.Empty,
                        lot.Status ?? string.Empty,
                        lot.CreatedAt,
                        null,
                        customer.CustomerName ?? string.Empty,
                        lot.OrderId ?? string.Empty);

        var result = await query.OrderByDescending(x => x.CreatedTime).ToListAsync();
        return ApiResponse<List<LotTraceDto>>.Ok(result);
    }

    // ============================================================================
    // GET /api/trace/forward/{lotId} - Get forward trace operations
    // ============================================================================

    /// <summary>
    /// 获取指定批次的正向追溯信息（工序步骤 + 操作历史）。
    /// GET /api/trace/forward/{lotId}
    /// </summary>
    [HttpGet("forward/{lotId}")]
    public async Task<ApiResponse<List<ForwardTraceDto>>> GetForwardTrace(string lotId)
    {
        if (string.IsNullOrEmpty(lotId))
            return ApiResponse<List<ForwardTraceDto>>.Fail("批次号不能为空");

        var steps = await _dbContext.ProdLotSteps
            .Where(s => s.LotId == lotId)
            .OrderBy(s => s.StepSeq)
            .ToListAsync();

        var operations = await _dbContext.ProdOperationHistories
            .Where(o => o.LotId == lotId)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();

        var result = new List<ForwardTraceDto>();

        foreach (var step in steps)
        {
            var matchingOp = operations.FirstOrDefault(o => o.StepCode == step.StepCode && o.StepSeq == step.StepSeq);

            result.Add(new ForwardTraceDto(
                step.StepSeq.ToString(),
                step.StepName ?? string.Empty,
                step.TrackInEquipment ?? string.Empty,
                step.TrackInTime ?? DateTime.MinValue,
                step.TrackOutTime ?? DateTime.MinValue,
                step.TrackInOperator ?? string.Empty,
                step.RecipeId ?? string.Empty,
                step.Status ?? string.Empty));
        }

        // Add operations that don't have matching steps
        foreach (var op in operations)
        {
            if (!steps.Any(s => s.StepCode == op.StepCode && s.StepSeq == op.StepSeq))
            {
                result.Add(new ForwardTraceDto(
                    op.StepSeq.ToString(),
                    op.OperationType,
                    op.EquipmentId ?? string.Empty,
                    op.CreatedAt,
                    op.CreatedAt,
                    op.OperatorName ?? string.Empty,
                    op.RecipeId ?? string.Empty,
                    "Completed"));
            }
        }

        return ApiResponse<List<ForwardTraceDto>>.Ok(result);
    }

    // ============================================================================
    // GET /api/trace/backward/{lotId} - Get backward trace parent lots
    // ============================================================================

    /// <summary>
    /// 获取指定批次的反向追溯信息（父批次链，递归查找祖先）。
    /// GET /api/trace/backward/{lotId}
    /// </summary>
    [HttpGet("backward/{lotId}")]
    public async Task<ApiResponse<List<BackwardTraceDto>>> GetBackwardTrace(string lotId)
    {
        if (string.IsNullOrEmpty(lotId))
            return ApiResponse<List<BackwardTraceDto>>.Fail("批次号不能为空");

        var result = new List<BackwardTraceDto>();
        var visited = new HashSet<string>();
        var queue = new Queue<string>();

        queue.Enqueue(lotId);

        while (queue.Count > 0)
        {
            var currentLotId = queue.Dequeue();
            if (!visited.Add(currentLotId)) continue;

            var parentChains = await _dbContext.LotTraceChains
                .Where(c => c.ChildLotId == currentLotId)
                .ToListAsync();

            foreach (var chain in parentChains)
            {
                var parentLot = await _dbContext.ProdLots
                    .FirstOrDefaultAsync(l => l.LotId == chain.ParentLotId);

                var materialConsume = await _dbContext.MaterialConsumes
                    .Where(mc => mc.LotId == chain.ParentLotId)
                    .OrderByDescending(mc => mc.ConsumedAt)
                    .FirstOrDefaultAsync();

                result.Add(new BackwardTraceDto(
                    chain.ParentLotId,
                    parentLot?.WaferLotId ?? string.Empty,
                    materialConsume?.MaterialName ?? string.Empty,
                    parentLot?.LotId ?? string.Empty,
                    parentLot?.CreatedAt ?? DateTime.MinValue,
                    parentLot?.Status == "Completed" ? "Pass" : "Pending"));

                if (!visited.Contains(chain.ParentLotId))
                {
                    queue.Enqueue(chain.ParentLotId);
                }
            }
        }

        return ApiResponse<List<BackwardTraceDto>>.Ok(result);
    }

    // ============================================================================
    // GET /api/trace/genealogy/{lotId} - Get genealogy for a lot
    // ============================================================================

    /// <summary>
    /// 获取指定批次的谱系关系（父子关系链）。
    /// GET /api/trace/genealogy/{lotId}
    /// </summary>
    [HttpGet("genealogy/{lotId}")]
    public async Task<ApiResponse<List<GenealogyDto>>> GetGenealogy(string lotId)
    {
        if (string.IsNullOrEmpty(lotId))
            return ApiResponse<List<GenealogyDto>>.Fail("批次号不能为空");

        var genealogies = await _dbContext.ProdGenealogies
            .Where(g => g.ParentLotId == lotId || g.ChildLotId == lotId)
            .OrderBy(g => g.CreatedAt)
            .ToListAsync();

        var result = new List<GenealogyDto>();
        foreach (var g in genealogies)
        {
            var parentLot = await _dbContext.ProdLots.FirstOrDefaultAsync(l => l.LotId == g.ParentLotId);
            var childLot = await _dbContext.ProdLots.FirstOrDefaultAsync(l => l.LotId == g.ChildLotId);

            var relatedLot = parentLot ?? childLot;

            result.Add(new GenealogyDto(
                g.GenealogyId,
                lotId,
                g.ParentLotId,
                g.ChildLotId,
                relatedLot?.ProductName ?? string.Empty,
                g.StepCode,
                g.CreatedAt,
                string.Empty,
                g.OperatorId));
        }

        return ApiResponse<List<GenealogyDto>>.Ok(result);
    }

    // ============================================================================
    // GET /api/trace/impact-analysis - Get impact analysis
    // ============================================================================

    /// <summary>
    /// 获取影响分析（共享谱系关系的批次 - 找到同父批次的所有子批次）。
    /// GET /api/trace/impact-analysis?lotId={lotId}
    /// </summary>
    [HttpGet("impact-analysis")]
    public async Task<ApiResponse<List<ImpactAnalysisDto>>> GetImpactAnalysis([FromQuery] string? lotId = null)
    {
        var result = new List<ImpactAnalysisDto>();

        // Find lots that share parent lots with the given lot (if specified)
        // Or find all lots that have genealogy relations
        var genealogies = await _dbContext.ProdGenealogies
            .ToListAsync();

        var groupedByParent = genealogies
            .Where(g => !string.IsNullOrEmpty(g.ParentLotId))
            .GroupBy(g => g.ParentLotId)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in groupedByParent)
        {
            var siblings = group.Select(g => g.ChildLotId).Distinct().ToList();
            var parentLot = await _dbContext.ProdLots.FirstOrDefaultAsync(l => l.LotId == group.Key);

            foreach (var sibling in siblings)
            {
                if (string.IsNullOrEmpty(lotId) || sibling == lotId)
                {
                    result.Add(new ImpactAnalysisDto(
                        sibling,
                        $"共享父批次 {group.Key}",
                        "Parent",
                        1,
                        "检查该批次是否受到父批次异常影响",
                        DateTime.UtcNow));
                }
            }
        }

        return ApiResponse<List<ImpactAnalysisDto>>.Ok(result);
    }

    // ============================================================================
    // GET /api/trace/customer-report/{lotId} - Get customer trace report data
    // ============================================================================

    /// <summary>
    /// 获取指定批次的客户追溯报告数据。
    /// GET /api/trace/customer-report/{lotId}
    /// </summary>
    [HttpGet("customer-report/{lotId}")]
    public async Task<ApiResponse<List<CustomerTraceReportDto>>> GetCustomerReport(string lotId)
    {
        if (string.IsNullOrEmpty(lotId))
            return ApiResponse<List<CustomerTraceReportDto>>.Fail("批次号不能为空");

        var lot = await _dbContext.ProdLots.FirstOrDefaultAsync(l => l.LotId == lotId);
        if (lot == null)
            return ApiResponse<List<CustomerTraceReportDto>>.Fail("批次未找到");

        var product = await _dbContext.MasterProducts.FirstOrDefaultAsync(p => p.ProductId == lot.ProductId);
        var customer = product != null
            ? await _dbContext.MasterCustomers.FirstOrDefaultAsync(c => c.CustomerId == product.CustomerId)
            : null;

        var report = new List<CustomerTraceReportDto>
        {
            new CustomerTraceReportDto(
                $"CTR-{lotId}",
                lotId,
                customer?.CustomerName ?? "Unknown",
                lot.CreatedAt,
                lot.ProductName,
                lot.TotalPassQty,
                lot.Status == "Completed" ? "Complete" : "InProgress")
        };

        return ApiResponse<List<CustomerTraceReportDto>>.Ok(report);
    }
}
