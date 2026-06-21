namespace MES.Infrastructure.Persistence.Entities;

// ==================== 工序参数管控 ====================

/// <summary>
/// 工序参数集
/// </summary>
public class ProcessParameterSet
{
    public string ParameterSetId { get; set; } = string.Empty;
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? EquipmentType { get; set; }
    public string Version { get; set; } = "1.0";
    public string Status { get; set; } = "Active";
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Description { get; set; }
    public int ItemCount { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Deleted { get; set; }
}

/// <summary>
/// 参数明细
/// </summary>
public class ProcessParameterItem
{
    public long ItemId { get; set; }
    public string ParameterSetId { get; set; } = string.Empty;
    public string ParameterCode { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public string? ParameterType { get; set; } // Numeric, Boolean, Enum, Text
    public string? Unit { get; set; }
    public decimal? StandardValue { get; set; }
    public decimal? LowerLimit { get; set; }
    public decimal? UpperLimit { get; set; }
    public decimal? TargetValue { get; set; }
    public decimal? WarningLowerLimit { get; set; }
    public decimal? WarningUpperLimit { get; set; }
    public bool IsRequired { get; set; }
    public bool IsAutoCollect { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRule { get; set; }
    public int SortOrder { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 参数修改日志
/// </summary>
public class ProcessParameterOverrideLog
{
    public long LogId { get; set; }
    public string ParameterSetId { get; set; } = string.Empty;
    public long? ItemId { get; set; }
    public string ParameterCode { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string? EquipmentId { get; set; }
    public decimal? OriginalValue { get; set; }
    public decimal? NewValue { get; set; }
    public string? OriginalLowerLimit { get; set; }
    public string? OriginalUpperLimit { get; set; }
    public string? NewLowerLimit { get; set; }
    public string? NewUpperLimit { get; set; }
    public string OverrideType { get; set; } = string.Empty; // Value, Limit, Both
    public string? Reason { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public DateTime OverrideTime { get; set; } = DateTime.UtcNow;
}

// ==================== 固化温度曲线 ====================

/// <summary>
/// 固化温度曲线
/// </summary>
public class CuringTemperatureCurve
{
    public string CurveId { get; set; } = string.Empty;
    public string CurveName { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? GlueType { get; set; }
    public string? EquipmentType { get; set; }
    public string Version { get; set; } = "1.0";
    public string Status { get; set; } = "Active";
    public int? TotalZones { get; set; }
    public decimal? PreheatTemp { get; set; }
    public decimal? PreheatDuration { get; set; }
    public decimal? CuringTemp { get; set; }
    public decimal? CuringDuration { get; set; }
    public decimal? CoolingTemp { get; set; }
    public decimal? CoolingDuration { get; set; }
    public decimal? RampUpRate { get; set; }
    public decimal? RampDownRate { get; set; }
    public string? ZoneTemperatures { get; set; } // JSON: [{ZoneNo, Temp, Duration}]
    public string? ProfileData { get; set; } // JSON full profile
    public DateTime? EffectiveDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Deleted { get; set; }
}

// ==================== Bin 分选管控 ====================

/// <summary>
/// Bin 定义
/// </summary>
public class BinDefinition
{
    public string BinId { get; set; } = string.Empty;
    public string BinCode { get; set; } = string.Empty;
    public string BinName { get; set; } = string.Empty;
    public string BinCategory { get; set; } = string.Empty; // Good, Fail, Skip
    public int BinNo { get; set; }
    public string? Description { get; set; }
    public string Color { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ProductId { get; set; }
    public string? ProcessCode { get; set; }
    public string? TestType { get; set; } // CP, FT
    public int SortOrder { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Deleted { get; set; }
}

/// <summary>
/// Bin 分选记录
/// </summary>
public class BinSortRecord
{
    public string RecordId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string? TestProgram { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public string BinName { get; set; } = string.Empty;
    public string BinCategory { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal? YieldContribution { get; set; }
    public string? TestResult { get; set; }
    public string? BinDescription { get; set; }
    public string? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public DateTime SortTime { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Bin 统计表
/// </summary>
public class BinStatistics
{
    public long StatId { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public string BinName { get; set; } = string.Empty;
    public string BinCategory { get; set; } = string.Empty;
    public int TotalQty { get; set; }
    public decimal? Percentage { get; set; }
    public int InputQty { get; set; }
    public decimal? CumulativeYield { get; set; }
    public DateTime StatPeriod { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ==================== 线材管控 ====================

/// <summary>
/// 线材切换记录
/// </summary>
public class WireMaterialSwitchRecord
{
    public string SwitchId { get; set; } = string.Empty;
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
    public string? VerificationResult { get; set; }
    public string? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public DateTime SwitchTime { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 线材消耗记录
/// </summary>
public class WireConsumption
{
    public long ConsumptionId { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string WireMaterialId { get; set; } = string.Empty;
    public string WireMaterialName { get; set; } = string.Empty;
    public string WireLotNo { get; set; } = string.Empty;
    public decimal? WireDiameter { get; set; }
    public decimal ConsumedLength { get; set; }
    public string LengthUnit { get; set; } = "m";
    public int? BondCount { get; set; }
    public int? ProductQty { get; set; }
    public decimal? AvgLengthPerBond { get; set; }
    public decimal? TheoreticalLength { get; set; }
    public decimal? LossRate { get; set; }
    public string? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public DateTime ConsumptionTime { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ==================== 工装管控 ====================

/// <summary>
/// 工装台账
/// </summary>
public class ToolingRegistry
{
    public string ToolingId { get; set; } = string.Empty;
    public string ToolingCode { get; set; } = string.Empty;
    public string ToolingName { get; set; } = string.Empty;
    public string ToolingType { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Supplier { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? InstallDate { get; set; }
    public int? ExpectedLifespan { get; set; } // hours or cycles
    public string LifespanUnit { get; set; } = "Hours"; // Hours, Cycles
    public int CurrentUsage { get; set; }
    public string Status { get; set; } = "Available"; // Available, InUse, Maintenance, Retired
    public string? Location { get; set; }
    public string? AssociatedEquipment { get; set; }
    public string? AssociatedProcess { get; set; }
    public string? LastMaintenanceDate { get; set; }
    public string? NextMaintenanceDate { get; set; }
    public string? Remark { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Deleted { get; set; }
}

/// <summary>
/// 工装使用日志
/// </summary>
public class ToolingUsageLog
{
    public long LogId { get; set; }
    public string ToolingId { get; set; } = string.Empty;
    public string ToolingCode { get; set; } = string.Empty;
    public string ToolingName { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? UsageDuration { get; set; }
    public string? UsageDurationUnit { get; set; }
    public int? UsageCount { get; set; }
    public string? UsageStatus { get; set; } // Normal, Warning, Abnormal
    public string? Remark { get; set; }
    public string? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 工装更换记录
/// </summary>
public class ToolingReplacementRecord
{
    public string ReplacementId { get; set; } = string.Empty;
    public string OldToolingId { get; set; } = string.Empty;
    public string OldToolingCode { get; set; } = string.Empty;
    public string OldToolingName { get; set; } = string.Empty;
    public string NewToolingId { get; set; } = string.Empty;
    public string NewToolingCode { get; set; } = string.Empty;
    public string NewToolingName { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public string? LotId { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string ReplacementReason { get; set; } = string.Empty; // Normal, WearOut, Damage, Maintenance
    public string? ReasonDetail { get; set; }
    public int? OldToolingUsage { get; set; }
    public int? ExpectedLifespan { get; set; }
    public decimal? UsagePercentage { get; set; }
    public string? VerificationResult { get; set; }
    public string? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public DateTime ReplacementTime { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ==================== 操作员资质管理 ====================

/// <summary>
/// 操作员资质
/// </summary>
public class OperatorQualification
{
    public string QualificationId { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string QualificationLevel { get; set; } = string.Empty; // Trainee, Qualified, Expert
    public string CertificationType { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? IssuedBy { get; set; }
    public string? CertificationNo { get; set; }
    public string Status { get; set; } = "Active"; // Active, Expired, Revoked, Pending
    public string? Remark { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Deleted { get; set; }
}

/// <summary>
/// 资质校验日志
/// </summary>
public class QualificationCheckLog
{
    public long LogId { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string? QualificationId { get; set; }
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string? EquipmentId { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string CheckType { get; set; } = string.Empty; // TrackIn, Periodic, Manual
    public string CheckResult { get; set; } = string.Empty; // Pass, Fail, Warning
    public string? QualificationLevel { get; set; }
    public string? QualificationStatus { get; set; }
    public DateTime? QualificationExpiryDate { get; set; }
    public bool IsQualified { get; set; }
    public string? FailReason { get; set; }
    public string? Action { get; set; } // Allow, Block, Warn
    public DateTime CheckTime { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

