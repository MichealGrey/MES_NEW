using MES.Contracts.Common;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics;

public interface IKpiService
{
    Task<KpiDashboardResponse> GetDashboardAsync(string? periodType = null);
    Task<KpiMetricResponse> GetMetricDetailAsync(string metricCode, DateTime? startDate = null, DateTime? endDate = null);
    Task<KpiMetricResponse> CaptureMetricAsync(KpiCaptureRequest request, string operatorId);
}
