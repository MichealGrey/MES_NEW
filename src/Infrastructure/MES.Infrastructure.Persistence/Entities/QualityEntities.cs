namespace MES.Infrastructure.Persistence.Entities;

public class IqcIncomingBatch
{
    public string BatchId { get; set; } = string.Empty;
    public string PoNumber { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? MaterialSpec { get; set; }
    public int ReceivedQty { get; set; }
    public string? Unit { get; set; }
    public string? LotNumber { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public string? WarehouseId { get; set; }
    public string Status { get; set; } = "Pending";
    public string? InspectorId { get; set; }
    public string? InspectorName { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class IqcInspectionTask
{
    public string TaskId { get; set; } = string.Empty;
    public string BatchId { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string SupplierId { get; set; } = string.Empty;
    public string InspectionType { get; set; } = string.Empty;
    public int InspectionQty { get; set; }
    public int SampleSize { get; set; }
    public string? SamplingPlan { get; set; }
    public string? AqlLevel { get; set; }
    public string Status { get; set; } = "Pending";
    public string? InspectorId { get; set; }
    public string? InspectorName { get; set; }
    public DateTime? AssignedTime { get; set; }
    public DateTime? CompletedTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class IqcInspectionResult
{
    public string ResultId { get; set; } = string.Empty;
    public string TaskId { get; set; } = string.Empty;
    public string BatchId { get; set; } = string.Empty;
    public string StandardId { get; set; } = string.Empty;
    public string InspectionItem { get; set; } = string.Empty;
    public string? InspectionMethod { get; set; }
    public string? StandardValue { get; set; }
    public string? UpperLimit { get; set; }
    public string? LowerLimit { get; set; }
    public string? ActualValue { get; set; }
    public string? Unit { get; set; }
    public string Judgment { get; set; } = string.Empty;
    public string? DefectCode { get; set; }
    public string? DefectDescription { get; set; }
    public string? InspectorId { get; set; }
    public string? InspectorName { get; set; }
    public DateTime InspectionTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class IqcInspectionStandard
{
    public string StandardId { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string InspectionItem { get; set; } = string.Empty;
    public string? InspectionMethod { get; set; }
    public string? StandardValue { get; set; }
    public string? UpperLimit { get; set; }
    public string? LowerLimit { get; set; }
    public string? Unit { get; set; }
    public string? SamplingPlan { get; set; }
    public string? AqlLevel { get; set; }
    public int SamplingQty { get; set; }
    public string InspectionType { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string? Version { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class IqcSupplierQualityStat
{
    public long Id { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string? MaterialId { get; set; }
    public int TotalBatches { get; set; }
    public int PassBatches { get; set; }
    public int FailBatches { get; set; }
    public int ReturnBatches { get; set; }
    public decimal? PassRate { get; set; }
    public int TotalDefects { get; set; }
    public string? TopDefectType { get; set; }
    public string? EvaluationPeriod { get; set; }
    public string QualityGrade { get; set; } = string.Empty;
    public DateTime StatDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class FqcInspectionRecord
{
    public string RecordId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? RouteId { get; set; }
    public string? StepCode { get; set; }
    public int InspectionQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public string Judgment { get; set; } = string.Empty;
    public string? InspectorId { get; set; }
    public string? InspectorName { get; set; }
    public DateTime InspectionTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class OqcInspectionRecord
{
    public string RecordId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int InspectionQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public string Judgment { get; set; } = string.Empty;
    public string? InspectorId { get; set; }
    public string? InspectorName { get; set; }
    public DateTime InspectionTime { get; set; }
    public string? PackagingCheck { get; set; }
    public string? LabelCheck { get; set; }
    public string? MslCheck { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ShipmentMslCheck
{
    public string CheckId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string? MslLevel { get; set; }
    public DateTime? BakeStartTime { get; set; }
    public DateTime? BakeEndTime { get; set; }
    public int? BakeDurationHours { get; set; }
    public DateTime? ExposureStartTime { get; set; }
    public int? ExposureDurationHours { get; set; }
    public int? FloorLifeRemaining { get; set; }
    public string HumidityCardResult { get; set; } = string.Empty;
    public string PackagingCondition { get; set; } = string.Empty;
    public string Judgment { get; set; } = string.Empty;
    public string? CheckerId { get; set; }
    public string? CheckerName { get; set; }
    public DateTime CheckTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class NonconformingRecord
{
    public string NcrId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string? OrderId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public string? StepCode { get; set; }
    public int DefectQty { get; set; }
    public string? DefectCode { get; set; }
    public string DefectDescription { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Disposition { get; set; } = string.Empty;
    public string? MrbReference { get; set; }
    public string Status { get; set; } = "Open";
    public string? ReporterId { get; set; }
    public string? ReporterName { get; set; }
    public DateTime ReportTime { get; set; }
    public string? ResponsibleDept { get; set; }
    public string? RootCause { get; set; }
    public string? CorrectiveAction { get; set; }
    public string? PreventiveAction { get; set; }
    public string? ClosedBy { get; set; }
    public DateTime? ClosedTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class MrbReview
{
    public string MrbId { get; set; } = string.Empty;
    public string? NcrId { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public int AffectedQty { get; set; }
    public string Disposition { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? ReviewType { get; set; }
    public DateTime? ReviewTime { get; set; }
    public string? ReviewerIds { get; set; }
    public string? ReviewerNames { get; set; }
    public string? ReviewConclusion { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class MrbReviewItem
{
    public string ItemId { get; set; } = string.Empty;
    public string MrbId { get; set; } = string.Empty;
    public string NcrId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public int AffectedQty { get; set; }
    public string Disposition { get; set; } = string.Empty;
    public string? DispositionDetail { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class DispositionRecord
{
    public string DispositionId { get; set; } = string.Empty;
    public string? MrbId { get; set; }
    public string? NcrId { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string DispositionType { get; set; } = string.Empty;
    public int DispositionQty { get; set; }
    public string? ExecutionDetail { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ExecutorId { get; set; }
    public string? ExecutorName { get; set; }
    public DateTime? ExecutionTime { get; set; }
    public string? VerifierId { get; set; }
    public string? VerifierName { get; set; }
    public DateTime? VerificationTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
