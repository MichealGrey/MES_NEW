using MES.Adapters.Abstractions;

namespace MES.Adapters.Abstractions.Oa;

/// <summary>
/// OA 审批流集成适配器
/// 对接场景：MES 审批流程→OA：工艺变更、工程变更、不合格品处置、插单审批、报废审批
/// </summary>
public interface IMesOaAdapter : IMesAdapter
{
    Task<AdapterResult<ApprovalPushResult>> PushApprovalRequestAsync(ApprovalRequest request);
    Task<AdapterResult<ApprovalStatus>> GetApprovalStatusAsync(string approvalId);
    Task<AdapterResult<ApprovalCallback>> ReceiveApprovalCallbackAsync(ApprovalResult callback);
}

public class ApprovalRequest
{
    public string ApprovalId { get; set; } = string.Empty;
    public string ApprovalType { get; set; } = string.Empty; // ECN, NCR, RushOrder, Scrap, ProcessChange
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string InitiatorId { get; set; } = string.Empty;
    public string InitiatorName { get; set; } = string.Empty;
    public string? RelatedWorkOrderId { get; set; }
    public string? RelatedLotId { get; set; }
    public List<ApprovalAttachment> Attachments { get; set; } = new();
    public string ApprovalFlow { get; set; } = string.Empty; // 审批流程模板ID
    public string CallbackUrl { get; set; } = string.Empty;
}

public class ApprovalAttachment
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
}

public class ApprovalPushResult
{
    public string ApprovalId { get; set; } = string.Empty;
    public string? OaApprovalNo { get; set; }
    public bool Pushed { get; set; }
}

public class ApprovalStatus
{
    public string ApprovalId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected, Cancelled
    public string? CurrentApprover { get; set; }
    public string? CurrentStep { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<ApprovalStepStatus> StepHistory { get; set; } = new();
}

public class ApprovalStepStatus
{
    public string StepName { get; set; } = string.Empty;
    public string Approver { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty; // Approved, Rejected, Pending
    public string? Comment { get; set; }
    public DateTime? ActionTime { get; set; }
}

public class ApprovalResult
{
    public string ApprovalId { get; set; } = string.Empty;
    public string OaApprovalNo { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty; // Approved, Rejected
    public string? Comment { get; set; }
    public string ApproverId { get; set; } = string.Empty;
    public string ApproverName { get; set; } = string.Empty;
    public DateTime ApprovalTime { get; set; }
}

public class ApprovalCallback
{
    public bool Processed { get; set; }
    public string ApprovalId { get; set; } = string.Empty;
}
