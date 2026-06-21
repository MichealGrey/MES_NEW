using MES.Contracts.Common;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics;

public interface IReportService
{
    Task<ReportScheduleResponse> CreateScheduleAsync(CreateReportScheduleRequest request, string operatorId);
    Task<PagedResult<ReportScheduleResponse>> QuerySchedulesAsync(ReportScheduleQuery query);
    Task<bool> GenerateReportAsync(string scheduleId);
    Task<byte[]> DownloadReportAsync(string scheduleId);
}
