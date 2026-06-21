namespace MES.Infrastructure.Persistence.Entities;

public class MasterProduct
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? DieName { get; set; }
    public string PackageType { get; set; } = string.Empty;
    public string ProcessStage { get; set; } = "Assemble";
    public string? DefaultRouteId { get; set; }
    public int UnitQty { get; set; } = 1;
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPn { get; set; }
    public string? InternalPn { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MasterRoute
{
    public string RouteId { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = "1.0";
    public string ProductId { get; set; } = string.Empty;
    public string? PackageType { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsApproved { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<MasterRouteStep> Steps { get; set; } = [];
}

public class MasterRouteStep
{
    public long Id { get; set; }
    public string RouteId { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string? EquipmentGroup { get; set; }
    public bool IsRework { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MasterEquipment
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentGroup { get; set; } = string.Empty;
    public string EquipmentType { get; set; } = string.Empty;
    public string ProcessStage { get; set; } = "Assemble";
    public string? Vendor { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? Capability { get; set; }
    public string Status { get; set; } = "Available";
    public string? CurrentLotId { get; set; }
    public string? CurrentRecipe { get; set; }
    public string? Location { get; set; }
    public string? ResponsiblePerson { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public int MaintenanceIntervalHours { get; set; } = 500;
    public int RunningHours { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MasterEquipmentRoute
{
    public long Id { get; set; }
    public string EquipmentId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
}

public class MasterCarrier
{
    public string CarrierId { get; set; } = string.Empty;
    public string CarrierType { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";
    public string? CurrentLotId { get; set; }
    public int Capacity { get; set; }
    public int UseCount { get; set; }
    public int MaxUseCount { get; set; }
    public DateTime? LastCleanDate { get; set; }
    public int CleanIntervalUses { get; set; }
    public string? Location { get; set; }
    public string? ApplicableProcess { get; set; }
    public string? ApplicablePackage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MasterCustomer
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? CustomerPnPrefix { get; set; }
    public string QualityLevel { get; set; } = "Industrial";
    public string? SpecialRequirements { get; set; }
    public string? DefaultPackingSpec { get; set; }
    public string? DefaultOqcSpec { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MasterReasonCode
{
    public string ReasonCodeId { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string ReasonText { get; set; } = string.Empty;
    public string ApplicableTo { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MasterDefectCode
{
    public string DefectCodeId { get; set; } = string.Empty;
    public string DefectCategory { get; set; } = string.Empty;
    public string DefectText { get; set; } = string.Empty;
    public string Severity { get; set; } = "Major";
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MasterRecipe
{
    public string RecipeId { get; set; } = string.Empty;
    public string RecipeName { get; set; } = string.Empty;
    public string EquipmentGroup { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public bool IsActive { get; set; } = true;
    public string? Parameters { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MasterYieldRule
{
    public string RuleId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public decimal YieldThreshold { get; set; }
    public string ActionType { get; set; } = "AutoHold";
    public string NotifyRole { get; set; } = "QA";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MasterAlarmRule
{
    public string RuleId { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Warning";
    public decimal? ThresholdYield { get; set; }
    public int? ThresholdQty { get; set; }
    public int? ThresholdMinutes { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string? NotifyRole { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MasterScrapRule
{
    public string RuleId { get; set; } = string.Empty;
    public string? RouteId { get; set; }
    public string? StepCode { get; set; }
    public decimal ThresholdPercent { get; set; }
    public bool RequiresApproval { get; set; } = true;
    public string? ApprovalLevel { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
