using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Analytics;

public class ReliabilityTestService : IReliabilityTestService
{
    private readonly MesDbContext _context;
    private readonly ILogger<ReliabilityTestService> _logger;

    public ReliabilityTestService(MesDbContext context, ILogger<ReliabilityTestService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ReliabilityTestPlanResponse> CreatePlanAsync(CreateReliabilityTestPlanRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var planId = $"RTP-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var plan = new ReliabilityTestPlan
        {
            PlanId = planId,
            PlanName = request.PlanName,
            TestType = request.TestType,
            ProductId = request.ProductId,
            LotId = request.LotId,
            SampleSize = request.SampleSize,
            TestDuration = request.TestDuration,
            TestConditions = request.TestConditions,
            Status = "Planned",
            FaTriggered = false,
            CreatedBy = operatorId,
            CreatedAt = now
        };

        _context.ReliabilityTestPlans.Add(plan);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created reliability test plan {PlanId}: {PlanName}", planId, request.PlanName);

        return MapToResponse(plan);
    }

    public async Task<PagedResult<ReliabilityTestPlanResponse>> QueryPlansAsync(ReliabilityTestQuery query)
    {
        var iqQuery = _context.ReliabilityTestPlans.AsQueryable();

        if (!string.IsNullOrEmpty(query.TestType))
            iqQuery = iqQuery.Where(x => x.TestType == query.TestType);
        if (!string.IsNullOrEmpty(query.ProductId))
            iqQuery = iqQuery.Where(x => x.ProductId == query.ProductId);
        if (!string.IsNullOrEmpty(query.Status))
            iqQuery = iqQuery.Where(x => x.Status == query.Status);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => MapToResponse(x))
            .ToListAsync();

        return new PagedResult<ReliabilityTestPlanResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<ReliabilityTestPlanResponse> GetPlanAsync(string planId)
    {
        var plan = await _context.ReliabilityTestPlans
            .FirstOrDefaultAsync(x => x.PlanId == planId);

        if (plan == null)
            throw new KeyNotFoundException($"Reliability test plan '{planId}' not found");

        return MapToResponse(plan);
    }

    public async Task<ReliabilityTestPlanResponse> ExecuteTestAsync(string planId, string resultSummary, string operatorId)
    {
        var plan = await _context.ReliabilityTestPlans
            .FirstOrDefaultAsync(x => x.PlanId == planId);

        if (plan == null)
            throw new KeyNotFoundException($"Reliability test plan '{planId}' not found");

        plan.Status = "Completed";
        plan.EndDate = DateTime.UtcNow;
        plan.ResultSummary = resultSummary;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Executed reliability test plan {PlanId}", planId);

        return MapToResponse(plan);
    }

    public async Task<ReliabilityTestPlanResponse> TriggerFailFaAsync(string planId, string operatorId)
    {
        var plan = await _context.ReliabilityTestPlans
            .FirstOrDefaultAsync(x => x.PlanId == planId);

        if (plan == null)
            throw new KeyNotFoundException($"Reliability test plan '{planId}' not found");

        plan.Status = "Failed";
        plan.FaTriggered = true;
        plan.EndDate = DateTime.UtcNow;
        plan.ResultSummary = "Failure analysis triggered - test failed";

        await _context.SaveChangesAsync();

        _logger.LogWarning("Failure analysis triggered for reliability test plan {PlanId}", planId);

        return MapToResponse(plan);
    }

    private static ReliabilityTestPlanResponse MapToResponse(ReliabilityTestPlan entity) => new()
    {
        PlanId = entity.PlanId,
        PlanName = entity.PlanName,
        TestType = entity.TestType,
        ProductId = entity.ProductId,
        LotId = entity.LotId,
        SampleSize = entity.SampleSize,
        TestDuration = entity.TestDuration,
        TestConditions = entity.TestConditions,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        Status = entity.Status,
        ResultSummary = entity.ResultSummary,
        FaTriggered = entity.FaTriggered,
        CreatedBy = entity.CreatedBy,
        CreatedAt = entity.CreatedAt
    };
}
