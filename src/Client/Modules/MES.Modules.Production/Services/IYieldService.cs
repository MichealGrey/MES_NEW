using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IYieldService
{
    Task<double> CalculateStepYieldAsync(int passQty, int inputQty);
    Task<YieldRule?> CheckYieldRuleAsync(string routeId, string stepCode, double actualYield);
    Task<bool> ShouldAutoHoldAsync(string routeId, string stepCode, double actualYield);
}
