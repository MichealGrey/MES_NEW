namespace MES.Modules.Equipment.Models;

public class EquipmentInfo
{
    public string Id { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Status { get; set; } = "Idle";
    public string? CurrentLot { get; set; }
    public string? RecipeId { get; set; }
    public double OEE { get; set; }
    public double Availability { get; set; }
    public double Performance { get; set; }
    public double Quality { get; set; }
    public DateTime LastMaintenance { get; set; }
    public int TotalRunHours { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public DateTime InstallDate { get; set; }
}

public class EquipmentHistoryRecord
{
    public string Id { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public DateTime EventTime { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
}

public class PmScheduleItem
{
    public string Id { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string PmType { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string AssignedTo { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class SparePartItem
{
    public string Id { get; set; } = string.Empty;
    public string PartName { get; set; } = string.Empty;
    public string PartNo { get; set; } = string.Empty;
    public string EquipmentType { get; set; } = string.Empty;
    public int StockQty { get; set; }
    public int MinQty { get; set; }
    public int MaxQty { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double UnitPrice { get; set; }
}

public class FixtureItem
{
    public string Id { get; set; } = string.Empty;
    public string FixtureNo { get; set; } = string.Empty;
    public string FixtureType { get; set; } = string.Empty;
    public string EquipmentType { get; set; } = string.Empty;
    public int UseCount { get; set; }
    public int MaxUseCount { get; set; }
    public string Status { get; set; } = "Available";
    public DateTime LastUsed { get; set; }
    public DateTime LastCalibration { get; set; }
    public string Location { get; set; } = string.Empty;
}

public class EquipmentPerformanceItem
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public double OEE { get; set; }
    public double Availability { get; set; }
    public double Performance { get; set; }
    public double Quality { get; set; }
    public int TotalLots { get; set; }
    public int CompletedLots { get; set; }
    public double AvgCycleTime { get; set; }
    public int AlarmCount { get; set; }
    public double Uptime { get; set; }
}

public class EquipmentAlarmRecord
{
    public string Id { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string AlarmCode { get; set; } = string.Empty;
    public string AlarmMessage { get; set; } = string.Empty;
    public DateTime AlarmTime { get; set; }
    public DateTime? ClearTime { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
}
