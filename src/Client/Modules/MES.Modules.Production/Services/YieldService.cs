using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class YieldService : IYieldService
{
    private readonly IRepository<MasterYieldRule> _ruleRepo;

    public YieldService(IRepository<MasterYieldRule> ruleRepo)
    {
        _ruleRepo = ruleRepo;
    }

    public Task<double> CalculateStepYieldAsync(int passQty, int inputQty)
    {
        if (inputQty <= 0) return Task.FromResult(100.0);
        return Task.FromResult((double)passQty / inputQty * 100);
    }

    public async Task<YieldRule?> CheckYieldRuleAsync(string routeId, string stepCode, double actualYield)
    {
        var rules = await _ruleRepo.GetWhereAsync(r => r.RouteId == routeId && r.StepCode == stepCode && r.IsActive);
        var rule = rules.FirstOrDefault();
        if (rule is null) return null;

        return actualYield < (double)rule.YieldThreshold ? MapToModel(rule) : null;
    }

    public async Task<bool> ShouldAutoHoldAsync(string routeId, string stepCode, double actualYield)
    {
        var rule = await CheckYieldRuleAsync(routeId, stepCode, actualYield);
        return rule?.HoldYield > actualYield;
    }

    private static YieldRule MapToModel(MasterYieldRule entity) => new()
    {
        RuleId = entity.RuleId,
        RouteId = entity.RouteId,
        StepCode = entity.StepCode,
        YieldThreshold = (double)entity.YieldThreshold,
        ActionType = entity.ActionType,
        NotifyRole = entity.NotifyRole,
        IsActive = entity.IsActive,
        // Phase 4 computed properties
        WarningYield = (double)entity.YieldThreshold - 5,
        AlarmYield = (double)entity.YieldThreshold - 10,
        HoldYield = (double)entity.YieldThreshold,
    };
}
