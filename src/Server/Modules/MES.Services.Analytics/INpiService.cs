using MES.Contracts.Common;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics;

public interface INpiService
{
    Task<NpiProjectResponse> CreateProjectAsync(CreateNpiProjectRequest request, string operatorId);
    Task<NpiProjectResponse> GetProjectAsync(string projectId);
    Task<PagedResult<NpiProjectResponse>> QueryProjectsAsync(NpiProjectQuery query);
    Task<List<NpiStageResponse>> GetStagesAsync(string projectId);
    Task<NpiStageResponse> ExecuteTrialRunAsync(string projectId, TrialRunRequest request, string operatorId);
    Task<NpiStageResponse> ReviewAsync(string projectId, NpiReviewRequest request, string operatorId);
    Task<NpiProjectResponse> TransferToMassProductionAsync(string projectId, string operatorId);
}
