using MES.Contracts.Common;
using MES.Contracts.Phase1;

namespace MES.Services.Production;

public interface IAbnormalService
{
    Task<AbnormalRecordResponse> ReportAsync(ReportAbnormalRequest request);
    Task<PagedResult<AbnormalRecordResponse>> GetRecordsAsync(AbnormalQuery query);
    Task<AbnormalRecordResponse> GetDetailAsync(string abnormalId);
    Task<LineStopResponse> StopLineAsync(LineStopRequest request);
    Task<LineStopResponse> ResumeLineAsync(string stopId, string resumeBy, string comment);
    Task<bool> HandleAsync(HandleAbnormalRequest request);
    Task<bool> VerifyAsync(VerifyAbnormalRequest request);
    Task<AbnormalStatisticsResponse> GetStatisticsAsync(AbnormalQuery query);
}
