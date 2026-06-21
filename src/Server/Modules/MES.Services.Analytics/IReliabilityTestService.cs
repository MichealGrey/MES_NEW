using MES.Contracts.Common;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics;

public interface IReliabilityTestService
{
    Task<ReliabilityTestPlanResponse> CreatePlanAsync(CreateReliabilityTestPlanRequest request, string operatorId);
    Task<PagedResult<ReliabilityTestPlanResponse>> QueryPlansAsync(ReliabilityTestQuery query);
    Task<ReliabilityTestPlanResponse> GetPlanAsync(string planId);
    Task<ReliabilityTestPlanResponse> ExecuteTestAsync(string planId, string resultSummary, string operatorId);
    Task<ReliabilityTestPlanResponse> TriggerFailFaAsync(string planId, string operatorId);
}
