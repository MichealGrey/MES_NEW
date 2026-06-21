namespace MES.Infrastructure.Persistence.Entities;

public class SysDepartment
{
    public string DeptId { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? ManagerId { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SysRole
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Level { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SysUser
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public string DeptId { get; set; } = string.Empty;
    public string? Shift { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class SysUserPermission
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SysPermissionConfirm
{
    public long Id { get; set; }
    public string OperationType { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string? EmployeeName { get; set; }
    public string RequiredLevel { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Reason { get; set; }
    public DateTime ConfirmAt { get; set; } = DateTime.UtcNow;
}

public class SysSignatureLevel
{
    public string LevelCode { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public int LevelOrder { get; set; }
    public string? Description { get; set; }
}

public class ExtSystemEvent
{
    public string EventId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = "MES";
    public string TargetSystem { get; set; } = string.Empty;
    public string? Payload { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
}

public class ExtSystemConfig
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public string SystemType { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string AuthType { get; set; } = "None";
    public string? AuthCredential { get; set; }
    public bool IsEnabled { get; set; } = true;
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
    public string? SubscribedEvents { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CustomerRequirement
{
    public string RequirementId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? OrderId { get; set; }
    public string? ProductId { get; set; }
    public string RequirementType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
    public bool IsMandatory { get; set; }
    public string? VerificationMethod { get; set; }
    public string Status { get; set; } = "Active";
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class MasterMaterial
{
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string Unit { get; set; } = "pcs";
    public string? Supplier { get; set; }
    public int MinStock { get; set; }
    public int CurrentStock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MaterialRequirement
{
    public string RequirementId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public decimal RequiredQty { get; set; }
    public string Unit { get; set; } = "pcs";
    public bool IsMandatory { get; set; } = true;
}

public class MaterialConsumeEntity
{
    public string ConsumeId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public decimal ConsumedQty { get; set; }
    public string Unit { get; set; } = "pcs";
    public string? BatchNo { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime ConsumedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class QualityGateEntity
{
    public string GateId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string GateType { get; set; } = "QACheck";
    public string Status { get; set; } = "Pending";
    public string? CheckedBy { get; set; }
    public string? CheckedByName { get; set; }
    public DateTime? CheckedAt { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpireAt { get; set; }
}

// ============================================================================
// V3.0.0 - Permission System & New Feature Entities
// ============================================================================

public class SysMenu
{
    public string MenuId { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? Icon { get; set; }
    public string? ViewName { get; set; }
    public string? ModuleKey { get; set; }
    public string? PermissionCode { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SysUserRole
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SysRoleMenu
{
    public long Id { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public string MenuId { get; set; } = string.Empty;
}

public class SysRolePermission
{
    public long Id { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
}

public class SysLoginLog
{
    public long Id { get; set; }
    public string? UserId { get; set; }
    public string? EmployeeId { get; set; }
    public DateTime? LoginTime { get; set; }
    public string? IpAddress { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
}

public class Complaint8D
{
    public string ComplaintId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? OrderNo { get; set; }
    public string? CustomerPONO { get; set; }
    public string? LotId { get; set; }
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? DefectType { get; set; }
    public string? Severity { get; set; }
    public string? Priority { get; set; } = "Normal";
    public string Status { get; set; } = "Open";
    public string EightDStatus { get; set; } = "D0";
    public int AffectedQty { get; set; }
    public int ReturnQty { get; set; }
    public int SampleQty { get; set; }

    // D0 - 准备
    public bool? D0Assessment { get; set; }
    public string? D0AssessmentComment { get; set; }
    public DateTime? D0Date { get; set; }

    // D1 - 团队
    public string? D1TeamMembers { get; set; }
    public DateTime? D1Date { get; set; }

    // D2 - 问题描述 (5W2H)
    public string? D2ProblemDescription { get; set; }
    public string? D2What { get; set; }
    public string? D2Who { get; set; }
    public string? D2Where { get; set; }
    public string? D2When { get; set; }
    public string? D2Why { get; set; }
    public string? D2How { get; set; }
    public string? D2HowMany { get; set; }
    public string? D2DefectLocation { get; set; }
    public DateTime? D2OccurrenceDate { get; set; }
    public DateTime? D2DiscoveryDate { get; set; }
    public string? D2DiscoveryMethod { get; set; }
    public DateTime? D2Date { get; set; }

    // D3 - 围堵措施
    public string? D3ContainmentAction { get; set; }
    public string? D3ContainmentResult { get; set; }
    public DateTime? D3ContainmentDate { get; set; }
    public DateTime? D3Date { get; set; }

    // D4 - 原因分析
    public string? D4RootCause { get; set; }
    public string? D4AnalysisMethod { get; set; }
    public string? D4OccurrenceCause { get; set; }
    public string? D4EscapeCause { get; set; }
    public DateTime? D4Date { get; set; }

    // D5 - 纠正措施
    public string? D5PermanentAction { get; set; }
    public string? D5ActionValidation { get; set; }
    public DateTime? D5ValidationDate { get; set; }
    public DateTime? D5Date { get; set; }

    // D6 - 实施验证
    public string? D6Implementation { get; set; }
    public string? D6VerificationResult { get; set; }
    public DateTime? D6ImplementDate { get; set; }
    public DateTime? D6Date { get; set; }

    // D7 - 预防措施
    public string? D7Prevention { get; set; }
    public string? D7DocUpdateList { get; set; }
    public string? D7Standardization { get; set; }
    public string? D7HorizontalExpand { get; set; }
    public DateTime? D7Date { get; set; }

    // D8 - 结案
    public string? D8ClosureComment { get; set; }
    public string? D8TeamRecognition { get; set; }
    public string? D8EffectivenessConfirm { get; set; }
    public DateTime? D8Date { get; set; }

    // 审批
    public string? ApprovalStatus { get; set; } = "Pending";
    public string? Approver { get; set; }
    public DateTime? ApproveDate { get; set; }
    public string? ApprovalComment { get; set; }

    // 通用
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    public string? ClosedBy { get; set; }
    public DateTime? DueDate { get; set; }
    public int OverdueDays { get; set; }
    public string? Attachments { get; set; }
    public string? Remark { get; set; }
}

public class EcnRequest
{
    public string EcnId { get; set; } = string.Empty;
    public string EcnNo { get; set; } = string.Empty;
    public string? EcnTitle { get; set; }
    public string? EcnType { get; set; } // 设计变更/工艺变更/材料变更/规格变更/软件变更/BOM变更
    public string? ChangeCategory { get; set; } // 永久变更/临时变更/紧急变更
    public string? Reason { get; set; }
    public string? ChangeDescription { get; set; }
    public string? ChangeContent { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Status { get; set; } = "Draft"; // Draft/Review/Approval/Implement/Verify/Close/Rejected
    public string? AffectedRoutes { get; set; }
    public string? AffectedProducts { get; set; }
    public string? ImpactAssessment { get; set; }
    public string? Urgency { get; set; } // 一般/紧急/特急
    public string? RiskLevel { get; set; } // Low/Medium/High/Critical
    public string? ReviewComments { get; set; }
    public string? RejectReason { get; set; }
    public string? VerifyResult { get; set; }
    public string? RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class NpiProject
{
    public string ProjectId { get; set; } = string.Empty;
    public string? ProjectName { get; set; }
    public string? CustomerId { get; set; }
    public string? ProductId { get; set; }
    public string? Status { get; set; }
    public string? Phase { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? TargetCompletion { get; set; }
    public DateTime? ActualCompletion { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SpcMeasurement
{
    public long Id { get; set; }
    public string? LotId { get; set; }
    public string? StepCode { get; set; }
    public string? ParameterName { get; set; }
    public decimal? MeasuredValue { get; set; }
    public decimal? Usl { get; set; }
    public decimal? Lsl { get; set; }
    public decimal? TargetValue { get; set; }
    public string? EquipmentId { get; set; }
    public string? OperatorId { get; set; }
    public DateTime? MeasuredAt { get; set; }
    public bool IsOutOfControl { get; set; }
}

public class ShiftSchedule
{
    public long Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public DateTime ShiftDate { get; set; }
    public string? ShiftType { get; set; }
    public string? DepartmentId { get; set; }
    public string? Workshop { get; set; }
}

// ============================================================================
// Phase 3.4 - Quality Inspection Entities
// ============================================================================

/// <summary>
/// 质量检验记录（IQC/IPQC/OQC）
/// </summary>
public class QualityInspection
{
    public string InspectionId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string InspectionType { get; set; } = string.Empty; // IQC/IPQC/OQC
    public string InspectorId { get; set; } = string.Empty;
    public string? InspectorName { get; set; }
    public string Result { get; set; } = "Pass"; // Pass/Fail/Conditional
    public string? Detail { get; set; }
    public string? Remark { get; set; }
    public DateTime InspectionTime { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 检验项目明细
/// </summary>
public class QualityInspectionItem
{
    public long Id { get; set; }
    public string InspectionId { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public decimal? Usl { get; set; }
    public decimal? Lsl { get; set; }
    public decimal? TargetValue { get; set; }
    public decimal? MeasuredValue { get; set; }
    public string Result { get; set; } = "Pass"; // Pass/Fail
    public string? Unit { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 不合格品报告（NCR）
/// </summary>
public class NonConformanceReport
{
    public string NcrId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string DefectType { get; set; } = string.Empty;
    public string? DefectDescription { get; set; }
    public int Quantity { get; set; }
    public string Severity { get; set; } = "Minor"; // Critical/Major/Minor
    public string Status { get; set; } = "Open"; // Open/UnderReview/Dispositioned/Closed
    public string? Disposition { get; set; } // Rework/Scrap/Return/UseAsIs
    public string? DispositionDetail { get; set; }
    public string DiscoveredBy { get; set; } = string.Empty;
    public DateTime DiscoveredAt { get; set; } = DateTime.UtcNow;
    public string DiscovererId { get; set; } = string.Empty;
    public string? DiscovererName { get; set; }
    public DateTime DiscoveredTime { get; set; } = DateTime.UtcNow;
    public string? ReviewerId { get; set; }
    public string? ReviewerName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? CloserId { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? ClosureComment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
