namespace MES.Infrastructure.Persistence.Entities;

public class ProdWorkOrder
{
    public string OrderId { get; set; } = string.Empty;
    public string? ParentOrderId { get; set; }
    public string? WoType { get; set; } // Parent, Child-Assemble, Child-Test
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public string? DieName { get; set; }
    public string PackageType { get; set; } = string.Empty;
    public int PlannedQty { get; set; }
    public int CompletedQty { get; set; }
    public int WaferQty { get; set; }
    public int UnitQty { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPn { get; set; }
    public string? InternalPn { get; set; }
    public string Priority { get; set; } = "Normal";
    public string Status { get; set; } = "Created";
    public string Creator { get; set; } = string.Empty;
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public decimal? TargetCpYield { get; set; }
    public decimal? TargetFtYield { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ProdLot
{
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? DieName { get; set; }
    public string PackageType { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = "1.0";
    public int CurrentStepSeq { get; set; }
    public string? CurrentStepCode { get; set; }
    public string Status { get; set; } = "Waiting";
    public string? ProcessStage { get; set; }
    public int UnitCount { get; set; }
    public int StripCount { get; set; }
    public string Priority { get; set; } = "Normal";
    public string? CarrierType { get; set; }
    public string? CarrierId { get; set; }
    public string? WaferLotId { get; set; }
    public int OriginalQty { get; set; }
    public int TotalPassQty { get; set; }
    public int TotalScrapQty { get; set; }
    public int TotalReworkQty { get; set; }
    public int TotalHoldQty { get; set; }
    public bool IsPartialLot { get; set; }
    public string? MotherLotId { get; set; }
    public string? SplitReason { get; set; }
    public DateTime? SplitTime { get; set; }
    public int? SplitQty { get; set; }
    public bool IsReworkLot { get; set; }
    public string? OriginalRouteId { get; set; }
    public string? ReworkRouteId { get; set; }
    public int? ReworkCount { get; set; }
    public string? ReworkReason { get; set; }
    public bool IsUnderMrb { get; set; }
    public string? MrbReference { get; set; }
    public string? MrbDisposition { get; set; }
    public string? Grade { get; set; }
    public string? OriginalLotId { get; set; }
    public string? BinResult { get; set; }
    public string? TestResult { get; set; }
    public int QtyPass { get; set; }
    public int QtyFail { get; set; }
    public string? HoldCategory { get; set; }
    public string? HoldReason { get; set; }
    public DateTime? HoldTime { get; set; }
    public string? HoldOperator { get; set; }
    public string? ReleaseCondition { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ProdLotArchive
{
    public long Id { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public string? RouteId { get; set; }
    public string? ProcessStage { get; set; }
    public string Status { get; set; } = "Completed";
    public int OriginalQty { get; set; }
    public int TotalPassQty { get; set; }
    public int TotalScrapQty { get; set; }
    public int TotalReworkQty { get; set; }
    public int TotalHoldQty { get; set; }
    public decimal FinalYield { get; set; }
    public string? Grade { get; set; }
    public DateTime CompletedAt { get; set; }
    public DateTime ArchivedAt { get; set; } = DateTime.UtcNow;
    public string? ArchivedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ProdLotStep
{
    public string RecordId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = "1.0";
    public int StepSeq { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string Status { get; set; } = "Waiting";
    public string? TrackInEquipment { get; set; }
    public string? TrackInCarrier { get; set; }
    public string? TrackInRecipe { get; set; }
    public DateTime? TrackInTime { get; set; }
    public string? TrackInOperator { get; set; }
    public DateTime? TrackOutTime { get; set; }
    public string? TrackOutOperator { get; set; }
    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public int ScrapQty { get; set; }
    public int ReworkQty { get; set; }
    public int HoldQty { get; set; }
    public int PendingQty { get; set; }
    public string? RecipeId { get; set; }
    public string? TestProgram { get; set; }
    public string? BinSummary { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProdOperationHistory
{
    public long Id { get; set; }
    public string OperationId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string? OrderId { get; set; }
    public string OperationType { get; set; } = string.Empty;
    public string? StepCode { get; set; }
    public int? StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string? CarrierId { get; set; }
    public string? RecipeId { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public string? OperatorName { get; set; }
    public int? InputQty { get; set; }
    public int? OutputQty { get; set; }
    public int? ScrapQty { get; set; }
    public string? Detail { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProdAuditTrail
{
    public string AuditId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public string? OperatorName { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? BeforeState { get; set; }
    public string? AfterState { get; set; }
    public string? Reason { get; set; }
    public string? Detail { get; set; }
    public int SignatureLevel { get; set; }
    public string? ApprovedBy { get; set; }
}

public class ProdHoldRecord
{
    public string HoldId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string HoldType { get; set; } = string.Empty;
    public string? HoldReasonCode { get; set; }
    public string HoldReason { get; set; } = string.Empty;
    public int HoldQty { get; set; }
    public string? ResponsibleDept { get; set; }
    public string? Owner { get; set; }
    public string Status { get; set; } = "Open";
    public string HoldBy { get; set; } = string.Empty;
    public DateTime HoldTime { get; set; }
    public string? RootCause { get; set; }
    public string? CorrectiveAction { get; set; }
    public string? Disposition { get; set; }
    public string? ReleaseBy { get; set; }
    public DateTime? ReleaseTime { get; set; }
    public string? ReleaseComment { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProdScrapRecord
{
    public string ScrapId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public int ScrapQty { get; set; }
    public string ScrapReason { get; set; } = string.Empty;
    public string ScrapReasonCode { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public DateTime ScrapTime { get; set; }
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
    public bool RequiresApproval { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProdReworkRecord
{
    public string ReworkId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string OriginalRouteId { get; set; } = string.Empty;
    public string ReworkRouteId { get; set; } = string.Empty;
    public string FromStepCode { get; set; } = string.Empty;
    public string TargetStepCode { get; set; } = string.Empty;
    public int ReworkQty { get; set; }
    public string ReworkReason { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public int ReworkCount { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
}

public class ProdLotSplit
{
    public string SplitId { get; set; } = string.Empty;
    public string MotherLotId { get; set; } = string.Empty;
    public string ChildLotId { get; set; } = string.Empty;
    public int SplitQty { get; set; }
    public string SplitReason { get; set; } = string.Empty;
    public string SplitType { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime SplitTime { get; set; }
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProdLotMerge
{
    public string MergeId { get; set; } = string.Empty;
    public string TargetLotId { get; set; } = string.Empty;
    public string SourceLotId { get; set; } = string.Empty;
    public int MergeQty { get; set; }
    public string MergeReason { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime MergeTime { get; set; }
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProdAlarm
{
    public string AlarmId { get; set; } = string.Empty;
    public string RuleId { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string? EquipmentId { get; set; }
    public string AlarmType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProdCarrierBinding
{
    public string BindingId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string CarrierId { get; set; } = string.Empty;
    public string CarrierType { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public DateTime BindTime { get; set; }
    public DateTime? UnbindTime { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public string? FromCarrierId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ProdSignature
{
    public string SignatureId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string SignerId { get; set; } = string.Empty;
    public string SignerName { get; set; } = string.Empty;
    public string SignerRole { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTime SignTime { get; set; }
}

public class ProdDispatchTask
{
    public string TaskId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string? RecipeId { get; set; }
    public int Qty { get; set; }
    public string Priority { get; set; } = "Normal";
    public string Status { get; set; } = "Pending";
    public string? AssignedOperator { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal? DueHours { get; set; }
    public decimal? RemainingHours { get; set; }
    public bool IsOverdue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class QuantityTransaction
{
    public long Id { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public int ScrapQty { get; set; }
    public int ReworkQty { get; set; }
    public int HoldQty { get; set; }
    public int PendingQty { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class ProdGenealogy
{
    public string GenealogyId { get; set; } = string.Empty;
    public string ParentLotId { get; set; } = string.Empty;
    public string ChildLotId { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public int Qty { get; set; }
    public string? Grade { get; set; }
    public string? WaferLotId { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public string? ReasonCode { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
