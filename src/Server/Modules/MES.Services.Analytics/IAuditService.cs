using MES.Contracts.Common;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics;

public interface IAuditService
{
    Task<PagedResult<DataCorrectionResponse>> QueryCorrectionsAsync(DataCorrectionQuery query);
    Task<bool> VerifyAuditIntegrityAsync();
    Task<Dictionary<string, string>> HashCheckAsync(string tableName, DateTime? startDate = null, DateTime? endDate = null);
    Task<DataCorrectionResponse> CreateCorrectionAsync(CreateDataCorrectionRequest request, string operatorId);
}
