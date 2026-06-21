using MES.Contracts.Common;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics;

public interface ICostService
{
    Task<CostRecordResponse> RecordCostAsync(CostCalculationRequest request, string operatorId);
    Task<CostAnalysisResponse> GetProductAnalysisAsync(string productId, DateOnly? startDate = null, DateOnly? endDate = null);
    Task<CostAnalysisResponse> GetWorkOrderAnalysisAsync(string workOrderId);
    Task<List<CostRecordResponse>> GetVarianceAnalysisAsync(string workOrderId);
    Task<List<CostRecordResponse>> CalculateCostsAsync(CostCalculationRequest request, string operatorId);
}
