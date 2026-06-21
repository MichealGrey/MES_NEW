namespace MES.Infrastructure.Persistence.Entities;

/// <summary>
/// 异常记录
/// </summary>
public class AbnormalRecord
{
    /// <summary>
    /// 异常ID
    /// </summary>
    public string AbnormalId { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string? OrderId { get; set; }
    public string? StepCode { get; set; }
    public string? EquipmentId { get; set; }
    public string AbnormalType { get; set; } = string.Empty;
    public string AbnormalCategory { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public string? ReporterId { get; set; }
    public string? ReporterName { get; set; }
    public DateTime ReportTime { get; set; }
    public string? ResponsibleDept { get; set; }
    public string? ResponsiblePerson { get; set; }
    public string? RootCause { get; set; }
    public string? CorrectiveAction { get; set; }
    public string? PreventiveAction { get; set; }
    public string? ClosedBy { get; set; }
    public DateTime? ClosedTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 停线记录
/// </summary>
public class LineStopRecord
{
    /// <summary>
    /// 停线ID
    /// </summary>
    public string StopId { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string? LineId { get; set; }
    public string? EquipmentId { get; set; }
    public string StopType { get; set; } = string.Empty;
    public string StopReason { get; set; } = string.Empty;
    public string? ReasonCode { get; set; }
    public string Severity { get; set; } = string.Empty;
    public DateTime StopTime { get; set; }
    public DateTime? ResumeTime { get; set; }
    public int? StopDurationMinutes { get; set; }
    public string? ReportedBy { get; set; }
    public string? ReportedByName { get; set; }
    public string? ResponsibleDept { get; set; }
    public string? Action { get; set; }
    public string Status { get; set; } = "Open";
    public string? ClosedBy { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 设备故障记录
/// </summary>
public class EquipmentFaultRecord
{
    /// <summary>
    /// 故障ID
    /// </summary>
    public string FaultId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string? EquipmentName { get; set; }
    public string FaultType { get; set; } = string.Empty;
    public string FaultDescription { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime FaultTime { get; set; }
    public DateTime? RepairStartTime { get; set; }
    public DateTime? RepairEndTime { get; set; }
    public int? RepairDurationMinutes { get; set; }
    public string? ReportedBy { get; set; }
    public string? ReportedByName { get; set; }
    public string? RepairBy { get; set; }
    public string? RepairByName { get; set; }
    public string? RootCause { get; set; }
    public string? RepairAction { get; set; }
    public string? ReplacedParts { get; set; }
    public string Status { get; set; } = "Open";
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 设备维修备件记录
/// </summary>
public class EquipmentRepairSparePart
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    public string FaultId { get; set; } = string.Empty;
    public string SparePartId { get; set; } = string.Empty;
    public string SparePartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public string? WarehouseId { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 设备保养计划
/// </summary>
public class EquipmentPmPlan
{
    /// <summary>
    /// 保养计划ID
    /// </summary>
    public string PmPlanId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string? EquipmentName { get; set; }
    public string PmType { get; set; } = string.Empty;
    public string PmName { get; set; } = string.Empty;
    public string? PmDescription { get; set; }
    public int? CycleDays { get; set; }
    public string? CycleType { get; set; }
    public DateTime? NextDueDate { get; set; }
    public string Status { get; set; } = "Active";
    public string? CreatedBy { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 设备保养执行记录
/// </summary>
public class EquipmentPmExecution
{
    /// <summary>
    /// 执行ID
    /// </summary>
    public string PmExecutionId { get; set; } = string.Empty;
    public string PmPlanId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string? EquipmentName { get; set; }
    public string PmType { get; set; } = string.Empty;
    public string? PmContent { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public int? ActualDurationMinutes { get; set; }
    public string? ExecutedBy { get; set; }
    public string? ExecutedByName { get; set; }
    public string Result { get; set; } = string.Empty;
    public string? IssuesFound { get; set; }
    public string? FollowUpAction { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 首件检验
/// </summary>
public class FirstArticleInspection
{
    /// <summary>
    /// 首件检验ID
    /// </summary>
    public string FaId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? RouteId { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string? StepName { get; set; }
    public string? EquipmentId { get; set; }
    public int InspectionQty { get; set; }
    public string Judgment { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? InspectorId { get; set; }
    public string? InspectorName { get; set; }
    public DateTime? InspectionTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 首件检验项目
/// </summary>
public class FirstArticleInspectionItem
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    public string FaId { get; set; } = string.Empty;
    public string InspectionItem { get; set; } = string.Empty;
    public string? InspectionMethod { get; set; }
    public string? StandardValue { get; set; }
    public string? UpperLimit { get; set; }
    public string? LowerLimit { get; set; }
    public string? ActualValue { get; set; }
    public string? Unit { get; set; }
    public string Judgment { get; set; } = string.Empty;
    public string? DefectDescription { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 首件签名
/// </summary>
public class FirstArticleSignature
{
    /// <summary>
    /// 签名ID
    /// </summary>
    public string SignatureId { get; set; } = string.Empty;
    public string FaId { get; set; } = string.Empty;
    public string SignerId { get; set; } = string.Empty;
    public string SignerName { get; set; } = string.Empty;
    public string SignerRole { get; set; } = string.Empty;
    public string SignatureType { get; set; } = string.Empty;
    public string? SignatureData { get; set; }
    public DateTime SignTime { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 焊线拉力测试记录
/// </summary>
public class BondPullTestRecord
{
    /// <summary>
    /// 测试ID
    /// </summary>
    public string TestId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string? OrderId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public string? StepCode { get; set; }
    public string? EquipmentId { get; set; }
    public string? BondWireType { get; set; }
    public int SampleSize { get; set; }
    public string? StandardValue { get; set; }
    public string? UpperLimit { get; set; }
    public string? LowerLimit { get; set; }
    public string? Unit { get; set; }
    public decimal? AvgValue { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public string Judgment { get; set; } = string.Empty;
    public string? TesterId { get; set; }
    public string? TesterName { get; set; }
    public DateTime TestTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 质量警报
/// </summary>
public class QualityAlert
{
    /// <summary>
    /// 警报ID
    /// </summary>
    public string AlertId { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? LotId { get; set; }
    public string? SupplierId { get; set; }
    public string? NcrId { get; set; }
    public string Status { get; set; } = "Active";
    public string? IssuedBy { get; set; }
    public string? IssuedByName { get; set; }
    public DateTime? IssuedTime { get; set; }
    public string? ClosedBy { get; set; }
    public DateTime? ClosedTime { get; set; }
    public string? CorrectiveAction { get; set; }
    public string? PreventiveAction { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 质量警报影响批次
/// </summary>
public class QualityAlertAffectedLot
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    public string AlertId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string? OrderId { get; set; }
    public string? ProductId { get; set; }
    public int? AffectedQty { get; set; }
    public string? Disposition { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 召回通知
/// </summary>
public class RecallNotice
{
    /// <summary>
    /// 召回ID
    /// </summary>
    public string RecallId { get; set; } = string.Empty;
    public string NoticeNumber { get; set; } = string.Empty;
    public string RecallType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime? RecallStartDate { get; set; }
    public DateTime? RecallEndDate { get; set; }
    public int TotalAffectedQty { get; set; }
    public int RecalledQty { get; set; }
    public string Status { get; set; } = "Open";
    public string? IssuedBy { get; set; }
    public string? IssuedByName { get; set; }
    public DateTime? IssuedTime { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 召回通知明细
/// </summary>
public class RecallNoticeItem
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    public string RecallId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public int AffectedQty { get; set; }
    public int RecalledQty { get; set; }
    public string? WarehouseId { get; set; }
    public string? CustomerId { get; set; }
    public string? ShipmentId { get; set; }
    public string Disposition { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
