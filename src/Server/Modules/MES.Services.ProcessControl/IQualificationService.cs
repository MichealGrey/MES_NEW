using MES.Contracts.Common;
using MES.Contracts.Phase3;

namespace MES.Services.ProcessControl;

public interface IQualificationService
{
    Task<OperatorQualificationResponse> CreateQualificationAsync(CreateOperatorQualificationRequest request, string operatorId);
    Task<OperatorQualificationResponse> GetQualificationAsync(string qualificationId);
    Task<PagedResult<OperatorQualificationResponse>> QueryQualificationsAsync(OperatorQualificationQuery query);
    Task<QualificationCheckLogResponse> CheckQualificationAsync(OperatorQualificationCheckRequest request, string operatorId);
    Task<PagedResult<QualificationCheckLogResponse>> GetCheckLogsAsync(string operatorIdFilter, int pageIndex, int pageSize);
}
