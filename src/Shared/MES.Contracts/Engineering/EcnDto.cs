namespace MES.Contracts.Engineering;

// ==================== 主表DTO ====================

public class EcnRequestDto
{
    public string EcnId { get; set; } = string.Empty;
    public string EcnNo { get; set; } = string.Empty;
    public string? EcnTitle { get; set; }
    public string? EcnType { get; set; }
    public string? ChangeCategory { get; set; }
    public string? Reason { get; set; }
    public string? ChangeDescription { get; set; }
    public string? ChangeContent { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Status { get; set; } = "Draft";
    public string? AffectedRoutes { get; set; }
    public string? AffectedProducts { get; set; }
    public string? ImpactAssessment { get; set; }
    public string? Urgency { get; set; }
    public string? RiskLevel { get; set; }
    public string? ReviewComments { get; set; }
    public string? RejectReason { get; set; }
    public string? VerifyResult { get; set; }
    public string? RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? PlannedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? CloseDate { get; set; }
    public int? DaysElapsed { get; set; }
    public string? OAFlowId { get; set; }
    public string? OANo { get; set; }
    public bool IsUrgent { get; set; }
    public decimal? CostEstimate { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Sub-table collections (loaded on demand)
    public List<EcnItemDto> Items { get; set; } = [];
    public List<EcnImpactItemDto> Impacts { get; set; } = [];
    public List<EcnApproverDto> Approvers { get; set; } = [];
    public List<EcnNotifyDeptDto> NotifyDepts { get; set; } = [];
    public List<EcnImplementDto> Implements { get; set; } = [];
}

public class CreateEcnRequest
{
    public string EcnTitle { get; set; } = string.Empty;
    public string? EcnType { get; set; }
    public string? ChangeCategory { get; set; }
    public string? Reason { get; set; }
    public string? ChangeDescription { get; set; }
    public string? ChangeContent { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? AffectedRoutes { get; set; }
    public string? AffectedProducts { get; set; }
    public string? ImpactAssessment { get; set; }
    public string? Urgency { get; set; }
    public string? RiskLevel { get; set; }
    public DateTime? PlannedDate { get; set; }
    public decimal? CostEstimate { get; set; }
    public bool IsUrgent { get; set; }
    public string? Remark { get; set; }
}

public class UpdateEcnRequest
{
    public string EcnId { get; set; } = string.Empty;
    public string? EcnTitle { get; set; }
    public string? Reason { get; set; }
    public string? ChangeDescription { get; set; }
    public string? ImpactAssessment { get; set; }
    public DateTime? PlannedDate { get; set; }
    public string? Remark { get; set; }
}

public class EcnAdvanceRequest
{
    public string EcnId { get; set; } = string.Empty;
    public string TargetStatus { get; set; } = string.Empty;
    public string? Comments { get; set; }
}

public class EcnApprovalRequest
{
    public string EcnId { get; set; } = string.Empty;
    public bool Approved { get; set; }
    public string? Comments { get; set; }
    public string? RejectReason { get; set; }
}

// ==================== 子表DTO：变更项 ====================

public class EcnItemDto
{
    public string ItemId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ChangeReason { get; set; }
    public string? Remark { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class CreateEcnItemRequest
{
    public string EcnId { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ChangeReason { get; set; }
    public string? Remark { get; set; }
    public int SortOrder { get; set; }
}

// ==================== 子表DTO：影响评估 ====================

public class EcnImpactItemDto
{
    public string ImpactId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string ImpactType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImpactAnalysis { get; set; }
    public string? Action { get; set; }
    public string? Responsible { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreateEcnImpactItemRequest
{
    public string EcnId { get; set; } = string.Empty;
    public string ImpactType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImpactAnalysis { get; set; }
    public string? Action { get; set; }
    public string? Responsible { get; set; }
    public DateTime? DueDate { get; set; }
}

// ==================== 子表DTO：审批人 ====================

public class EcnApproverDto
{
    public string ApproverId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string ApproverName { get; set; } = string.Empty;
    public string? Role { get; set; }
    public int ApprovalOrder { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Result { get; set; }
    public string? Comments { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEcnApproverRequest
{
    public string EcnId { get; set; } = string.Empty;
    public string ApproverName { get; set; } = string.Empty;
    public string? Role { get; set; }
    public int ApprovalOrder { get; set; }
}

public class UpdateEcnApproverRequest
{
    public string ApproverId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Result { get; set; }
    public string? Comments { get; set; }
}

// ==================== 子表DTO：通知部门 ====================

public class EcnNotifyDeptDto
{
    public string NotifyId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string DeptId { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
    public DateTime? NotifiedAt { get; set; }
    public string? ConfirmedBy { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEcnNotifyDeptRequest
{
    public string EcnId { get; set; } = string.Empty;
    public string DeptId { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
}

// ==================== 子表DTO：实施记录 ====================

public class EcnImplementDto
{
    public string ImplementId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Responsible { get; set; }
    public DateTime? PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Result { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class CreateEcnImplementRequest
{
    public string EcnId { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Responsible { get; set; }
    public DateTime? PlanDate { get; set; }
    public string? Remark { get; set; }
}

public class UpdateEcnImplementRequest
{
    public string ImplementId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? ActualDate { get; set; }
    public string? Result { get; set; }
    public string? Remark { get; set; }
}

// ==================== 统计DTO ====================

public class EcnStatisticsDto
{
    public int TotalCount { get; set; }
    public int DraftCount { get; set; }
    public int ReviewCount { get; set; }
    public int ApprovalCount { get; set; }
    public int ImplementCount { get; set; }
    public int VerifyCount { get; set; }
    public int ClosedCount { get; set; }
    public int RejectedCount { get; set; }
    public int UrgentCount { get; set; }
    public double CloseRate { get; set; }
    public double? AvgDaysToClose { get; set; }
    public Dictionary<string, int> TypeDistribution { get; set; } = [];
    public Dictionary<string, int> SeverityDistribution { get; set; } = [];
    public Dictionary<string, int> StatusDistribution { get; set; } = [];
}
