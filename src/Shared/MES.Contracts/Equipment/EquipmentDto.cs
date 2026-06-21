namespace MES.Contracts.Equipment;

/// <summary>
/// 设备基本信息 DTO
/// </summary>
public class EquipmentDto
{
    public string EquipmentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Status { get; set; } = "Idle";
    public string? CurrentLot { get; set; }
    public string? RecipeId { get; set; }
    public DateTime? LastPmTime { get; set; }
}

/// <summary>
/// 设备列表响应 DTO
/// </summary>
public class EquipmentListResponse
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime? LastMaintenance { get; set; }
    public double? Oee { get; set; }
}

/// <summary>
/// 设备详情响应 DTO
/// </summary>
public class EquipmentDetailResponse
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentGroup { get; set; } = string.Empty;
    public string EquipmentType { get; set; } = string.Empty;
    public string ProcessStage { get; set; } = string.Empty;
    public string? Vendor { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CurrentLotId { get; set; }
    public string? CurrentRecipe { get; set; }
    public string? Location { get; set; }
    public string? ResponsiblePerson { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public int MaintenanceIntervalHours { get; set; }
    public int RunningHours { get; set; }
    public List<MaintenanceResponse>? RecentMaintenances { get; set; }
    public List<FailureResponse>? RecentFailures { get; set; }
}

/// <summary>
/// 设备状态更新请求
/// </summary>
public class EquipmentStatusUpdateRequest
{
    public string EquipmentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? OperatorId { get; set; }
}

/// <summary>
/// 维护记录创建请求
/// </summary>
public class MaintenanceCreateRequest
{
    public string EquipmentId { get; set; } = string.Empty;
    public string MaintenanceType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string? TechnicianId { get; set; }
    public double? EstimatedHours { get; set; }
    public string? ReportedBy { get; set; }
}

/// <summary>
/// 维护记录响应
/// </summary>
public class MaintenanceResponse
{
    public long MaintenanceId { get; set; }
    public string EquipmentId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Technician { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public double? ActualHours { get; set; }
    public double? EstimatedHours { get; set; }
    public string? PartsReplaced { get; set; }
}

/// <summary>
/// 故障记录创建请求
/// </summary>
public class FailureCreateRequest
{
    public string EquipmentId { get; set; } = string.Empty;
    public string FailureType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string ReportedBy { get; set; } = string.Empty;
    public int? DowntimeMinutes { get; set; }
}

/// <summary>
/// 故障记录响应
/// </summary>
public class FailureResponse
{
    public long FailureId { get; set; }
    public string EquipmentId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ReportedBy { get; set; } = string.Empty;
    public DateTime ReportedTime { get; set; }
    public DateTime? ResolvedTime { get; set; }
    public string? ResolvedBy { get; set; }
    public int? DowntimeMinutes { get; set; }
    public string? RootCause { get; set; }
    public string? Resolution { get; set; }
}

/// <summary>
/// 设备 OEE 响应
/// </summary>
public class EquipmentOeeResponse
{
    public string EquipmentId { get; set; } = string.Empty;
    public double Availability { get; set; }
    public double Performance { get; set; }
    public double Quality { get; set; }
    public double Oee { get; set; }
    public double RunTime { get; set; }
    public double Downtime { get; set; }
    public long TotalUnits { get; set; }
    public long GoodUnits { get; set; }
}

/// <summary>
/// 设备看板响应
/// </summary>
public class EquipmentDashboardResponse
{
    public int TotalEquipment { get; set; }
    public Dictionary<string, int> StatusDistribution { get; set; } = new();
    public List<EquipmentAlert> ActiveAlerts { get; set; } = [];
    public List<UpcomingMaintenance> UpcomingMaintenances { get; set; } = [];
}

/// <summary>
/// 设备告警
/// </summary>
public class EquipmentAlert
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 即将到期的维护
/// </summary>
public class UpcomingMaintenance
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string MaintenanceType { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
}
