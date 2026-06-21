using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Engineering;
using MES.Contracts.Common;
using MES.Services.Engineering;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EngineeringChangeController : ControllerBase
{
    private readonly IEngineeringChangeService _ecnService;

    public EngineeringChangeController(IEngineeringChangeService ecnService)
    {
        _ecnService = ecnService;
    }

    // ============================================================================
    // Main CRUD
    // ============================================================================

    [HttpGet]
    public async Task<ApiResponse<PagedResult<EcnRequestDto>>> GetECNs(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? ecnType = null,
        [FromQuery] string? changeCategory = null,
        [FromQuery] string? urgency = null,
        [FromQuery] string? riskLevel = null,
        [FromQuery] string? requestedBy = null,
        [FromQuery] string? search = null)
    {
        var result = await _ecnService.GetPagedAsync(pageIndex, pageSize, status, ecnType, changeCategory, urgency, riskLevel, requestedBy, search);
        return ApiResponse<PagedResult<EcnRequestDto>>.Ok(result);
    }

    [HttpGet("{ecnId}")]
    public async Task<ApiResponse<EcnRequestDto?>> GetECN(string ecnId)
    {
        var result = await _ecnService.GetByIdAsync(ecnId);
        return result != null
            ? ApiResponse<EcnRequestDto?>.Ok(result)
            : ApiResponse<EcnRequestDto?>.Fail("ECN not found");
    }

    [HttpPost]
    public async Task<ApiResponse<EcnRequestDto>> CreateECN([FromBody] CreateEcnRequest request)
    {
        var createdBy = User.Identity?.Name ?? "System";
        var result = await _ecnService.CreateAsync(request, createdBy);
        return ApiResponse<EcnRequestDto>.Ok(result, "ECN created");
    }

    [HttpPut("{ecnId}")]
    public async Task<ApiResponse<bool>> UpdateECN(string ecnId, [FromBody] UpdateEcnRequest request)
    {
        request.EcnId = ecnId;
        var updatedBy = User.Identity?.Name ?? "System";
        var result = await _ecnService.UpdateAsync(request, updatedBy);
        return result
            ? ApiResponse<bool>.Ok(true, "ECN updated")
            : ApiResponse<bool>.Fail("ECN not found");
    }

    [HttpDelete("{ecnId}")]
    public async Task<ApiResponse<bool>> DeleteECN(string ecnId)
    {
        var result = await _ecnService.DeleteAsync(ecnId);
        return result
            ? ApiResponse<bool>.Ok(true, "ECN deleted")
            : ApiResponse<bool>.Fail("ECN not found");
    }

    // ============================================================================
    // Workflow
    // ============================================================================

    [HttpPost("{ecnId}/advance")]
    public async Task<ApiResponse<bool>> AdvanceStep(string ecnId, [FromBody] EcnAdvanceRequest request)
    {
        request.EcnId = ecnId;
        try
        {
            var result = await _ecnService.AdvanceAsync(ecnId, request.TargetStatus, request.Comments);
            return result
                ? ApiResponse<bool>.Ok(true, $"ECN advanced to {request.TargetStatus}")
                : ApiResponse<bool>.Fail("ECN not found");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<bool>.Fail(ex.Message);
        }
    }

    [HttpPost("{ecnId}/approve")]
    public async Task<ApiResponse<bool>> ApproveECN(string ecnId, [FromBody] EcnApprovalRequest request)
    {
        request.EcnId = ecnId;
        var approver = User.Identity?.Name ?? "System";
        var result = await _ecnService.ApproveAsync(ecnId, request.Approved, request.Comments, request.RejectReason, approver);
        return result
            ? ApiResponse<bool>.Ok(true, request.Approved ? "ECN approved" : "ECN rejected")
            : ApiResponse<bool>.Fail("ECN not found");
    }

    [HttpPost("{ecnId}/submit")]
    public async Task<ApiResponse<bool>> SubmitForApproval(string ecnId)
    {
        var result = await _ecnService.SubmitAsync(ecnId);
        return result
            ? ApiResponse<bool>.Ok(true, "ECN submitted for approval")
            : ApiResponse<bool>.Fail("ECN not found");
    }

    [HttpPost("{ecnId}/close")]
    public async Task<ApiResponse<bool>> CloseECN(string ecnId, [FromBody] string? comment = null)
    {
        var closedBy = User.Identity?.Name ?? "System";
        var result = await _ecnService.CloseAsync(ecnId, closedBy, comment);
        return result
            ? ApiResponse<bool>.Ok(true, "ECN closed")
            : ApiResponse<bool>.Fail("ECN not found");
    }

    [HttpPost("{ecnId}/notify")]
    public async Task<ApiResponse<bool>> NotifyDepartments(string ecnId)
    {
        var result = await _ecnService.NotifyAllDeptsAsync(ecnId);
        return result
            ? ApiResponse<bool>.Ok(true, "Departments notified")
            : ApiResponse<bool>.Fail("ECN not found");
    }

    // ============================================================================
    // Statistics
    // ============================================================================

    [HttpGet("statistics")]
    public async Task<ApiResponse<EcnStatisticsDto>> GetStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var result = await _ecnService.GetStatisticsAsync(startDate, endDate);
        return ApiResponse<EcnStatisticsDto>.Ok(result);
    }

    [HttpGet("pending-approval")]
    public async Task<ApiResponse<List<EcnRequestDto>>> GetPendingApproval()
    {
        var result = await _ecnService.GetPendingApprovalAsync();
        return ApiResponse<List<EcnRequestDto>>.Ok(result);
    }

    [HttpGet("implementing")]
    public async Task<ApiResponse<List<EcnRequestDto>>> GetImplementing()
    {
        var result = await _ecnService.GetImplementingAsync();
        return ApiResponse<List<EcnRequestDto>>.Ok(result);
    }

    [HttpGet("overdue")]
    public async Task<ApiResponse<List<EcnRequestDto>>> GetOverdue()
    {
        var result = await _ecnService.GetOverdueAsync();
        return ApiResponse<List<EcnRequestDto>>.Ok(result);
    }

    // ============================================================================
    // Sub-table: EcnItem
    // ============================================================================

    [HttpGet("{ecnId}/items")]
    public async Task<ApiResponse<List<EcnItemDto>>> GetItems(string ecnId)
    {
        var result = await _ecnService.GetItemsAsync(ecnId);
        return ApiResponse<List<EcnItemDto>>.Ok(result);
    }

    [HttpPost("items")]
    public async Task<ApiResponse<EcnItemDto>> CreateItem([FromBody] CreateEcnItemRequest request)
    {
        var createdBy = User.Identity?.Name ?? "System";
        var result = await _ecnService.CreateItemAsync(request, createdBy);
        return ApiResponse<EcnItemDto>.Ok(result, "Item created");
    }

    [HttpPut("items/{itemId}")]
    public async Task<ApiResponse<bool>> UpdateItem(string itemId, [FromBody] CreateEcnItemRequest request)
    {
        var updatedBy = User.Identity?.Name ?? "System";
        var result = await _ecnService.UpdateItemAsync(itemId, request, updatedBy);
        return result
            ? ApiResponse<bool>.Ok(true, "Item updated")
            : ApiResponse<bool>.Fail("Item not found");
    }

    [HttpDelete("items/{itemId}")]
    public async Task<ApiResponse<bool>> DeleteItem(string itemId)
    {
        var result = await _ecnService.DeleteItemAsync(itemId);
        return result
            ? ApiResponse<bool>.Ok(true, "Item deleted")
            : ApiResponse<bool>.Fail("Item not found");
    }

    // ============================================================================
    // Sub-table: EcnImpactItem
    // ============================================================================

    [HttpGet("{ecnId}/impacts")]
    public async Task<ApiResponse<List<EcnImpactItemDto>>> GetImpacts(string ecnId)
    {
        var result = await _ecnService.GetImpactsAsync(ecnId);
        return ApiResponse<List<EcnImpactItemDto>>.Ok(result);
    }

    [HttpPost("impacts")]
    public async Task<ApiResponse<EcnImpactItemDto>> CreateImpact([FromBody] CreateEcnImpactItemRequest request)
    {
        var createdBy = User.Identity?.Name ?? "System";
        var result = await _ecnService.CreateImpactAsync(request, createdBy);
        return ApiResponse<EcnImpactItemDto>.Ok(result, "Impact item created");
    }

    [HttpDelete("impacts/{impactId}")]
    public async Task<ApiResponse<bool>> DeleteImpact(string impactId)
    {
        var result = await _ecnService.DeleteImpactAsync(impactId);
        return result
            ? ApiResponse<bool>.Ok(true, "Impact item deleted")
            : ApiResponse<bool>.Fail("Impact item not found");
    }

    // ============================================================================
    // Sub-table: EcnApprover
    // ============================================================================

    [HttpGet("{ecnId}/approvers")]
    public async Task<ApiResponse<List<EcnApproverDto>>> GetApprovers(string ecnId)
    {
        var result = await _ecnService.GetApproversAsync(ecnId);
        return ApiResponse<List<EcnApproverDto>>.Ok(result);
    }

    [HttpPost("approvers")]
    public async Task<ApiResponse<EcnApproverDto>> CreateApprover([FromBody] CreateEcnApproverRequest request)
    {
        var result = await _ecnService.CreateApproverAsync(request);
        return ApiResponse<EcnApproverDto>.Ok(result, "Approver added");
    }

    [HttpPut("approvers/{approverId}")]
    public async Task<ApiResponse<bool>> UpdateApprover(string approverId, [FromBody] UpdateEcnApproverRequest request)
    {
        request.ApproverId = approverId;
        var result = await _ecnService.UpdateApproverAsync(request);
        return result
            ? ApiResponse<bool>.Ok(true, "Approver updated")
            : ApiResponse<bool>.Fail("Approver not found");
    }

    [HttpDelete("approvers/{approverId}")]
    public async Task<ApiResponse<bool>> DeleteApprover(string approverId)
    {
        var result = await _ecnService.DeleteApproverAsync(approverId);
        return result
            ? ApiResponse<bool>.Ok(true, "Approver removed")
            : ApiResponse<bool>.Fail("Approver not found");
    }

    // ============================================================================
    // Sub-table: EcnNotifyDept
    // ============================================================================

    [HttpGet("{ecnId}/notify-depts")]
    public async Task<ApiResponse<List<EcnNotifyDeptDto>>> GetNotifyDepts(string ecnId)
    {
        var result = await _ecnService.GetNotifyDeptsAsync(ecnId);
        return ApiResponse<List<EcnNotifyDeptDto>>.Ok(result);
    }

    [HttpPost("notify-depts")]
    public async Task<ApiResponse<EcnNotifyDeptDto>> CreateNotifyDept([FromBody] CreateEcnNotifyDeptRequest request)
    {
        var result = await _ecnService.CreateNotifyDeptAsync(request);
        return ApiResponse<EcnNotifyDeptDto>.Ok(result, "Department added");
    }

    [HttpPost("notify-depts/{notifyId}/confirm")]
    public async Task<ApiResponse<bool>> ConfirmNotify(string notifyId)
    {
        var confirmedBy = User.Identity?.Name ?? "System";
        var result = await _ecnService.ConfirmNotifyAsync(notifyId, confirmedBy);
        return result
            ? ApiResponse<bool>.Ok(true, "Department confirmed")
            : ApiResponse<bool>.Fail("Notification not found");
    }

    [HttpDelete("notify-depts/{notifyId}")]
    public async Task<ApiResponse<bool>> DeleteNotifyDept(string notifyId)
    {
        var result = await _ecnService.DeleteNotifyDeptAsync(notifyId);
        return result
            ? ApiResponse<bool>.Ok(true, "Department removed")
            : ApiResponse<bool>.Fail("Department not found");
    }

    // ============================================================================
    // Sub-table: EcnImplement
    // ============================================================================

    [HttpGet("{ecnId}/implements")]
    public async Task<ApiResponse<List<EcnImplementDto>>> GetImplements(string ecnId)
    {
        var result = await _ecnService.GetImplementsAsync(ecnId);
        return ApiResponse<List<EcnImplementDto>>.Ok(result);
    }

    [HttpPost("implements")]
    public async Task<ApiResponse<EcnImplementDto>> CreateImplement([FromBody] CreateEcnImplementRequest request)
    {
        var createdBy = User.Identity?.Name ?? "System";
        var result = await _ecnService.CreateImplementAsync(request, createdBy);
        return ApiResponse<EcnImplementDto>.Ok(result, "Implement task created");
    }

    [HttpPut("implements/{implementId}")]
    public async Task<ApiResponse<bool>> UpdateImplement(string implementId, [FromBody] UpdateEcnImplementRequest request)
    {
        request.ImplementId = implementId;
        var result = await _ecnService.UpdateImplementAsync(request);
        return result
            ? ApiResponse<bool>.Ok(true, "Implement task updated")
            : ApiResponse<bool>.Fail("Implement task not found");
    }

    [HttpDelete("implements/{implementId}")]
    public async Task<ApiResponse<bool>> DeleteImplement(string implementId)
    {
        var result = await _ecnService.DeleteImplementAsync(implementId);
        return result
            ? ApiResponse<bool>.Ok(true, "Implement task deleted")
            : ApiResponse<bool>.Fail("Implement task not found");
    }
}
