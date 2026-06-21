using MES.Contracts.Common;
using MES.Contracts.Phase2;

namespace MES.Services.Planning;

public interface IPlanningService
{
    Task<MppResponse> GeneratePlanAsync(CreateMppRequest request, string operatorId);
    Task<PagedResult<MppResponse>> GetPlansAsync(MppQuery query);
    Task<MppResponse> GetPlanAsync(string planId);
    Task<MppResponse> PublishPlanAsync(string planId, string operatorId);
    Task<List<CapacityLoadResponse>> GetCapacityLoadsAsync(string planId);
    Task<List<CapacityLoadResponse>> GetBottlenecksAsync(string planId);
    Task<CapacitySimulationResponse> SimulateCapacityAsync(CapacitySimulationRequest request, string operatorId);
}
