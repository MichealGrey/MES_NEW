using MES.Contracts.Common;
using MES.Contracts.Phase1;

namespace MES.Services.Quality;

public interface IQualityAlertService
{
    Task<QualityAlertResponse> CreateAlertAsync(CreateQualityAlertRequest request);
    Task<PagedResult<QualityAlertResponse>> GetAlertsAsync(QualityAlertQuery query);
    Task<QualityAlertResponse> GetDetailAsync(string alertId);
    Task<List<AffectedLotInfo>> AnalyzeAffectedLotsAsync(string alertId);
    Task<bool> FreezeLotsAsync(string alertId, List<string> lotIds, string operatorId);
    Task<RecallNoticeResponse> GenerateRecallAsync(string alertId, string issuedBy, string issuedByName);
    Task<bool> CloseAlertAsync(CloseQualityAlertRequest request);
}
