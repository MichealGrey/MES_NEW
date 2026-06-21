using MES.Contracts.Common;
using MES.Contracts.Phase1;

namespace MES.Services.Warehouse;

public interface IIssueReturnService
{
    Task<IssueOrderResponse> IssueMaterialAsync(IssueMaterialRequest request);
    Task<PagedResult<IssueOrderResponse>> GetIssueOrdersAsync(IssueOrderQuery query);
    Task<KitCheckResponse> CheckKitAsync(string workOrderId);
    Task<bool> SkipFifoApprovalAsync(string issueItemId, string approvedBy, string reason);
    Task<ReturnOrderResponse> ReturnMaterialAsync(ReturnMaterialRequest request);
    Task<PagedResult<ReturnOrderResponse>> GetReturnOrdersAsync(ReturnOrderQuery query);
}
