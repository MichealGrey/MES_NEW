using MES.Contracts.Common;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics;

public interface IYieldService
{
    Task<YieldStatisticsResponse> RecordYieldAsync(string lotId, string? stepCode, int inputQty, int outputQty, DateOnly statDate, string? defectJson = null);
    Task<PagedResult<YieldStatisticsResponse>> GetProcessYieldAsync(YieldQuery query);
    Task<decimal> GetCumulativeYieldAsync(string lotId);
    Task<YieldTrendResponse> GetYieldTrendAsync(string lotId);
    Task<YieldParetoResponse> GetParetoAsync(string? lotId = null, DateOnly? startDate = null, DateOnly? endDate = null);
    Task<DppmResponse> GetDppmAsync(string productId, DateOnly periodStart, DateOnly periodEnd);
}
