using MES.Contracts.Production;
using MES.Services.Production.Models;

namespace MES.Services.Production;

public interface IProductionService
{
    // Route operations
    Task<List<RouteInfo>> GetAllRoutesAsync();
    Task<RouteInfo?> GetRouteAsync(string routeId, string? version = null);
    Task<List<RouteStep>> GetRouteStepsAsync(string routeId, string? version = null);

    // TrackIn operations
    Task<TrackValidationResult> ValidateTrackInAsync(TrackInRequest request);
    Task<TrackResultDto> TrackInAsync(TrackInRequest request);

    // TrackOut operations
    Task<TrackValidationResult> ValidateTrackOutAsync(TrackOutRequest request);
    Task<TrackResultDto> TrackOutAsync(TrackOutRequest request);

    // History & audit
    Task<List<LotStepRecord>> GetOperationHistoryAsync(string lotId);
    Task<List<AuditTrail>> GetAuditTrailAsync(string entityType, string entityId);
    Task<Dictionary<string, object>> GetLotQuantitySummaryAsync(string lotId);

    // Seed data
    Task SeedAsync();
}
