using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Quality;
using MES.Contracts.Common;
using MES.Services.Quality;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComplaintsController : ControllerBase
{
    private readonly IComplaint8DService _complaintService;

    public ComplaintsController(IComplaint8DService complaintService)
    {
        _complaintService = complaintService;
    }

    // ============================================================================
    // Main CRUD
    // ============================================================================

    [HttpGet]
    public async Task<ApiResponse<PagedResult<Complaint8DDto>>> GetComplaints(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? eightDStatus = null,
        [FromQuery] string? customerId = null,
        [FromQuery] string? severity = null)
    {
        var result = await _complaintService.GetPagedAsync(pageIndex, pageSize, status, eightDStatus, customerId, severity);
        return ApiResponse<PagedResult<Complaint8DDto>>.Ok(result);
    }

    [HttpGet("{complaintId}")]
    public async Task<ApiResponse<Complaint8DDto?>> GetComplaint(string complaintId)
    {
        var result = await _complaintService.GetByIdAsync(complaintId);
        return result != null
            ? ApiResponse<Complaint8DDto?>.Ok(result)
            : ApiResponse<Complaint8DDto?>.Fail("Complaint not found");
    }

    [HttpPost]
    public async Task<ApiResponse<Complaint8DDto>> CreateComplaint([FromBody] CreateComplaint8DRequest request)
    {
        var createdBy = User.Identity?.Name ?? "System";
        var result = await _complaintService.CreateAsync(request, createdBy);
        return ApiResponse<Complaint8DDto>.Ok(result, "Complaint created");
    }

    [HttpPut("{complaintId}")]
    public async Task<ApiResponse<bool>> UpdateComplaint(string complaintId, [FromBody] UpdateComplaint8DRequest request)
    {
        request.ComplaintId = complaintId;
        var updatedBy = User.Identity?.Name ?? "System";
        var result = await _complaintService.UpdateAsync(request, updatedBy);
        return result
            ? ApiResponse<bool>.Ok(true, "Complaint updated")
            : ApiResponse<bool>.Fail("Complaint not found");
    }

    [HttpPost("{complaintId}/close")]
    public async Task<ApiResponse<bool>> CloseComplaint(string complaintId, [FromBody] CloseComplaintRequest? body = null)
    {
        var closedBy = User.Identity?.Name ?? "System";
        var comment = body?.ClosureComment;
        var result = await _complaintService.CloseAsync(complaintId, closedBy, comment);
        return result
            ? ApiResponse<bool>.Ok(true, "Complaint closed")
            : ApiResponse<bool>.Fail("Complaint not found");
    }

    // ============================================================================
    // Workflow
    // ============================================================================

    [HttpPost("{complaintId}/advance")]
    public async Task<ApiResponse<bool>> AdvanceStep(string complaintId, [FromBody] Complaint8DAdvancementRequest request)
    {
        request.ComplaintId = complaintId;
        try
        {
            var result = await _complaintService.AdvanceStepAsync(complaintId, request.TargetStep, request.Comment);
            return result
                ? ApiResponse<bool>.Ok(true, $"Advanced to {request.TargetStep}")
                : ApiResponse<bool>.Fail("Complaint not found");
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<bool>.Fail(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<bool>.Fail(ex.Message);
        }
    }

    [HttpPost("{complaintId}/approve")]
    public async Task<ApiResponse<bool>> Approve(string complaintId, [FromBody] Complaint8DApprovalRequest request)
    {
        request.ComplaintId = complaintId;
        var approver = User.Identity?.Name ?? "System";
        var result = await _complaintService.ApproveAsync(complaintId, request.Approved, request.Comment, approver);
        return result
            ? ApiResponse<bool>.Ok(true, request.Approved ? "Complaint approved" : "Complaint rejected")
            : ApiResponse<bool>.Fail("Complaint not found");
    }

    // ============================================================================
    // Statistics
    // ============================================================================

    [HttpGet("statistics")]
    public async Task<ApiResponse<ComplaintStatisticsDto>> GetStatistics()
    {
        var result = await _complaintService.GetStatisticsAsync();
        return ApiResponse<ComplaintStatisticsDto>.Ok(result);
    }

    // ============================================================================
    // Sub-table: Team Members
    // ============================================================================

    [HttpGet("{complaintId}/team-members")]
    public async Task<ApiResponse<List<Complaint8DTeamMemberDto>>> GetTeamMembers(string complaintId)
    {
        var complaint = await _complaintService.GetByIdAsync(complaintId);
        if (complaint == null)
            return ApiResponse<List<Complaint8DTeamMemberDto>>.Fail("Complaint not found");

        return ApiResponse<List<Complaint8DTeamMemberDto>>.Ok(complaint.TeamMembers);
    }

    [HttpPost("{complaintId}/team-members")]
    public async Task<ApiResponse<bool>> AddTeamMember(string complaintId, [FromBody] Complaint8DTeamMemberDto request)
    {
        var complaint = await _complaintService.GetByIdAsync(complaintId);
        if (complaint == null)
            return ApiResponse<bool>.Fail("Complaint not found");

        await _complaintService.AddTeamMemberAsync(complaintId, request);
        return ApiResponse<bool>.Ok(true, "Team member added");
    }

    [HttpDelete("team-members/{memberId}")]
    public async Task<ApiResponse<bool>> DeleteTeamMember(string memberId)
    {
        var result = await _complaintService.DeleteTeamMemberAsync(memberId);
        return result
            ? ApiResponse<bool>.Ok(true, "Team member deleted")
            : ApiResponse<bool>.Fail("Team member not found");
    }

    // ============================================================================
    // Sub-table: Containments
    // ============================================================================

    [HttpGet("{complaintId}/containments")]
    public async Task<ApiResponse<List<Complaint8DContainmentDto>>> GetContainments(string complaintId)
    {
        var complaint = await _complaintService.GetByIdAsync(complaintId);
        if (complaint == null)
            return ApiResponse<List<Complaint8DContainmentDto>>.Fail("Complaint not found");

        return ApiResponse<List<Complaint8DContainmentDto>>.Ok(complaint.Containments);
    }

    [HttpPost("{complaintId}/containments")]
    public async Task<ApiResponse<bool>> AddContainment(string complaintId, [FromBody] Complaint8DContainmentDto request)
    {
        var complaint = await _complaintService.GetByIdAsync(complaintId);
        if (complaint == null)
            return ApiResponse<bool>.Fail("Complaint not found");

        await _complaintService.AddContainmentAsync(complaintId, request);
        return ApiResponse<bool>.Ok(true, "Containment added");
    }

    [HttpPut("containments/{containmentId}")]
    public async Task<ApiResponse<bool>> UpdateContainment(string containmentId, [FromBody] Complaint8DContainmentDto request)
    {
        var result = await _complaintService.UpdateContainmentAsync(containmentId, request);
        return result
            ? ApiResponse<bool>.Ok(true, "Containment updated")
            : ApiResponse<bool>.Fail("Containment not found");
    }

    [HttpDelete("containments/{containmentId}")]
    public async Task<ApiResponse<bool>> DeleteContainment(string containmentId)
    {
        var result = await _complaintService.DeleteContainmentAsync(containmentId);
        return result
            ? ApiResponse<bool>.Ok(true, "Containment deleted")
            : ApiResponse<bool>.Fail("Containment not found");
    }

    // ============================================================================
    // Sub-table: Root Causes
    // ============================================================================

    [HttpGet("{complaintId}/root-causes")]
    public async Task<ApiResponse<List<Complaint8DRootCauseDto>>> GetRootCauses(string complaintId)
    {
        var complaint = await _complaintService.GetByIdAsync(complaintId);
        if (complaint == null)
            return ApiResponse<List<Complaint8DRootCauseDto>>.Fail("Complaint not found");

        return ApiResponse<List<Complaint8DRootCauseDto>>.Ok(complaint.RootCauses);
    }

    [HttpPost("{complaintId}/root-causes")]
    public async Task<ApiResponse<bool>> AddRootCause(string complaintId, [FromBody] Complaint8DRootCauseDto request)
    {
        var complaint = await _complaintService.GetByIdAsync(complaintId);
        if (complaint == null)
            return ApiResponse<bool>.Fail("Complaint not found");

        await _complaintService.AddRootCauseAsync(complaintId, request);
        return ApiResponse<bool>.Ok(true, "Root cause added");
    }

    [HttpPut("root-causes/{causeId}")]
    public async Task<ApiResponse<bool>> UpdateRootCause(string causeId, [FromBody] Complaint8DRootCauseDto request)
    {
        var result = await _complaintService.UpdateRootCauseAsync(causeId, request);
        return result
            ? ApiResponse<bool>.Ok(true, "Root cause updated")
            : ApiResponse<bool>.Fail("Root cause not found");
    }

    [HttpDelete("root-causes/{causeId}")]
    public async Task<ApiResponse<bool>> DeleteRootCause(string causeId)
    {
        var result = await _complaintService.DeleteRootCauseAsync(causeId);
        return result
            ? ApiResponse<bool>.Ok(true, "Root cause deleted")
            : ApiResponse<bool>.Fail("Root cause not found");
    }

    // ============================================================================
    // Sub-table: Actions
    // ============================================================================

    [HttpGet("{complaintId}/actions")]
    public async Task<ApiResponse<List<Complaint8DActionDto>>> GetActions(string complaintId)
    {
        var complaint = await _complaintService.GetByIdAsync(complaintId);
        if (complaint == null)
            return ApiResponse<List<Complaint8DActionDto>>.Fail("Complaint not found");

        return ApiResponse<List<Complaint8DActionDto>>.Ok(complaint.Actions);
    }

    [HttpPost("{complaintId}/actions")]
    public async Task<ApiResponse<bool>> AddAction(string complaintId, [FromBody] Complaint8DActionDto request)
    {
        var complaint = await _complaintService.GetByIdAsync(complaintId);
        if (complaint == null)
            return ApiResponse<bool>.Fail("Complaint not found");

        await _complaintService.AddActionAsync(complaintId, request);
        return ApiResponse<bool>.Ok(true, "Action added");
    }

    [HttpPut("actions/{actionId}")]
    public async Task<ApiResponse<bool>> UpdateAction(string actionId, [FromBody] Complaint8DActionDto request)
    {
        var result = await _complaintService.UpdateActionAsync(actionId, request);
        return result
            ? ApiResponse<bool>.Ok(true, "Action updated")
            : ApiResponse<bool>.Fail("Action not found");
    }

    [HttpDelete("actions/{actionId}")]
    public async Task<ApiResponse<bool>> DeleteAction(string actionId)
    {
        var result = await _complaintService.DeleteActionAsync(actionId);
        return result
            ? ApiResponse<bool>.Ok(true, "Action deleted")
            : ApiResponse<bool>.Fail("Action not found");
    }

    // ============================================================================
    // Sub-table: Doc Updates
    // ============================================================================

    [HttpGet("{complaintId}/doc-updates")]
    public async Task<ApiResponse<List<Complaint8DDocUpdateDto>>> GetDocUpdates(string complaintId)
    {
        var complaint = await _complaintService.GetByIdAsync(complaintId);
        if (complaint == null)
            return ApiResponse<List<Complaint8DDocUpdateDto>>.Fail("Complaint not found");

        return ApiResponse<List<Complaint8DDocUpdateDto>>.Ok(complaint.DocUpdates);
    }

    [HttpPost("{complaintId}/doc-updates")]
    public async Task<ApiResponse<bool>> AddDocUpdate(string complaintId, [FromBody] Complaint8DDocUpdateDto request)
    {
        var complaint = await _complaintService.GetByIdAsync(complaintId);
        if (complaint == null)
            return ApiResponse<bool>.Fail("Complaint not found");

        await _complaintService.AddDocUpdateAsync(complaintId, request);
        return ApiResponse<bool>.Ok(true, "Doc update added");
    }

    [HttpPut("doc-updates/{docId}")]
    public async Task<ApiResponse<bool>> UpdateDocUpdate(string docId, [FromBody] Complaint8DDocUpdateDto request)
    {
        var result = await _complaintService.UpdateDocUpdateAsync(docId, request);
        return result
            ? ApiResponse<bool>.Ok(true, "Doc update updated")
            : ApiResponse<bool>.Fail("Doc update not found");
    }

    [HttpDelete("doc-updates/{docId}")]
    public async Task<ApiResponse<bool>> DeleteDocUpdate(string docId)
    {
        var result = await _complaintService.DeleteDocUpdateAsync(docId);
        return result
            ? ApiResponse<bool>.Ok(true, "Doc update deleted")
            : ApiResponse<bool>.Fail("Doc update not found");
    }
}

/// <summary>
/// Request body for closing a complaint with optional comment.
/// </summary>
public class CloseComplaintRequest
{
    public string? ClosureComment { get; set; }
}
