using System.Collections.ObjectModel;

namespace MES.Modules.SystemSettings.Models;

/// <summary>
/// 系统参数
/// </summary>
public class SystemParameter
{
    public string ParamId { get; set; } = string.Empty;
    public string ParamCode { get; set; } = string.Empty;
    public string ParamName { get; set; } = string.Empty;
    public string ParamValue { get; set; } = string.Empty;
    public string ParamType { get; set; } = "String";
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public bool IsVisible { get; set; } = true;
    public string? DefaultValue { get; set; }
    public DateTime? LastModified { get; set; }
    public string? ModifiedBy { get; set; }
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 用户信息
/// </summary>
public class UserInfo
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public bool IsLocked { get; set; }
    public int LoginFailCount { get; set; }
}

/// <summary>
/// 角色信息
/// </summary>
public class RoleInfo
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}

/// <summary>
/// 权限信息
/// </summary>
public class PermissionInfo
{
    public string PermissionId { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// 用户角色关联
/// </summary>
public class UserRoleMapping
{
    public string MappingId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; } = DateTime.Now;
    public string AssignedBy { get; set; } = string.Empty;
}

/// <summary>
/// 操作日志
/// </summary>
public class OperationLog
{
    public string LogId { get; set; } = string.Empty;
    public DateTime OperationTime { get; set; } = DateTime.Now;
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string Result { get; set; } = "Success";
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 系统监控指标
/// </summary>
public class SystemMonitorMetric
{
    public string MetricId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string MetricName { get; set; } = string.Empty;
    public string MetricType { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public double Threshold { get; set; }
    public bool IsExceeded => Value > Threshold;
    public string ServerName { get; set; } = string.Empty;
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 系统服务状态
/// </summary>
public class ServiceStatus
{
    public string ServiceId { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = "Running";
    public DateTime LastHeartbeat { get; set; } = DateTime.Now;
    public int ResponseTime { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int RequestCount { get; set; }
    public int ErrorCount { get; set; }
    public double ErrorRate => RequestCount > 0 ? (double)ErrorCount / RequestCount * 100 : 0;
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 数据库连接状态
/// </summary>
public class DatabaseConnectionStatus
{
    public string ConnectionId { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string Status { get; set; } = "Connected";
    public int ActiveConnections { get; set; }
    public int MaxConnections { get; set; }
    public double ConnectionPoolUsage => MaxConnections > 0 ? (double)ActiveConnections / MaxConnections * 100 : 0;
    public long DatabaseSizeMB { get; set; }
    public DateTime LastBackupTime { get; set; }
    public string ServerVersion { get; set; } = string.Empty;
    public double AvgQueryTime { get; set; }
    public long SlowQueries { get; set; }
}

/// <summary>
/// 系统健康报告
/// </summary>
public class SystemHealthReport
{
    public string ReportId { get; set; } = string.Empty;
    public DateTime ReportTime { get; set; } = DateTime.Now;
    public string OverallStatus { get; set; } = "Healthy";
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public double NetworkUsage { get; set; }
    public int ActiveUsers { get; set; }
    public int PendingTasks { get; set; }
    public int ActiveConnections { get; set; }
    public int ErrorCount24h { get; set; }
    public double Uptime { get; set; }
    public string DatabaseStatus { get; set; } = string.Empty;
    public string CacheStatus { get; set; } = string.Empty;
    public string QueueStatus { get; set; } = string.Empty;
    public List<HealthAlert> Alerts { get; set; } = [];
}

/// <summary>
/// 健康告警
/// </summary>
public class HealthAlert
{
    public string AlertId { get; set; } = string.Empty;
    public DateTime AlertTime { get; set; } = DateTime.Now;
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Warning";
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public bool IsAcknowledged { get; set; }
    public string? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedTime { get; set; }
}

/// <summary>
/// 外部系统配置
/// </summary>
public class ExternalSystemConfig
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemCode { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public string SystemType { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string AuthType { get; set; } = "Basic";
    public bool IsEnabled { get; set; } = true;
    public int Timeout { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
    public string? LastSyncTime { get; set; }
    public string LastSyncStatus { get; set; } = string.Empty;
    public int TotalSyncCount { get; set; }
    public int FailedSyncCount { get; set; }
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 外部系统同步记录
/// </summary>
public class SyncRecord
{
    public string RecordId { get; set; } = string.Empty;
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public DateTime SyncTime { get; set; } = DateTime.Now;
    public string SyncType { get; set; } = string.Empty;
    public string SyncStatus { get; set; } = string.Empty;
    public int RecordsCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public int Duration { get; set; }
    public string? ErrorMessage { get; set; }
    public string TriggeredBy { get; set; } = string.Empty;
}

/// <summary>
/// 外部系统事件
/// </summary>
public class ExternalSystemEvent
{
    public string EventId { get; set; } = string.Empty;
    public DateTime EventTime { get; set; } = DateTime.Now;
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public string ProcessingStatus { get; set; } = "Pending";
    public string? ProcessedBy { get; set; }
    public DateTime? ProcessedTime { get; set; }
    public string? ErrorMessage { get; set; }
}
