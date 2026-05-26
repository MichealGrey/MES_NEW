namespace MES.Infrastructure.Persistence.Entities;

public class MasterProduct
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? DieName { get; set; }
    public string PackageType { get; set; } = string.Empty;
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
