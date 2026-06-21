using MES.Contracts.Common;
using MES.Contracts.Phase1;

namespace MES.Services.Quality;

public interface IIqcService
{
    Task<IqcTaskResponse> CreateTaskAsync(CreateIqcTaskRequest request);
    Task<PagedResult<IqcTaskResponse>> GetTasksAsync(IqcTaskQuery query);
    Task<IqcTaskDetailResponse> GetTaskDetailAsync(string taskId);
    Task<InspectionResultResponse> ExecuteInspectionAsync(string taskId, ExecuteInspectionRequest request);
    Task<IqcTaskResponse> JudgeAsync(string taskId, JudgeRequest request);
    Task<bool> IsolateBatchAsync(string batchId, string operatorId);
    Task<bool> ReleaseBatchAsync(string batchId, string operatorId);
    Task<IqcStatisticsResponse> GetStatisticsAsync(IqcStatisticsQuery query);
}
