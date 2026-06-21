using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase2;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Planning;

public class PlanningService : IPlanningService
{
    private readonly MesDbContext _context;
    private readonly ILogger<PlanningService> _logger;

    public PlanningService(MesDbContext context, ILogger<PlanningService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MppResponse> GeneratePlanAsync(CreateMppRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var planId = $"MPP-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var plan = new MasterProductionPlan
        {
            PlanId = planId,
            PlanName = request.PlanName,
            PlanType = request.PlanType,
            PlanPeriodStart = request.PlanPeriodStart,
            PlanPeriodEnd = request.PlanPeriodEnd,
            Planner = request.Planner,
            Remark = request.Remark,
            Status = "Draft",
            CreatedBy = operatorId
        };

        _context.MasterProductionPlans.Add(plan);
        await _context.SaveChangesAsync();

        return MapToResponse(plan);
    }

    public async Task<PagedResult<MppResponse>> GetPlansAsync(MppQuery query)
    {
        var iqQuery = _context.MasterProductionPlans.AsQueryable();

        if (!string.IsNullOrEmpty(query.Status))
            iqQuery = iqQuery.Where(p => p.Status == query.Status);
        if (query.StartDate.HasValue)
            iqQuery = iqQuery.Where(p => p.PlanPeriodStart >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            iqQuery = iqQuery.Where(p => p.PlanPeriodEnd <= query.EndDate.Value);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => MapToResponse(p))
            .ToListAsync();

        return new PagedResult<MppResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<MppResponse> GetPlanAsync(string planId)
    {
        var plan = await _context.MasterProductionPlans
            .FirstOrDefaultAsync(p => p.PlanId == planId);

        if (plan == null)
            throw new KeyNotFoundException($"Plan {planId} not found");

        return MapToResponse(plan);
    }

    public async Task<MppResponse> PublishPlanAsync(string planId, string operatorId)
    {
        var plan = await _context.MasterProductionPlans.FirstOrDefaultAsync(p => p.PlanId == planId);
        if (plan == null)
            throw new KeyNotFoundException($"Plan {planId} not found");

        var now = DateTime.UtcNow;
        plan.Status = "Published";
        plan.PublishedBy = operatorId;
        plan.PublishedAt = now;
        plan.UpdatedBy = operatorId;

        await _context.SaveChangesAsync();

        return MapToResponse(plan);
    }

    public async Task<List<CapacityLoadResponse>> GetCapacityLoadsAsync(string planId)
    {
        return await _context.CapacityLoads
            .Where(cl => cl.PlanId == planId)
            .OrderBy(cl => cl.ProcessCode)
            .Select(cl => new CapacityLoadResponse
            {
                LoadId = cl.LoadId,
                PlanId = cl.PlanId,
                ProcessCode = cl.ProcessCode,
                ProcessName = cl.ProcessName,
                EquipmentGroup = cl.EquipmentGroup,
                Uph = cl.Uph,
                AvailableHours = cl.AvailableHours,
                RequiredHours = cl.RequiredHours,
                LoadRate = cl.LoadRate,
                IsBottleneck = cl.IsBottleneck,
                AvailableQty = cl.AvailableQty,
                RequiredQty = cl.RequiredQty,
                ShortageQty = cl.ShortageQty,
                ShiftPlan = cl.ShiftPlan,
                CalculatedAt = cl.CalculatedAt
            })
            .ToListAsync();
    }

    public async Task<List<CapacityLoadResponse>> GetBottlenecksAsync(string planId)
    {
        return await _context.CapacityLoads
            .Where(cl => cl.PlanId == planId && cl.IsBottleneck)
            .OrderByDescending(cl => cl.LoadRate)
            .Select(cl => new CapacityLoadResponse
            {
                LoadId = cl.LoadId,
                PlanId = cl.PlanId,
                ProcessCode = cl.ProcessCode,
                ProcessName = cl.ProcessName,
                EquipmentGroup = cl.EquipmentGroup,
                Uph = cl.Uph,
                AvailableHours = cl.AvailableHours,
                RequiredHours = cl.RequiredHours,
                LoadRate = cl.LoadRate,
                IsBottleneck = cl.IsBottleneck,
                AvailableQty = cl.AvailableQty,
                RequiredQty = cl.RequiredQty,
                ShortageQty = cl.ShortageQty,
                ShiftPlan = cl.ShiftPlan,
                CalculatedAt = cl.CalculatedAt
            })
            .ToListAsync();
    }

    public async Task<CapacitySimulationResponse> SimulateCapacityAsync(CapacitySimulationRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var simulationId = $"SIM-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        // Mock simulation - in production this would run actual capacity calculation
        var simulation = new CapacitySimulation
        {
            SimulationId = simulationId,
            SimulationName = request.SimulationName,
            BasePlanId = request.BasePlanId,
            ScenarioDescription = request.ScenarioDescription,
            ScenarioParams = request.ScenarioParams,
            TotalDemandQty = 10000, // Mock data
            TotalCapacity = 12000,
            CapacityUtilization = 83.33m,
            BottleneckCount = 2,
            ResultSummary = "产能模拟完成，发现2个瓶颈工序",
            ResultData = "{}",
            Status = "Completed",
            CreatedBy = operatorId
        };

        _context.CapacitySimulations.Add(simulation);
        await _context.SaveChangesAsync();

        return new CapacitySimulationResponse
        {
            SimulationId = simulation.SimulationId,
            SimulationName = simulation.SimulationName,
            BasePlanId = simulation.BasePlanId,
            ScenarioDescription = simulation.ScenarioDescription,
            TotalDemandQty = simulation.TotalDemandQty,
            TotalCapacity = simulation.TotalCapacity,
            CapacityUtilization = simulation.CapacityUtilization,
            BottleneckCount = simulation.BottleneckCount,
            ResultSummary = simulation.ResultSummary,
            Status = simulation.Status,
            CreatedAt = simulation.CreatedAt
        };
    }

    private static MppResponse MapToResponse(MasterProductionPlan plan) => new()
    {
        PlanId = plan.PlanId,
        PlanName = plan.PlanName,
        PlanType = plan.PlanType,
        PlanPeriodStart = plan.PlanPeriodStart,
        PlanPeriodEnd = plan.PlanPeriodEnd,
        Status = plan.Status,
        TotalDemandQty = plan.TotalDemandQty,
        TotalCapacity = plan.TotalCapacity,
        CapacityUtilization = plan.CapacityUtilization,
        BottleneckIdentified = plan.BottleneckIdentified,
        BottleneckDescription = plan.BottleneckDescription,
        Planner = plan.Planner,
        Remark = plan.Remark,
        CreatedBy = plan.CreatedBy,
        CreatedAt = plan.CreatedAt,
        PublishedAt = plan.PublishedAt
    };
}
