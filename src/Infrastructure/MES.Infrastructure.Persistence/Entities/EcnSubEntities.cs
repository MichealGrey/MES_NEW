namespace MES.Infrastructure.Persistence.Entities;

/// <summary>
/// ECN变更项
/// </summary>
public class EcnItem
{
    public string ItemId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty; // 物料/工艺/文件/BOM/软件
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? ChangeReason { get; set; }
    public string? Remark { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// ECN影响评估项
/// </summary>
public class EcnImpactItem
{
    public string ImpactId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string ImpactType { get; set; } = string.Empty; // 质量/成本/交期/库存/设备/人员
    public string Severity { get; set; } = "Medium"; // Low/Medium/High/Critical
    public string? Description { get; set; }
    public string? ImpactAnalysis { get; set; }
    public string? Action { get; set; }
    public string? Responsible { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
}

/// <summary>
/// ECN审批人
/// </summary>
public class EcnApprover
{
    public string ApproverId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string ApproverName { get; set; } = string.Empty;
    public string? Role { get; set; } // 角色：申请人/审核人/批准人
    public int ApprovalOrder { get; set; } // 审批顺序
    public string Status { get; set; } = "Pending"; // Pending/Approved/Rejected/Skipped
    public string? Result { get; set; }
    public string? Comments { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// ECN通知部门
/// </summary>
public class EcnNotifyDept
{
    public string NotifyId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string DeptId { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
    public DateTime? NotifiedAt { get; set; }
    public string? ConfirmedBy { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// ECN实施记录
/// </summary>
public class EcnImplement
{
    public string ImplementId { get; set; } = string.Empty;
    public string EcnId { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Responsible { get; set; }
    public DateTime? PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending/InProgress/Completed/Blocked
    public string? Result { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
