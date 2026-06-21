using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MES.Modules.EngineeringChange.Models;

/// <summary>
/// Main ECN model matching backend EcnRequestDto
/// </summary>
public class ECNInfo
{
    public string EcnId { get; set; } = string.Empty;
    public string EcnNo { get; set; } = string.Empty;
    public string EcnTitle { get; set; } = string.Empty;
    public string EcnType { get; set; } = string.Empty;
    public string ChangeCategory { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string ChangeDescription { get; set; } = string.Empty;
    public string ChangeContent { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AffectedRoutes { get; set; } = string.Empty;
    public string AffectedProducts { get; set; } = string.Empty;
    public string ImpactAssessment { get; set; } = string.Empty;
    public string Urgency { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public string ReviewComments { get; set; } = string.Empty;
    public string RejectReason { get; set; } = string.Empty;
    public string VerifyResult { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? PlannedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? CloseDate { get; set; }
    public int? DaysElapsed { get; set; }
    public string OAFlowId { get; set; } = string.Empty;
    public string OANo { get; set; } = string.Empty;
    public bool IsUrgent { get; set; }
    public decimal? CostEstimate { get; set; }
    public string Remark { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;

    // Sub-table collections
    public List<EcnItem> Items { get; set; } = new();
    public List<EcnImpact> Impacts { get; set; } = new();
    public List<EcnApprover> Approvers { get; set; } = new();
    public List<EcnNotifyDept> NotifyDepts { get; set; } = new();
    public List<EcnImplement> Implements { get; set; } = new();
}

/// <summary>
/// ECN item matching backend EcnItemDto
/// </summary>
public class EcnItem
{
    public string ItemId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public string ChangeReason { get; set; } = string.Empty;
    public string Remark { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}

/// <summary>
/// ECN impact matching backend EcnImpactItemDto
/// </summary>
public class EcnImpact
{
    public string ImpactId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string ImpactType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImpactAnalysis { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

/// <summary>
/// ECN approver matching backend EcnApproverDto
/// </summary>
public class EcnApprover
{
    public string ApproverId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string ApproverName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int ApprovalOrder { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// ECN notify department matching backend EcnNotifyDeptDto
/// </summary>
public class EcnNotifyDept
{
    public string NotifyId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string DeptId { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
    public DateTime? NotifiedAt { get; set; }
    public string ConfirmedBy { get; set; } = string.Empty;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// ECN implement matching backend EcnImplementDto
/// </summary>
public class EcnImplement
{
    public string ImplementId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public DateTime? PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Remark { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}

/// <summary>
/// ECN history for timeline display (kept as is)
/// </summary>
public class ECNHistory
{
    public string ECNId { get; set; } = string.Empty;
    public string ECNNo { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ActionBy { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; } = DateTime.Now;
    public string Comment { get; set; } = string.Empty;
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
}

/// <summary>
/// ECN statistics matching backend EcnStatisticsDto
/// </summary>
public class EcnStatistics
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
    public double AvgDaysToClose { get; set; }
    public Dictionary<string, int> TypeDistribution { get; set; } = new();
    public Dictionary<string, int> SeverityDistribution { get; set; } = new();
    public Dictionary<string, int> StatusDistribution { get; set; } = new();
}
