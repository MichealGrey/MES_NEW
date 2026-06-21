using MES.Contracts.Common;
using MES.Contracts.Engineering;

namespace MES.Services.Engineering;

public interface IEngineeringChangeService
{
    // Main CRUD
    Task<PagedResult<EcnRequestDto>> GetPagedAsync(int pageIndex, int pageSize,
        string? status, string? ecnType, string? changeCategory, string? urgency,
        string? riskLevel, string? requestedBy, string? search);
    Task<EcnRequestDto?> GetByIdAsync(string ecnId);
    Task<EcnRequestDto> CreateAsync(CreateEcnRequest request, string createdBy);
    Task<bool> UpdateAsync(UpdateEcnRequest request, string updatedBy);
    Task<bool> DeleteAsync(string ecnId);

    // Workflow
    Task<bool> SubmitAsync(string ecnId);
    Task<bool> AdvanceAsync(string ecnId, string targetStatus, string? comments);
    Task<bool> ApproveAsync(string ecnId, bool approved, string? comments, string? rejectReason, string approver);
    Task<bool> CloseAsync(string ecnId, string closedBy, string? comment);
    Task<bool> NotifyAllDeptsAsync(string ecnId);

    // Queries
    Task<List<EcnRequestDto>> GetPendingApprovalAsync();
    Task<List<EcnRequestDto>> GetImplementingAsync();
    Task<List<EcnRequestDto>> GetOverdueAsync();
    Task<EcnStatisticsDto> GetStatisticsAsync(DateTime? startDate, DateTime? endDate);

    // Sub-table: EcnItem
    Task<List<EcnItemDto>> GetItemsAsync(string ecnId);
    Task<EcnItemDto> CreateItemAsync(CreateEcnItemRequest request, string createdBy);
    Task<bool> UpdateItemAsync(string itemId, CreateEcnItemRequest request, string updatedBy);
    Task<bool> DeleteItemAsync(string itemId);

    // Sub-table: EcnImpactItem
    Task<List<EcnImpactItemDto>> GetImpactsAsync(string ecnId);
    Task<EcnImpactItemDto> CreateImpactAsync(CreateEcnImpactItemRequest request, string createdBy);
    Task<bool> DeleteImpactAsync(string impactId);

    // Sub-table: EcnApprover
    Task<List<EcnApproverDto>> GetApproversAsync(string ecnId);
    Task<EcnApproverDto> CreateApproverAsync(CreateEcnApproverRequest request);
    Task<bool> UpdateApproverAsync(UpdateEcnApproverRequest request);
    Task<bool> DeleteApproverAsync(string approverId);

    // Sub-table: EcnNotifyDept
    Task<List<EcnNotifyDeptDto>> GetNotifyDeptsAsync(string ecnId);
    Task<EcnNotifyDeptDto> CreateNotifyDeptAsync(CreateEcnNotifyDeptRequest request);
    Task<bool> ConfirmNotifyAsync(string notifyId, string confirmedBy);
    Task<bool> DeleteNotifyDeptAsync(string notifyId);

    // Sub-table: EcnImplement
    Task<List<EcnImplementDto>> GetImplementsAsync(string ecnId);
    Task<EcnImplementDto> CreateImplementAsync(CreateEcnImplementRequest request, string createdBy);
    Task<bool> UpdateImplementAsync(UpdateEcnImplementRequest request);
    Task<bool> DeleteImplementAsync(string implementId);
}
