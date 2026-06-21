namespace MES.Contracts.Phase3;

// ==================== 工序参数管控 DTOs ====================

public class CreateProcessParameterSetRequest
{
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? EquipmentType { get; set; }
    public string Version { get; set; } = "1.0";
    public string? Description { get; set; }
    public List<CreateParameterItemRequest> Items { get; set; } = new();
}

public class CreateParameterItemRequest
{
    public string ParameterCode { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public string? ParameterType { get; set; }
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
}

public class ProcessParameterSetResponse
{
    public string ParameterSetId { get; set; } = string.Empty;
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? EquipmentType { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Description { get; set; }
    public int ItemCount { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ParameterItemResponse
{
    public long ItemId { get; set; }
    public string ParameterSetId { get; set; } = string.Empty;
    public string ParameterCode { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public string? ParameterType { get; set; }
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
    public DateTime CreatedAt { get; set; }
}

public class ProcessParameterQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ProcessCode { get; set; }
    public string? ProductId { get; set; }
    public string? EquipmentType { get; set; }
    public string? Status { get; set; }
}

public class ActivateParameterSetRequest
{
    public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
}

public class OverrideParameterRequest
{
    public string? LotId { get; set; }
    public string? EquipmentId { get; set; }
    public string ParameterCode { get; set; } = string.Empty;
    public decimal? NewValue { get; set; }
    public string? NewLowerLimit { get; set; }
    public string? NewUpperLimit { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ParameterOverrideLogResponse
{
    public long LogId { get; set; }
    public string ParameterSetId { get; set; } = string.Empty;
    public string ParameterCode { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string? EquipmentId { get; set; }
    public decimal? OriginalValue { get; set; }
    public decimal? NewValue { get; set; }
    public string OverrideType { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? OperatorName { get; set; }
    public DateTime OverrideTime { get; set; }
}

// ==================== 固化温度曲线 DTOs ====================

public class CreateCuringCurveRequest
{
    public string CurveName { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? GlueType { get; set; }
    public string? EquipmentType { get; set; }
    public string Version { get; set; } = "1.0";
    public int? TotalZones { get; set; }
    public decimal? PreheatTemp { get; set; }
    public decimal? PreheatDuration { get; set; }
    public decimal? CuringTemp { get; set; }
    public decimal? CuringDuration { get; set; }
    public decimal? CoolingTemp { get; set; }
    public decimal? CoolingDuration { get; set; }
    public decimal? RampUpRate { get; set; }
    public decimal? RampDownRate { get; set; }
    public string? ZoneTemperatures { get; set; }
    public string? ProfileData { get; set; }
    public string? Description { get; set; }
}

public class CuringCurveResponse
{
    public string CurveId { get; set; } = string.Empty;
    public string CurveName { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? GlueType { get; set; }
    public string? EquipmentType { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? TotalZones { get; set; }
    public decimal? PreheatTemp { get; set; }
    public decimal? PreheatDuration { get; set; }
    public decimal? CuringTemp { get; set; }
    public decimal? CuringDuration { get; set; }
    public string? ZoneTemperatures { get; set; }
    public string? ProfileData { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CuringCurveQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ProductId { get; set; }
    public string? EquipmentType { get; set; }
    public string? Status { get; set; }
    public string? GlueType { get; set; }
}
