using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MES.Contracts.Common;
using MES.Modules.SystemSettings.Models;

namespace MES.Modules.SystemSettings.Services;

/// <summary>
/// Interface for Settings operations.
/// </summary>
public interface ISettingsService
{
    // System parameters
    Task<List<SystemParameter>> GetAllParametersAsync();
    Task<SystemParameter?> GetParameterAsync(string paramCode);
    Task SaveParameterAsync(SystemParameter parameter);
    Task DeleteParameterAsync(string paramCode);
    Task UpdateParameterValueAsync(string paramCode, string value, string modifiedBy);
    Task<List<SystemParameter>> GetParametersByCategoryAsync(string category);

    // User permissions
    Task<List<UserInfo>> GetAllUsersAsync();
    Task<UserInfo?> GetUserAsync(string userId);
    Task SaveUserAsync(UserInfo user);
    Task DeleteUserAsync(string userId);
    Task UpdateUserStatusAsync(string userId, string status);
    Task<List<RoleInfo>> GetAllRolesAsync();
    Task SaveRoleAsync(RoleInfo role);
    Task DeleteRoleAsync(string roleId);
    Task<List<PermissionInfo>> GetAllPermissionsAsync();
    Task<List<UserRoleMapping>> GetUserRoleMappingsAsync(string? userId = null);
    Task AssignRoleToUserAsync(string userId, string roleId, string assignedBy);
    Task RemoveRoleFromUserAsync(string mappingId);
    Task<List<UserInfo>> GetUsersByRoleAsync(string roleId);

    // Operation logs
    Task<List<OperationLog>> GetOperationLogsAsync(DateTime? startDate = null, DateTime? endDate = null,
        string? module = null, string? operatorName = null, string? operationType = null);
    Task<OperationLog?> GetOperationLogAsync(string logId);
    Task<long> GetOperationLogCountAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<string>> GetDistinctModulesAsync();
    Task<List<string>> GetDistinctOperationTypesAsync();

    // System monitoring
    Task<List<SystemMonitorMetric>> GetMonitorMetricsAsync(string? metricType = null, int hours = 24);
    Task<List<ServiceStatus>> GetServiceStatusesAsync();
    Task<DatabaseConnectionStatus> GetDatabaseStatusAsync();
    Task<SystemHealthReport> GetSystemHealthReportAsync();
    Task<List<HealthAlert>> GetHealthAlertsAsync(bool? isAcknowledged = null);
    Task AcknowledgeAlertAsync(string alertId, string acknowledgedBy);

    // External systems
    Task<List<ExternalSystemConfig>> GetAllExternalSystemsAsync();
    Task<ExternalSystemConfig?> GetExternalSystemAsync(string systemId);
    Task SaveExternalSystemAsync(ExternalSystemConfig config);
    Task DeleteExternalSystemAsync(string systemId);
    Task UpdateExternalSystemStatusAsync(string systemId, bool isEnabled);
    Task<List<SyncRecord>> GetSyncRecordsAsync(string? systemId = null, int limit = 50);
    Task<List<ExternalSystemEvent>> GetExternalEventsAsync(string? systemId = null, string? status = null);
    Task ProcessEventAsync(string eventId, string processedBy);
    Task TriggerSyncAsync(string systemId, string triggeredBy);
}

/// <summary>
/// REST API client service for Settings operations.
/// Communicates with the backend SettingsController.
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly HttpClient _httpClient;

    public SettingsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ============================================================================
    // System Parameters
    // ============================================================================

    public async Task<List<SystemParameter>> GetAllParametersAsync()
    {
        var response = await _httpClient.GetAsync("Settings/parameters");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<SystemParameterDto>>>();
        return apiResponse?.Data?.Select(MapToSystemParameter).ToList() ?? new List<SystemParameter>();
    }

    public async Task<SystemParameter?> GetParameterAsync(string paramCode)
    {
        var response = await _httpClient.GetAsync($"Settings/parameters/{paramCode}");
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<SystemParameterDto>>();
        return apiResponse?.Data != null ? MapToSystemParameter(apiResponse.Data) : null;
    }

    public async Task SaveParameterAsync(SystemParameter parameter)
    {
        var dto = MapToParameterDto(parameter);
        var response = await _httpClient.PostAsJsonAsync("Settings/parameters", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteParameterAsync(string paramCode)
    {
        var response = await _httpClient.DeleteAsync($"Settings/parameters/{paramCode}");
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateParameterValueAsync(string paramCode, string value, string modifiedBy)
    {
        var request = new UpdateParamValueRequest(value, modifiedBy);
        var response = await _httpClient.PatchAsJsonAsync($"Settings/parameters/{paramCode}/value", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<SystemParameter>> GetParametersByCategoryAsync(string category)
    {
        var all = await GetAllParametersAsync();
        return all.Where(p => p.Category == category).ToList();
    }

    // ============================================================================
    // User Permissions
    // ============================================================================

    public async Task<List<UserInfo>> GetAllUsersAsync()
    {
        var response = await _httpClient.GetAsync("Settings/users");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserInfoDto>>>();
        return apiResponse?.Data?.Select(MapToUserInfo).ToList() ?? new List<UserInfo>();
    }

    public async Task<UserInfo?> GetUserAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"Settings/users/{userId}");
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserInfoDto>>();
        return apiResponse?.Data != null ? MapToUserInfo(apiResponse.Data) : null;
    }

    public async Task SaveUserAsync(UserInfo user)
    {
        var dto = MapToUserDto(user);
        var response = await _httpClient.PostAsJsonAsync("Settings/users", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteUserAsync(string userId)
    {
        var response = await _httpClient.DeleteAsync($"Settings/users/{userId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateUserStatusAsync(string userId, string status)
    {
        var request = new UpdateStatusRequest(status);
        var response = await _httpClient.PatchAsJsonAsync($"Settings/users/{userId}/status", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<RoleInfo>> GetAllRolesAsync()
    {
        var response = await _httpClient.GetAsync("Settings/roles");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<RoleInfoDto>>>();
        return apiResponse?.Data?.Select(MapToRoleInfo).ToList() ?? new List<RoleInfo>();
    }

    public async Task SaveRoleAsync(RoleInfo role)
    {
        var dto = MapToRoleDto(role);
        var response = await _httpClient.PostAsJsonAsync("Settings/roles", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteRoleAsync(string roleId)
    {
        var response = await _httpClient.DeleteAsync($"Settings/roles/{roleId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<PermissionInfo>> GetAllPermissionsAsync()
    {
        var response = await _httpClient.GetAsync("Settings/permissions");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<PermissionInfoDto>>>();
        return apiResponse?.Data?.Select(MapToPermissionInfo).ToList() ?? new List<PermissionInfo>();
    }

    public async Task<List<UserRoleMapping>> GetUserRoleMappingsAsync(string? userId = null)
    {
        var url = string.IsNullOrWhiteSpace(userId)
            ? "Settings/user-roles"
            : $"Settings/user-roles?userId={userId}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserRoleMappingDto>>>();
        return apiResponse?.Data?.Select(MapToUserRoleMapping).ToList() ?? new List<UserRoleMapping>();
    }

    public async Task AssignRoleToUserAsync(string userId, string roleId, string assignedBy)
    {
        var request = new AssignRoleRequest(userId, roleId, assignedBy);
        var response = await _httpClient.PostAsJsonAsync("Settings/user-roles", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveRoleFromUserAsync(string mappingId)
    {
        var response = await _httpClient.DeleteAsync($"Settings/user-roles/{mappingId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<UserInfo>> GetUsersByRoleAsync(string roleId)
    {
        var allMappings = await GetUserRoleMappingsAsync();
        var userIds = allMappings.Where(m => m.RoleId == roleId).Select(m => m.UserId).Distinct();
        var result = new List<UserInfo>();
        foreach (var userId in userIds)
        {
            var user = await GetUserAsync(userId);
            if (user != null) result.Add(user);
        }
        return result;
    }

    // ============================================================================
    // Operation Logs
    // ============================================================================

    public async Task<List<OperationLog>> GetOperationLogsAsync(DateTime? startDate = null, DateTime? endDate = null,
        string? module = null, string? operatorName = null, string? operationType = null)
    {
        var queryParams = new List<string>();
        if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:O}");
        if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:O}");
        if (!string.IsNullOrWhiteSpace(module)) queryParams.Add($"module={module}");
        if (!string.IsNullOrWhiteSpace(operatorName)) queryParams.Add($"operatorName={operatorName}");
        if (!string.IsNullOrWhiteSpace(operationType)) queryParams.Add($"operationType={operationType}");

        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
        var response = await _httpClient.GetAsync($"Settings/operation-logs{queryString}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<OperationLogDto>>>();
        return apiResponse?.Data?.Select(MapToOperationLog).ToList() ?? new List<OperationLog>();
    }

    public async Task<OperationLog?> GetOperationLogAsync(string logId)
    {
        var response = await _httpClient.GetAsync($"Settings/operation-logs/{logId}");
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<OperationLogDto>>();
        return apiResponse?.Data != null ? MapToOperationLog(apiResponse.Data) : null;
    }

    public async Task<long> GetOperationLogCountAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = new List<string>();
        if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:O}");
        if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:O}");
        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
        var response = await _httpClient.GetAsync($"Settings/operation-logs/count{queryString}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<long>>();
        return apiResponse?.Data ?? 0;
    }

    public async Task<List<string>> GetDistinctModulesAsync()
    {
        var response = await _httpClient.GetAsync("Settings/operation-logs/modules");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<string>>>();
        return apiResponse?.Data ?? new List<string>();
    }

    public async Task<List<string>> GetDistinctOperationTypesAsync()
    {
        var response = await _httpClient.GetAsync("Settings/operation-logs/types");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<string>>>();
        return apiResponse?.Data ?? new List<string>();
    }

    // ============================================================================
    // System Monitoring
    // ============================================================================

    public async Task<List<SystemMonitorMetric>> GetMonitorMetricsAsync(string? metricType = null, int hours = 24)
    {
        var queryString = string.IsNullOrWhiteSpace(metricType)
            ? $"?hours={hours}"
            : $"?metricType={metricType}&hours={hours}";
        var response = await _httpClient.GetAsync($"Settings/monitor/metrics{queryString}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<MonitorMetricRawDto>>>();
        return apiResponse?.Data?.Select(MapToSystemMonitorMetric).ToList() ?? new List<SystemMonitorMetric>();
    }

    public async Task<List<ServiceStatus>> GetServiceStatusesAsync()
    {
        var response = await _httpClient.GetAsync("Settings/monitor/services");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<ServiceStatusRawDto>>>();
        return apiResponse?.Data?.Select(MapToServiceStatus).ToList() ?? new List<ServiceStatus>();
    }

    public async Task<DatabaseConnectionStatus> GetDatabaseStatusAsync()
    {
        var response = await _httpClient.GetAsync("Settings/monitor/database");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<DatabaseConnectionStatusRawDto>>();
        return apiResponse?.Data != null ? MapToDatabaseConnectionStatus(apiResponse.Data) : new DatabaseConnectionStatus();
    }

    public async Task<SystemHealthReport> GetSystemHealthReportAsync()
    {
        var response = await _httpClient.GetAsync("Settings/monitor/health");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<SystemHealthReportRawDto>>();
        return apiResponse?.Data != null ? MapToSystemHealthReport(apiResponse.Data) : new SystemHealthReport();
    }

    public async Task<List<HealthAlert>> GetHealthAlertsAsync(bool? isAcknowledged = null)
    {
        var queryString = isAcknowledged.HasValue
            ? $"?isAcknowledged={isAcknowledged.Value}"
            : string.Empty;
        var response = await _httpClient.GetAsync($"Settings/monitor/alerts{queryString}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<HealthAlertRawDto>>>();
        return apiResponse?.Data?.Select(MapToHealthAlert).ToList() ?? new List<HealthAlert>();
    }

    public async Task AcknowledgeAlertAsync(string alertId, string acknowledgedBy)
    {
        var request = new AcknowledgeAlertRequest(acknowledgedBy);
        var response = await _httpClient.PostAsJsonAsync($"Settings/monitor/alerts/{alertId}/acknowledge", request);
        response.EnsureSuccessStatusCode();
    }

    // ============================================================================
    // External Systems
    // ============================================================================

    public async Task<List<ExternalSystemConfig>> GetAllExternalSystemsAsync()
    {
        var response = await _httpClient.GetAsync("Settings/external-systems");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<ExternalSystemConfigDto>>>();
        return apiResponse?.Data?.Select(MapToExternalSystemConfig).ToList() ?? new List<ExternalSystemConfig>();
    }

    public async Task<ExternalSystemConfig?> GetExternalSystemAsync(string systemId)
    {
        var response = await _httpClient.GetAsync($"Settings/external-systems/{systemId}");
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ExternalSystemConfigDto>>();
        return apiResponse?.Data != null ? MapToExternalSystemConfig(apiResponse.Data) : null;
    }

    public async Task SaveExternalSystemAsync(ExternalSystemConfig config)
    {
        var dto = MapToExternalSystemConfigDto(config);
        var response = await _httpClient.PostAsJsonAsync("Settings/external-systems", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteExternalSystemAsync(string systemId)
    {
        var response = await _httpClient.DeleteAsync($"Settings/external-systems/{systemId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateExternalSystemStatusAsync(string systemId, bool isEnabled)
    {
        var request = new UpdateSystemStatusRequest(isEnabled);
        var response = await _httpClient.PatchAsJsonAsync($"Settings/external-systems/{systemId}/status", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<SyncRecord>> GetSyncRecordsAsync(string? systemId = null, int limit = 50)
    {
        var queryString = string.IsNullOrWhiteSpace(systemId)
            ? $"?limit={limit}"
            : $"?systemId={systemId}&limit={limit}";
        var response = await _httpClient.GetAsync($"Settings/external-systems/sync-records{queryString}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<SyncRecordDto>>>();
        return apiResponse?.Data?.Select(MapToSyncRecord).ToList() ?? new List<SyncRecord>();
    }

    public async Task<List<ExternalSystemEvent>> GetExternalEventsAsync(string? systemId = null, string? status = null)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(systemId)) queryParams.Add($"systemId={systemId}");
        if (!string.IsNullOrWhiteSpace(status)) queryParams.Add($"status={status}");
        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
        var response = await _httpClient.GetAsync($"Settings/external-systems/events{queryString}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<ExternalSystemEventDto>>>();
        return apiResponse?.Data?.Select(MapToExternalSystemEvent).ToList() ?? new List<ExternalSystemEvent>();
    }

    public async Task ProcessEventAsync(string eventId, string processedBy)
    {
        var request = new ProcessEventRequest(processedBy);
        var response = await _httpClient.PostAsJsonAsync($"Settings/external-systems/events/{eventId}/process", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task TriggerSyncAsync(string systemId, string triggeredBy)
    {
        var request = new TriggerSyncRequest(triggeredBy);
        var response = await _httpClient.PostAsJsonAsync($"Settings/external-systems/{systemId}/sync", request);
        response.EnsureSuccessStatusCode();
    }

    // ============================================================================
    // Mapping helpers: Backend DTO -> Client Model
    // ============================================================================

    private static SystemParameter MapToSystemParameter(SystemParameterDto dto) => new()
    {
        ParamId = dto.ParamId,
        ParamCode = dto.ParamCode,
        ParamName = dto.ParamName,
        ParamValue = dto.ParamValue,
        ParamType = dto.ParamType,
        Category = dto.Category,
        Description = dto.Description,
        IsSystem = dto.IsSystem,
        IsVisible = dto.IsVisible,
        DefaultValue = dto.DefaultValue,
        LastModified = dto.LastModified,
        ModifiedBy = dto.ModifiedBy,
        Remark = dto.Remark,
    };

    private static UserInfo MapToUserInfo(UserInfoDto dto) => new()
    {
        UserId = dto.UserId,
        UserName = dto.UserName,
        DisplayName = dto.DisplayName,
        Email = dto.Email,
        Department = dto.Department,
        Position = dto.Position,
        Phone = dto.Phone,
        Status = dto.Status,
        LastLogin = dto.LastLogin,
        CreatedDate = dto.CreatedDate,
        IsLocked = dto.IsLocked,
        LoginFailCount = dto.LoginFailCount,
    };

    private static RoleInfo MapToRoleInfo(RoleInfoDto dto) => new()
    {
        RoleId = dto.RoleId,
        RoleCode = dto.RoleCode,
        RoleName = dto.RoleName,
        Description = dto.Description,
        IsSystem = dto.IsSystem,
        UserCount = dto.UserCount,
        CreatedDate = dto.CreatedDate,
    };

    private static PermissionInfo MapToPermissionInfo(PermissionInfoDto dto) => new()
    {
        PermissionId = dto.PermissionId,
        PermissionCode = dto.PermissionCode,
        PermissionName = dto.PermissionName,
        Module = dto.Module,
        ActionType = dto.ActionType,
        Description = dto.Description,
    };

    private static UserRoleMapping MapToUserRoleMapping(UserRoleMappingDto dto) => new()
    {
        MappingId = dto.MappingId,
        UserId = dto.UserId,
        UserName = dto.UserName,
        RoleId = dto.RoleId,
        RoleName = dto.RoleName,
        AssignedDate = dto.AssignedDate,
        AssignedBy = dto.AssignedBy,
    };

    private static OperationLog MapToOperationLog(OperationLogDto dto) => new()
    {
        LogId = dto.LogId,
        OperationTime = dto.OperationTime,
        OperatorId = dto.OperatorId,
        OperatorName = dto.OperatorName,
        OperationType = dto.OperationType,
        Module = dto.Module,
        TargetType = dto.TargetType,
        TargetId = dto.TargetId,
        Description = dto.Description,
        IpAddress = dto.IpAddress,
        UserAgent = dto.UserAgent,
        Duration = dto.Duration,
        Result = dto.Result,
        ErrorMessage = dto.ErrorMessage,
    };

    private static SystemMonitorMetric MapToSystemMonitorMetric(MonitorMetricRawDto dto) => new()
    {
        MetricId = dto.MetricId,
        Timestamp = dto.Timestamp,
        MetricName = dto.MetricName,
        MetricType = dto.MetricType,
        Value = dto.Value,
        Unit = dto.Unit,
        Threshold = dto.Threshold,
        ServerName = dto.ServerName,
    };

    private static ServiceStatus MapToServiceStatus(ServiceStatusRawDto dto) => new()
    {
        ServiceId = dto.ServiceId,
        ServiceName = dto.ServiceName,
        Status = dto.Status,
        LastHeartbeat = dto.LastHeartbeat,
        ResponseTime = dto.ResponseTime,
        Endpoint = dto.Endpoint,
        Version = dto.Version,
        RequestCount = dto.RequestCount,
        ErrorCount = dto.ErrorCount,
    };

    private static DatabaseConnectionStatus MapToDatabaseConnectionStatus(DatabaseConnectionStatusRawDto dto) => new()
    {
        ConnectionId = dto.ConnectionId,
        DatabaseName = dto.DatabaseName,
        Status = dto.Status,
        ActiveConnections = dto.ActiveConnections,
        MaxConnections = dto.MaxConnections,
        DatabaseSizeMB = dto.DatabaseSizeMB,
        LastBackupTime = dto.LastBackupTime,
        ServerVersion = dto.ServerVersion,
        AvgQueryTime = dto.AvgQueryTime,
        SlowQueries = dto.SlowQueries,
    };

    private static SystemHealthReport MapToSystemHealthReport(SystemHealthReportRawDto dto) => new()
    {
        ReportId = dto.ReportId,
        ReportTime = dto.ReportTime,
        OverallStatus = dto.OverallStatus,
        CpuUsage = dto.CpuUsage,
        MemoryUsage = dto.MemoryUsage,
        DiskUsage = dto.DiskUsage,
        NetworkUsage = dto.NetworkUsage,
        ActiveUsers = dto.ActiveUsers,
        PendingTasks = dto.PendingTasks,
        ActiveConnections = dto.ActiveConnections,
        ErrorCount24h = dto.ErrorCount24h,
        Uptime = dto.Uptime,
        DatabaseStatus = dto.DatabaseStatus,
        CacheStatus = dto.CacheStatus,
        QueueStatus = dto.QueueStatus,
        Alerts = dto.Alerts?.Select(MapToHealthAlert).ToList() ?? [],
    };

    private static HealthAlert MapToHealthAlert(HealthAlertRawDto dto) => new()
    {
        AlertId = dto.AlertId,
        AlertTime = dto.AlertTime,
        AlertType = dto.AlertType,
        Severity = dto.Severity,
        Message = dto.Message,
        Source = dto.Source,
        IsAcknowledged = dto.IsAcknowledged,
        AcknowledgedBy = dto.AcknowledgedBy,
        AcknowledgedTime = dto.AcknowledgedTime,
    };

    private static ExternalSystemConfig MapToExternalSystemConfig(ExternalSystemConfigDto dto) => new()
    {
        SystemId = dto.SystemId,
        SystemCode = dto.SystemCode,
        SystemName = dto.SystemName,
        SystemType = dto.SystemType,
        Endpoint = dto.Endpoint,
        AuthType = dto.AuthType,
        IsEnabled = dto.IsEnabled,
        Timeout = dto.Timeout,
        RetryCount = dto.RetryCount,
        LastSyncTime = dto.LastSyncTime,
        LastSyncStatus = dto.LastSyncStatus,
        TotalSyncCount = dto.TotalSyncCount,
        FailedSyncCount = dto.FailedSyncCount,
        Remark = dto.Remark,
    };

    private static SyncRecord MapToSyncRecord(SyncRecordDto dto) => new()
    {
        RecordId = dto.RecordId,
        SystemId = dto.SystemId,
        SystemName = dto.SystemName,
        SyncTime = dto.SyncTime,
        SyncType = dto.SyncType,
        SyncStatus = dto.SyncStatus,
        RecordsCount = dto.RecordsCount,
        SuccessCount = dto.SuccessCount,
        FailedCount = dto.FailedCount,
        Duration = dto.Duration,
        ErrorMessage = dto.ErrorMessage,
        TriggeredBy = dto.TriggeredBy,
    };

    private static ExternalSystemEvent MapToExternalSystemEvent(ExternalSystemEventDto dto) => new()
    {
        EventId = dto.EventId,
        EventTime = dto.EventTime,
        SystemId = dto.SystemId,
        SystemName = dto.SystemName,
        EventType = dto.EventType,
        EventData = dto.EventData,
        ProcessingStatus = dto.ProcessingStatus,
        ProcessedBy = dto.ProcessedBy,
        ProcessedTime = dto.ProcessedTime,
        ErrorMessage = dto.ErrorMessage,
    };

    // ============================================================================
    // Mapping helpers: Client Model -> Backend DTO (for POST/PATCH)
    // ============================================================================

    private static SystemParameterDto MapToParameterDto(SystemParameter m) => new(
        m.ParamId, m.ParamCode, m.ParamName, m.ParamValue,
        m.ParamType, m.Category, m.Description, m.IsSystem,
        m.IsVisible, m.DefaultValue, m.LastModified, m.ModifiedBy, m.Remark);

    private static UserInfoDto MapToUserDto(UserInfo m) => new(
        m.UserId, m.UserName, m.DisplayName, m.Email,
        m.Department, m.Position, m.Phone, m.Status,
        m.LastLogin, m.CreatedDate, m.IsLocked, m.LoginFailCount);

    private static RoleInfoDto MapToRoleDto(RoleInfo m) => new(
        m.RoleId, m.RoleCode, m.RoleName, m.Description,
        m.IsSystem, m.UserCount, m.CreatedDate);

    private static ExternalSystemConfigDto MapToExternalSystemConfigDto(ExternalSystemConfig m) => new(
        m.SystemId, m.SystemCode, m.SystemName, m.SystemType,
        m.Endpoint, m.AuthType, m.IsEnabled, m.Timeout, m.RetryCount,
        m.LastSyncTime, m.LastSyncStatus, m.TotalSyncCount,
        m.FailedSyncCount, m.Remark);

    // ============================================================================
    // Private DTO records matching server response format
    // ============================================================================

    private record SystemParameterDto(
        string ParamId, string ParamCode, string ParamName, string ParamValue,
        string ParamType, string Category, string Description, bool IsSystem,
        bool IsVisible, string? DefaultValue, DateTime? LastModified,
        string? ModifiedBy, string Remark);

    private record UserInfoDto(
        string UserId, string UserName, string DisplayName, string Email,
        string Department, string Position, string Phone, string Status,
        DateTime? LastLogin, DateTime CreatedDate, bool IsLocked, int LoginFailCount);

    private record RoleInfoDto(
        string RoleId, string RoleCode, string RoleName, string Description,
        bool IsSystem, int UserCount, DateTime CreatedDate);

    private record PermissionInfoDto(
        string PermissionId, string PermissionCode, string PermissionName,
        string Module, string ActionType, string Description);

    private record UserRoleMappingDto(
        string MappingId, string UserId, string UserName, string RoleId,
        string RoleName, DateTime AssignedDate, string AssignedBy);

    private record OperationLogDto(
        string LogId, DateTime OperationTime, string OperatorId, string OperatorName,
        string OperationType, string Module, string TargetType, string TargetId,
        string Description, string IpAddress, string UserAgent, int Duration,
        string Result, string? ErrorMessage);

    private record ExternalSystemConfigDto(
        string SystemId, string SystemCode, string SystemName, string SystemType,
        string Endpoint, string AuthType, bool IsEnabled, int Timeout, int RetryCount,
        string? LastSyncTime, string LastSyncStatus, int TotalSyncCount,
        int FailedSyncCount, string Remark);

    private record ExternalSystemEventDto(
        string EventId, DateTime EventTime, string SystemId, string SystemName,
        string EventType, string EventData, string ProcessingStatus,
        string? ProcessedBy, DateTime? ProcessedTime, string? ErrorMessage);

    private record SyncRecordDto(
        string RecordId, string SystemId, string SystemName, DateTime SyncTime,
        string SyncType, string SyncStatus, int RecordsCount, int SuccessCount,
        int FailedCount, int Duration, string? ErrorMessage, string TriggeredBy);

    // Raw DTOs for monitor endpoints (must match server typed DTOs exactly)
    private record MonitorMetricRawDto(
        string MetricId, DateTime Timestamp, string MetricName, string MetricType,
        double Value, string Unit, double Threshold, bool IsExceeded, string ServerName, string Remark);

    private record ServiceStatusRawDto(
        string ServiceId, string ServiceName, string Status, DateTime LastHeartbeat,
        int ResponseTime, string Endpoint, string Version, int RequestCount, int ErrorCount, string Remark);

    private record DatabaseConnectionStatusRawDto(
        string ConnectionId, string DatabaseName, string Status, int ActiveConnections,
        int MaxConnections, long DatabaseSizeMB, DateTime LastBackupTime,
        string ServerVersion, double AvgQueryTime, long SlowQueries);

    private record SystemHealthReportRawDto(
        string ReportId, DateTime ReportTime, string OverallStatus, double CpuUsage,
        double MemoryUsage, double DiskUsage, double NetworkUsage, int ActiveUsers,
        int PendingTasks, int ActiveConnections, int ErrorCount24h, double Uptime,
        string DatabaseStatus, string CacheStatus, string QueueStatus,
        List<HealthAlertRawDto>? Alerts);

    private record HealthAlertRawDto(
        string AlertId, DateTime AlertTime, string AlertType, string Severity,
        string Message, string Source, bool IsAcknowledged, string? AcknowledgedBy,
        DateTime? AcknowledgedTime);

    // Request DTOs for PATCH/POST
    private record UpdateParamValueRequest(string Value, string? ModifiedBy);
    private record UpdateStatusRequest(string Status);
    private record AssignRoleRequest(string UserId, string RoleId, string? AssignedBy);
    private record AcknowledgeAlertRequest(string AcknowledgedBy);
    private record UpdateSystemStatusRequest(bool IsEnabled);
    private record ProcessEventRequest(string ProcessedBy);
    private record TriggerSyncRequest(string? TriggeredBy);
}
