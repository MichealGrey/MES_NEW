using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IRouteService
{
    Task<List<RouteInfo>> GetAllRoutesAsync();
    Task<RouteInfo?> GetRouteAsync(string routeId, string? version = null);
    Task SaveRouteAsync(RouteInfo route);
    Task<RouteStep?> GetCurrentStepAsync(string lotId);
    Task<RouteStep?> GetNextStepAsync(string lotId, string routeId, string? routeVersion, int currentStepSeq);
    Task<List<RouteStep>> GetStepsAsync(string routeId, string? version = null);
    Task<bool> IsEquipmentAllowedAsync(string stepCode, string equipmentGroup);
}
