using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase2;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Planning;

public class RushOrderService : IRushOrderService
{
    private readonly MesDbContext _context;
    private readonly ILogger<RushOrderService> _logger;

    public RushOrderService(MesDbContext context, ILogger<RushOrderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RushOrderResponse> CreateRushOrderAsync(CreateRushOrderRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var requestId = $"RUSH-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var rushOrder = new RushOrderRequest
        {
            RequestId = requestId,
            OrderId = request.OrderId,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            RushQuantity = request.RushQuantity,
            RequiredDate = request.RequiredDate,
            RushReason = request.RushReason,
            PriorityLevel = request.PriorityLevel,
            Status = "Pending",
            CreatedBy = operatorId
        };

        _context.RushOrderRequests.Add(rushOrder);
        await _context.SaveChangesAsync();

        return MapToResponse(rushOrder);
    }

    public async Task<PagedResult<RushOrderResponse>> GetRushOrdersAsync(RushOrderQuery query)
    {
        var iqQuery = _context.RushOrderRequests.AsQueryable();

        if (!string.IsNullOrEmpty(query.Status))
            iqQuery = iqQuery.Where(r => r.Status == query.Status);
        if (!string.IsNullOrEmpty(query.PriorityLevel))
            iqQuery = iqQuery.Where(r => r.PriorityLevel == query.PriorityLevel);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(r => r.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => MapToResponse(r))
            .ToListAsync();

        return new PagedResult<RushOrderResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<RushOrderResponse> GetRushOrderAsync(string requestId)
    {
        var rushOrder = await _context.RushOrderRequests
            .FirstOrDefaultAsync(r => r.RequestId == requestId);

        if (rushOrder == null)
            throw new KeyNotFoundException($"Rush order {requestId} not found");

        return MapToResponse(rushOrder);
    }

    public async Task<List<RushOrderImpactResponse>> AnalyzeImpactAsync(string requestId, string operatorId)
    {
        var rushOrder = await _context.RushOrderRequests
            .FirstOrDefaultAsync(r => r.RequestId == requestId);

        if (rushOrder == null)
            throw new KeyNotFoundException($"Rush order {requestId} not found");

        // Mock impact analysis - in production this would analyze actual production schedule
        var mockImpacts = new[]
        {
            new
            {
                AffectedOrderId = "SO-001",
                ImpactType = "Delay",
                DelayDays = 3,
                Severity = "High"
            },
            new
            {
                AffectedOrderId = "SO-002",
                ImpactType = "Capacity",
                DelayDays = 1,
                Severity = "Medium"
            }
        };

        var impacts = new List<RushOrderImpact>();
        foreach (var mock in mockImpacts)
        {
            var impact = new RushOrderImpact
            {
                RequestId = requestId,
                AffectedOrderId = mock.AffectedOrderId,
                ImpactType = mock.ImpactType,
                ImpactDescription = $"插单将导致订单 {mock.AffectedOrderId} 延迟 {mock.DelayDays} 天",
                DelayDays = mock.DelayDays,
                Severity = mock.Severity,
                MitigationPlan = "建议调整班次以缓解影响"
            };
            impacts.Add(impact);
            _context.RushOrderImpacts.Add(impact);
        }

        rushOrder.ImpactAnalysisDone = true;
        rushOrder.AnalysisSummary = $"影响分析完成，共影响 {impacts.Count} 个订单";

        await _context.SaveChangesAsync();

        return impacts.Select(i => MapImpactToResponse(i)).ToList();
    }

    public async Task<RushOrderResponse> ApproveRushOrderAsync(string requestId, ApproveRushOrderRequest request, string operatorId)
    {
        var rushOrder = await _context.RushOrderRequests
            .FirstOrDefaultAsync(r => r.RequestId == requestId);

        if (rushOrder == null)
            throw new KeyNotFoundException($"Rush order {requestId} not found");

        var now = DateTime.UtcNow;
        rushOrder.ApprovalResult = request.ApprovalResult;
        rushOrder.ApprovalBy = operatorId;
        rushOrder.ApprovalAt = now;
        rushOrder.ApprovalComments = request.ApprovalComments;
        rushOrder.Status = request.ApprovalResult == "Approved" ? "Approved" : "Rejected";

        await _context.SaveChangesAsync();

        return MapToResponse(rushOrder);
    }

    public async Task<RushOrderResponse> ExecuteRushOrderAsync(string requestId, string operatorId)
    {
        var rushOrder = await _context.RushOrderRequests
            .FirstOrDefaultAsync(r => r.RequestId == requestId);

        if (rushOrder == null)
            throw new KeyNotFoundException($"Rush order {requestId} not found");

        if (rushOrder.Status != "Approved")
            throw new InvalidOperationException("Rush order must be approved before execution");

        var now = DateTime.UtcNow;
        rushOrder.Status = "Executing";
        rushOrder.ExecutedBy = operatorId;
        rushOrder.ExecutedAt = now;

        await _context.SaveChangesAsync();

        return MapToResponse(rushOrder);
    }

    private static RushOrderResponse MapToResponse(RushOrderRequest entity) => new()
    {
        RequestId = entity.RequestId,
        OrderId = entity.OrderId,
        CustomerId = entity.CustomerId,
        CustomerName = entity.CustomerName,
        ProductId = entity.ProductId,
        ProductName = entity.ProductName,
        RushQuantity = entity.RushQuantity,
        RequiredDate = entity.RequiredDate,
        RushReason = entity.RushReason,
        PriorityLevel = entity.PriorityLevel,
        Status = entity.Status,
        ImpactAnalysisDone = entity.ImpactAnalysisDone,
        AnalysisSummary = entity.AnalysisSummary,
        ApprovalResult = entity.ApprovalResult,
        ApprovalBy = entity.ApprovalBy,
        ApprovalAt = entity.ApprovalAt,
        ApprovalComments = entity.ApprovalComments,
        ExecutedBy = entity.ExecutedBy,
        ExecutedAt = entity.ExecutedAt,
        CreatedBy = entity.CreatedBy,
        CreatedAt = entity.CreatedAt
    };

    private static RushOrderImpactResponse MapImpactToResponse(RushOrderImpact entity) => new()
    {
        ImpactId = entity.ImpactId,
        RequestId = entity.RequestId,
        AffectedOrderId = entity.AffectedOrderId,
        AffectedWorkOrderId = entity.AffectedWorkOrderId,
        ImpactType = entity.ImpactType,
        ImpactDescription = entity.ImpactDescription,
        DelayDays = entity.DelayDays,
        Severity = entity.Severity,
        MitigationPlan = entity.MitigationPlan,
        AnalyzedAt = entity.AnalyzedAt
    };
}
