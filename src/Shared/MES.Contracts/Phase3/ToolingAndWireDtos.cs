namespace MES.Contracts.Phase3;

// ==================== 线材管控 DTOs ====================

public class WireMaterialSwitchRequest
{
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string OldWireMaterialId { get; set; } = string.Empty;
    public string OldWireMaterialName { get; set; } = string.Empty;
    public string? OldWireLotNo { get; set; }
    public decimal? OldWireDiameter { get; set; }
    public string NewWireMaterialId { get; set; } = string.Empty;
    public string NewWireMaterialName { get; set; } = string.Empty;
    public string NewWireLotNo { get; set; } = string.Empty;
    public decimal? NewWireDiameter { get; set; }
    public string? WireSupplier { get; set; }
    public string SwitchReason { get; set; } = string.Empty;
}

public class WireMaterialSwitchResponse
{
    public string SwitchId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public string OldWireMaterialId { get; set; } = string.Empty;
    public string OldWireMaterialName { get; set; } = string.Empty;
    public string NewWireMaterialId { get; set; } = string.Empty;
    public string NewWireMaterialName { get; set; } = string.Empty;
    public string SwitchReason { get; set; } = string.Empty;
    public string? OperatorName { get; set; }
    public DateTime SwitchTime { get; set; }
}

public class WireConsumptionResponse
{
    public long ConsumptionId { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public string WireMaterialId { get; set; } = string.Empty;
    public string WireMaterialName { get; set; } = string.Empty;
    public decimal ConsumedLength { get; set; }
    public string LengthUnit { get; set; } = string.Empty;
    public int? BondCount { get; set; }
    public decimal? AvgLengthPerBond { get; set; }
    public decimal? LossRate { get; set; }
    public DateTime ConsumptionTime { get; set; }
}

public class WireConsumptionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? LotId { get; set; }
    public string? StepCode { get; set; }
    public string? WireMaterialId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

// ==================== 工装管控 DTOs ====================

public class CreateToolingRequest
{
    public string ToolingCode { get; set; } = string.Empty;
    public string ToolingName { get; set; } = string.Empty;
    public string ToolingType { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Supplier { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public int? ExpectedLifespan { get; set; }
    public string LifespanUnit { get; set; } = "Hours";
    public string? Location { get; set; }
    public string? AssociatedProcess { get; set; }
    public string? Remark { get; set; }
}

public class ToolingResponse
{
    public string ToolingId { get; set; } = string.Empty;
    public string ToolingCode { get; set; } = string.Empty;
    public string ToolingName { get; set; } = string.Empty;
    public string ToolingType { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public int? ExpectedLifespan { get; set; }
    public string LifespanUnit { get; set; } = string.Empty;
    public int CurrentUsage { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? AssociatedEquipment { get; set; }
    public string? AssociatedProcess { get; set; }
    public string? NextMaintenanceDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ToolingQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ToolingType { get; set; }
    public string? Status { get; set; }
    public string? AssociatedEquipment { get; set; }
}

public class ToolingUsageLogResponse
{
    public long LogId { get; set; }
    public string ToolingId { get; set; } = string.Empty;
    public string ToolingCode { get; set; } = string.Empty;
    public string ToolingName { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? UsageDuration { get; set; }
    public int? UsageCount { get; set; }
    public string? UsageStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ToolingReplacementResponse
{
    public string ReplacementId { get; set; } = string.Empty;
    public string OldToolingId { get; set; } = string.Empty;
    public string OldToolingCode { get; set; } = string.Empty;
    public string OldToolingName { get; set; } = string.Empty;
    public string NewToolingId { get; set; } = string.Empty;
    public string NewToolingCode { get; set; } = string.Empty;
    public string NewToolingName { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public string ReplacementReason { get; set; } = string.Empty;
    public int? OldToolingUsage { get; set; }
    public decimal? UsagePercentage { get; set; }
    public DateTime ReplacementTime { get; set; }
}

public class ToolingUsageLogRequest
{
    public string ToolingId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? UsageDuration { get; set; }
    public string? UsageDurationUnit { get; set; }
    public int? UsageCount { get; set; }
    public string? UsageStatus { get; set; }
    public string? Remark { get; set; }
}

public class ToolingReplacementRequest
{
    public string OldToolingId { get; set; } = string.Empty;
    public string NewToolingId { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public string? LotId { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string ReplacementReason { get; set; } = string.Empty;
    public string? ReasonDetail { get; set; }
    public string? VerificationResult { get; set; }
}

// ==================== 操作员资质管理 DTOs ====================

public class CreateOperatorQualificationRequest
{
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string QualificationLevel { get; set; } = "Trainee";
    public string CertificationType { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? IssuedBy { get; set; }
    public string? CertificationNo { get; set; }
    public string? Remark { get; set; }
}

public class OperatorQualificationResponse
{
    public string QualificationId { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string QualificationLevel { get; set; } = string.Empty;
    public string CertificationType { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CertificationNo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OperatorQualificationQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? OperatorId { get; set; }
    public string? ProcessCode { get; set; }
    public string? Status { get; set; }
    public string? QualificationLevel { get; set; }
}

public class QualificationCheckLogResponse
{
    public long LogId { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string? EquipmentId { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string CheckType { get; set; } = string.Empty;
    public string CheckResult { get; set; } = string.Empty;
    public string? QualificationLevel { get; set; }
    public bool IsQualified { get; set; }
    public string? FailReason { get; set; }
    public string? Action { get; set; }
    public DateTime CheckTime { get; set; }
}

public class OperatorQualificationCheckRequest
{
    public string OperatorId { get; set; } = string.Empty;
    public string ProcessCode { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string? EquipmentId { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string CheckType { get; set; } = "TrackIn";
}

// ==================== 焊线拉力测试 DTOs ====================

public class CreateBondPullTestRequest
{
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string? TestEquipmentId { get; set; }
    public string? TestEquipmentName { get; set; }
    public string? WireMaterialId { get; set; }
    public string? WireMaterialName { get; set; }
    public decimal? WireDiameter { get; set; }
    public string? WireLotNo { get; set; }
    public int SampleSize { get; set; }
    public decimal? MinPullForce { get; set; }
    public decimal? MaxPullForce { get; set; }
    public decimal? AvgPullForce { get; set; }
    public decimal? StdDeviation { get; set; }
    public decimal? SpecLowerLimit { get; set; }
    public decimal? SpecUpperLimit { get; set; }
    public string? Unit { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public string TestResult { get; set; } = "Pass";
    public string? FailureMode { get; set; }
    public string? TestMethod { get; set; }
    public string? TestEnvironment { get; set; }
    public string? TestSamples { get; set; }
}

public class BondPullTestResponse
{
    public string TestId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string? TestEquipmentId { get; set; }
    public string? TestEquipmentName { get; set; }
    public int SampleSize { get; set; }
    public decimal? MinPullForce { get; set; }
    public decimal? MaxPullForce { get; set; }
    public decimal? AvgPullForce { get; set; }
    public decimal? StdDeviation { get; set; }
    public decimal? SpecLowerLimit { get; set; }
    public decimal? SpecUpperLimit { get; set; }
    public string? Unit { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public string TestResult { get; set; } = string.Empty;
    public string? FailureMode { get; set; }
    public string? TestMethod { get; set; }
    public string? TesterName { get; set; }
    public DateTime TestTime { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

public class BondPullTestQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? LotId { get; set; }
    public string? StepCode { get; set; }
    public string? TestResult { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
