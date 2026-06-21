namespace MES.Infrastructure.Persistence.Entities;

// ==================== 订单评审 ====================

public class SalesOrder
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSpec { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalAmount { get; set; }
    public string Currency { get; set; } = "CNY";
    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string Priority { get; set; } = "Normal";
    public string Status { get; set; } = "Draft";
    public string ReviewStatus { get; set; } = "NotStarted";
    public string? ReviewResult { get; set; }
    public string? PackageType { get; set; }
    public string? LeadFrameType { get; set; }
    public string? WireType { get; set; }
    public string? SpecialRequirements { get; set; }
    public string QualityLevel { get; set; } = "Commercial";
    public string? Remark { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Deleted { get; set; }
}

public class OrderReview
{
    public string ReviewId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ReviewType { get; set; } = "Standard";
    public string Status { get; set; } = "Pending";
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? Deadline { get; set; }
    public string? InitiatedBy { get; set; }
    public string? Conclusion { get; set; }
    public string? Conditions { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class OrderReviewItem
{
    public long ItemId { get; set; }
    public string ReviewId { get; set; } = string.Empty;
    public string ReviewerRole { get; set; } = string.Empty;
    public string? ReviewerName { get; set; }
    public string? Vote { get; set; }
    public string? Comments { get; set; }
    public string? Conditions { get; set; }
    public DateTime? ReviewTime { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class OrderVersion
{
    public long VersionId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public int VersionNo { get; set; }
    public string? ChangeType { get; set; }
    public string? ChangeReason { get; set; }
    public string? ChangeDescription { get; set; }
    public string? OldData { get; set; }
    public string? NewData { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
}

// ==================== 主生产计划 ====================

public class MasterProductionPlan
{
    public string PlanId { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string PlanType { get; set; } = "MPS";
    public DateTime PlanPeriodStart { get; set; }
    public DateTime PlanPeriodEnd { get; set; }
    public string Status { get; set; } = "Draft";
    public int? TotalDemandQty { get; set; }
    public int? TotalCapacity { get; set; }
    public decimal? CapacityUtilization { get; set; }
    public bool BottleneckIdentified { get; set; }
    public string? BottleneckDescription { get; set; }
    public string? Planner { get; set; }
    public string? PlanData { get; set; }
    public string? Remark { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? PublishedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class CapacityLoad
{
    public long LoadId { get; set; }
    public string PlanId { get; set; } = string.Empty;
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string? EquipmentGroup { get; set; }
    public decimal? Uph { get; set; }
    public decimal? AvailableHours { get; set; }
    public decimal? RequiredHours { get; set; }
    public decimal? LoadRate { get; set; }
    public bool IsBottleneck { get; set; }
    public int? AvailableQty { get; set; }
    public int? RequiredQty { get; set; }
    public int? ShortageQty { get; set; }
    public string? ShiftPlan { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}

public class CapacitySimulation
{
    public string SimulationId { get; set; } = string.Empty;
    public string SimulationName { get; set; } = string.Empty;
    public string? BasePlanId { get; set; }
    public string? ScenarioDescription { get; set; }
    public string? ScenarioParams { get; set; }
    public int? TotalDemandQty { get; set; }
    public int? TotalCapacity { get; set; }
    public decimal? CapacityUtilization { get; set; }
    public int? BottleneckCount { get; set; }
    public string? ResultSummary { get; set; }
    public string? ResultData { get; set; }
    public string Status { get; set; } = "Completed";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ==================== BOM 与 MRP ====================

public class Bom
{
    public string BomId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string BomVersion { get; set; } = "1.0";
    public string Status { get; set; } = "Active";
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int TotalItems { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class BomItem
{
    public long ItemId { get; set; }
    public string BomId { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? MaterialSpec { get; set; }
    public decimal QuantityPerUnit { get; set; }
    public string? Unit { get; set; }
    public decimal LossRate { get; set; }
    public string? SubstituteMaterials { get; set; }
    public int SortOrder { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MrpCalculation
{
    public string CalculationId { get; set; } = string.Empty;
    public string? PlanId { get; set; }
    public string? WorkOrderId { get; set; }
    public string CalculationType { get; set; } = "Full";
    public string Status { get; set; } = "Completed";
    public int? TotalDemandItems { get; set; }
    public int? ShortageItems { get; set; }
    public int? SufficientItems { get; set; }
    public string? CalculationParams { get; set; }
    public string? ResultSummary { get; set; }
    public string? ResultData { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MrpShortageWarning
{
    public long WarningId { get; set; }
    public string CalculationId { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public decimal RequiredQty { get; set; }
    public decimal AvailableQty { get; set; }
    public decimal ShortageQty { get; set; }
    public DateTime? ExpectedArrival { get; set; }
    public string? PurchaseOrderNo { get; set; }
    public string Severity { get; set; } = "Medium";
    public string Status { get; set; } = "Open";
    public string? ResolutionNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ==================== 订单进度与 OTD ====================

public class OrderProgressSnapshot
{
    public long SnapshotId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public DateTime SnapshotTime { get; set; }
    public int TotalQuantity { get; set; }
    public int CompletedQuantity { get; set; }
    public int InProgressQuantity { get; set; }
    public int PendingQuantity { get; set; }
    public int DefectiveQuantity { get; set; }
    public decimal? YieldRate { get; set; }
    public decimal? ProgressPercentage { get; set; }
    public string? CurrentStage { get; set; }
    public DateTime? EstimatedCompletion { get; set; }
    public bool IsDelayed { get; set; }
    public int DelayDays { get; set; }
    public int WorkOrderCount { get; set; }
    public int CompletedWorkOrderCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class OtdStatistics
{
    public long StatId { get; set; }
    public string StatPeriod { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public int OnTimeOrders { get; set; }
    public int LateOrders { get; set; }
    public decimal? OtdRate { get; set; }
    public decimal? AvgDelayDays { get; set; }
    public int? MaxDelayDays { get; set; }
    public int TotalQuantity { get; set; }
    public int OnTimeQuantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class DelayReasonRecord
{
    public long RecordId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string? WorkOrderId { get; set; }
    public string DelayReasonCategory { get; set; } = string.Empty;
    public string? DelayReasonDetail { get; set; }
    public int DelayDays { get; set; }
    public int? ImpactQuantity { get; set; }
    public string? ResponsibleDept { get; set; }
    public string? CorrectiveAction { get; set; }
    public string? PreventiveAction { get; set; }
    public string? ReportedBy { get; set; }
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
}

// ==================== 插单处理 ====================

public class RushOrderRequest
{
    public string RequestId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int RushQuantity { get; set; }
    public DateTime RequiredDate { get; set; }
    public string? RushReason { get; set; }
    public string PriorityLevel { get; set; } = "Urgent";
    public string Status { get; set; } = "Pending";
    public bool ImpactAnalysisDone { get; set; }
    public string? AnalysisSummary { get; set; }
    public string? ApprovalResult { get; set; }
    public string? ApprovalBy { get; set; }
    public DateTime? ApprovalAt { get; set; }
    public string? ApprovalComments { get; set; }
    public string? ExecutedBy { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class RushOrderImpact
{
    public long ImpactId { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public string AffectedOrderId { get; set; } = string.Empty;
    public string? AffectedWorkOrderId { get; set; }
    public string? ImpactType { get; set; }
    public string? ImpactDescription { get; set; }
    public DateTime? OriginalDeliveryDate { get; set; }
    public DateTime? NewEstimatedDelivery { get; set; }
    public int? DelayDays { get; set; }
    public string? MaterialShortageItems { get; set; }
    public string? CapacityConflictDetails { get; set; }
    public string Severity { get; set; } = "Medium";
    public string? MitigationPlan { get; set; }
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}
