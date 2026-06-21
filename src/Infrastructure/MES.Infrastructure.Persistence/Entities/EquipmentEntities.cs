namespace MES.Infrastructure.Persistence.Entities;

/// <summary>
/// 设备维护记录实体
/// </summary>
public class EquipmentMaintenance
{
    public long Id { get; set; }
    public string EquipmentId { get; set; } = string.Empty;
    public string MaintenanceType { get; set; } = string.Empty; // Preventive, Corrective, Calibration
    public string? Description { get; set; }
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled
    public string? TechnicianId { get; set; }
    public string? TechnicianName { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedDate { get; set; }
    public double? EstimatedHours { get; set; }
    public double? ActualHours { get; set; }
    public string? PartsReplaced { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 设备故障记录实体
/// </summary>
public class EquipmentFailure
{
    public long Id { get; set; }
    public string EquipmentId { get; set; } = string.Empty;
    public string FailureType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "Minor"; // Minor, Major, Critical
    public string Status { get; set; } = "Open"; // Open, InProgress, Resolved, Closed
    public string ReportedBy { get; set; } = string.Empty;
    public string? ReportedByName { get; set; }
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public int? DowntimeMinutes { get; set; }
    public string? RootCause { get; set; }
    public string? Resolution { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
