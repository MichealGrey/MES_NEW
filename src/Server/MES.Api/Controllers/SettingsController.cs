using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MES.Contracts.Common;
using MES.Infrastructure.Persistence;

namespace MES.Api.Controllers;

/// <summary>
/// 系统设置 API 控制器 - 提供系统参数、用户权限、操作日志、系统监控、外部系统的管理功能。
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly MesDbContext _dbContext;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(MesDbContext dbContext, ILogger<SettingsController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    // ============================================================================
    // DTO Records
    // ============================================================================

    public record SystemParameterDto(
        string ParamId, string ParamCode, string ParamName, string ParamValue,
        string ParamType, string Category, string Description, bool IsSystem,
        bool IsVisible, string? DefaultValue, DateTime? LastModified,
        string? ModifiedBy, string Remark);

    public record UserInfoDto(
        string UserId, string UserName, string DisplayName, string Email,
        string Department, string Position, string Phone, string Status,
        DateTime? LastLogin, DateTime CreatedDate, bool IsLocked, int LoginFailCount);

    public record RoleInfoDto(
        string RoleId, string RoleCode, string RoleName, string Description,
        bool IsSystem, int UserCount, DateTime CreatedDate);

    public record PermissionInfoDto(
        string PermissionId, string PermissionCode, string PermissionName,
        string Module, string ActionType, string Description);

    public record UserRoleMappingDto(
        string MappingId, string UserId, string UserName, string RoleId,
        string RoleName, DateTime AssignedDate, string AssignedBy);

    public record OperationLogDto(
        string LogId, DateTime OperationTime, string OperatorId, string OperatorName,
        string OperationType, string Module, string TargetType, string TargetId,
        string Description, string IpAddress, string UserAgent, int Duration,
        string Result, string? ErrorMessage);

    public record ExternalSystemConfigDto(
        string SystemId, string SystemCode, string SystemName, string SystemType,
        string Endpoint, string AuthType, bool IsEnabled, int Timeout, int RetryCount,
        string? LastSyncTime, string LastSyncStatus, int TotalSyncCount,
        int FailedSyncCount, string Remark);

    public record ExternalSystemEventDto(
        string EventId, DateTime EventTime, string SystemId, string SystemName,
        string EventType, string EventData, string ProcessingStatus,
        string? ProcessedBy, DateTime? ProcessedTime, string? ErrorMessage);

    public record SyncRecordDto(
        string RecordId, string SystemId, string SystemName, DateTime SyncTime,
        string SyncType, string SyncStatus, int RecordsCount, int SuccessCount,
        int FailedCount, int Duration, string? ErrorMessage, string TriggeredBy);

    // ============================================================================
    // System Parameter CRUD
    // ============================================================================

    /// <summary>
    /// 获取所有系统参数。
    /// GET /api/settings/parameters
    /// </summary>
    [HttpGet("parameters")]
    public async Task<ApiResponse<List<SystemParameterDto>>> GetAllParameters()
    {
        var configs = await _dbContext.SystemConfigs
            .OrderBy(c => c.Category)
            .ThenBy(c => c.ConfigKey)
            .ToListAsync();

        var result = configs.Select(c => new SystemParameterDto(
            c.ConfigId,
            c.ConfigKey,
            c.ConfigKey,
            c.ConfigValue ?? string.Empty,
            c.ConfigType ?? "String",
            c.Category ?? string.Empty,
            c.Description ?? string.Empty,
            !c.IsPublic,
            c.IsPublic,
            c.ConfigValue,
            c.UpdatedAt,
            c.UpdatedBy,
            string.Empty
        )).ToList();

        return ApiResponse<List<SystemParameterDto>>.Ok(result);
    }

    /// <summary>
    /// 获取单个系统参数。
    /// GET /api/settings/parameters/{paramCode}
    /// </summary>
    [HttpGet("parameters/{paramCode}")]
    public async Task<ApiResponse<SystemParameterDto>> GetParameter(string paramCode)
    {
        if (string.IsNullOrEmpty(paramCode))
            return ApiResponse<SystemParameterDto>.Fail("参数编码不能为空");

        var config = await _dbContext.SystemConfigs
            .FirstOrDefaultAsync(c => c.ConfigKey == paramCode);

        if (config == null)
            return ApiResponse<SystemParameterDto>.Fail("参数未找到");

        var dto = new SystemParameterDto(
            config.ConfigId,
            config.ConfigKey,
            config.ConfigKey,
            config.ConfigValue ?? string.Empty,
            config.ConfigType ?? "String",
            config.Category ?? string.Empty,
            config.Description ?? string.Empty,
            !config.IsPublic,
            config.IsPublic,
            config.ConfigValue,
            config.UpdatedAt,
            config.UpdatedBy,
            string.Empty
        );

        return ApiResponse<SystemParameterDto>.Ok(dto);
    }

    /// <summary>
    /// 保存系统参数（创建或更新）。
    /// POST /api/settings/parameters
    /// </summary>
    [HttpPost("parameters")]
    public async Task<ApiResponse<bool>> SaveParameter([FromBody] SystemParameterDto dto)
    {
        if (string.IsNullOrEmpty(dto.ParamCode))
            return ApiResponse<bool>.Fail("参数编码不能为空");

        var existing = await _dbContext.SystemConfigs
            .FirstOrDefaultAsync(c => c.ConfigKey == dto.ParamCode);

        if (existing == null)
        {
            var newConfig = new MES.Infrastructure.Persistence.Entities.SystemConfig
            {
                ConfigId = $"CFG-{Guid.NewGuid():N}"[..18],
                ConfigKey = dto.ParamCode,
                ConfigValue = dto.ParamValue,
                ConfigType = dto.ParamType,
                Category = dto.Category,
                Description = dto.Description,
                IsPublic = dto.IsVisible,
                UpdatedBy = dto.ModifiedBy ?? "admin",
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.SystemConfigs.Add(newConfig);
        }
        else
        {
            existing.ConfigValue = dto.ParamValue;
            existing.ConfigType = dto.ParamType;
            existing.Category = dto.Category;
            existing.Description = dto.Description;
            existing.IsPublic = dto.IsVisible;
            existing.UpdatedBy = dto.ModifiedBy ?? "admin";
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    /// <summary>
    /// 删除系统参数。
    /// DELETE /api/settings/parameters/{paramCode}
    /// </summary>
    [HttpDelete("parameters/{paramCode}")]
    public async Task<ApiResponse<bool>> DeleteParameter(string paramCode)
    {
        if (string.IsNullOrEmpty(paramCode))
            return ApiResponse<bool>.Fail("参数编码不能为空");

        var config = await _dbContext.SystemConfigs
            .FirstOrDefaultAsync(c => c.ConfigKey == paramCode);

        if (config == null)
            return ApiResponse<bool>.Fail("参数未找到");

        // 不允许删除系统级参数（非公开参数视为系统参数）
        if (!config.IsPublic)
            return ApiResponse<bool>.Fail("系统参数不允许删除");

        _dbContext.SystemConfigs.Remove(config);
        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    /// <summary>
    /// 更新参数值。
    /// PATCH /api/settings/parameters/{paramCode}/value
    /// </summary>
    [HttpPatch("parameters/{paramCode}/value")]
    public async Task<ApiResponse<bool>> UpdateParameterValue(string paramCode, [FromBody] UpdateParamValueRequest request)
    {
        if (string.IsNullOrEmpty(paramCode))
            return ApiResponse<bool>.Fail("参数编码不能为空");

        var config = await _dbContext.SystemConfigs
            .FirstOrDefaultAsync(c => c.ConfigKey == paramCode);

        if (config == null)
            return ApiResponse<bool>.Fail("参数未找到");

        config.ConfigValue = request.Value;
        config.UpdatedBy = request.ModifiedBy ?? "admin";
        config.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    public record UpdateParamValueRequest(string Value, string? ModifiedBy);

    // Monitoring DTO Records
    public record SystemMonitorMetricDto(
        string MetricId, DateTime Timestamp, string MetricName, string MetricType,
        double Value, string Unit, double Threshold, bool IsExceeded, string ServerName, string Remark);

    public record ServiceStatusDto(
        string ServiceId, string ServiceName, string Status, DateTime LastHeartbeat,
        int ResponseTime, string Endpoint, string Version, int RequestCount, int ErrorCount, string Remark);

    public record DatabaseConnectionStatusDto(
        string ConnectionId, string DatabaseName, string Status,
        int ActiveConnections, int MaxConnections, long DatabaseSizeMB,
        DateTime LastBackupTime, string ServerVersion, double AvgQueryTime, long SlowQueries);

    public record SystemHealthReportDto(
        string ReportId, DateTime ReportTime, string OverallStatus,
        double CpuUsage, double MemoryUsage, double DiskUsage, double NetworkUsage,
        int ActiveUsers, int PendingTasks, int ActiveConnections, int ErrorCount24h,
        double Uptime, string DatabaseStatus, string CacheStatus, string QueueStatus,
        List<HealthAlertDto> Alerts);

    public record HealthAlertDto(
        string AlertId, DateTime AlertTime, string AlertType, string Severity,
        string Message, string Source, bool IsAcknowledged, string? AcknowledgedBy,
        DateTime? AcknowledgedTime);

    // ============================================================================
    // User Permission CRUD
    // ============================================================================

    /// <summary>
    /// 获取所有用户。
    /// GET /api/settings/users
    /// </summary>
    [HttpGet("users")]
    public async Task<ApiResponse<List<UserInfoDto>>> GetAllUsers()
    {
        var query = from user in _dbContext.SysUsers
                    join dept in _dbContext.SysDepartments on user.DeptId equals dept.DeptId into dd
                    from dept in dd.DefaultIfEmpty()
                    select new { user, dept };

        var result = await query.ToListAsync();

        var dtos = result.Select(x => new UserInfoDto(
            x.user.UserId,
            x.user.UserName,
            x.user.UserName, // DisplayName derived from UserName
            string.Empty, // Email not in entity
            x.dept?.DeptName ?? string.Empty,
            string.Empty, // Position not in entity
            string.Empty, // Phone not in entity
            x.user.IsActive ? "Active" : "Inactive",
            null, // LastLogin not in entity
            x.user.CreatedAt,
            false, // IsLocked not in entity
            0 // LoginFailCount not in entity
        )).ToList();

        return ApiResponse<List<UserInfoDto>>.Ok(dtos);
    }

    /// <summary>
    /// 获取单个用户。
    /// GET /api/settings/users/{userId}
    /// </summary>
    [HttpGet("users/{userId}")]
    public async Task<ApiResponse<UserInfoDto>> GetUser(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return ApiResponse<UserInfoDto>.Fail("用户ID不能为空");

        var user = await _dbContext.SysUsers.FindAsync(userId);
        if (user == null)
            return ApiResponse<UserInfoDto>.Fail("用户未找到");

        var dept = await _dbContext.SysDepartments.FindAsync(user.DeptId);

        var dto = new UserInfoDto(
            user.UserId,
            user.UserName,
            user.UserName,
            string.Empty,
            dept?.DeptName ?? string.Empty,
            string.Empty,
            string.Empty,
            user.IsActive ? "Active" : "Inactive",
            null,
            user.CreatedAt,
            false,
            0
        );

        return ApiResponse<UserInfoDto>.Ok(dto);
    }

    /// <summary>
    /// 保存用户（创建或更新）。
    /// POST /api/settings/users
    /// </summary>
    [HttpPost("users")]
    public async Task<ApiResponse<bool>> SaveUser([FromBody] UserInfoDto dto)
    {
        if (string.IsNullOrEmpty(dto.UserId))
            return ApiResponse<bool>.Fail("用户ID不能为空");

        var existing = await _dbContext.SysUsers.FindAsync(dto.UserId);

        if (existing == null)
        {
            var newUser = new MES.Infrastructure.Persistence.Entities.SysUser
            {
                UserId = dto.UserId,
                UserName = dto.UserName,
                DeptId = string.Empty,
                RoleId = string.Empty,
                IsActive = dto.Status == "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.SysUsers.Add(newUser);
        }
        else
        {
            existing.UserName = dto.UserName;
            existing.IsActive = dto.Status == "Active";
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    /// <summary>
    /// 删除用户。
    /// DELETE /api/settings/users/{userId}
    /// </summary>
    [HttpDelete("users/{userId}")]
    public async Task<ApiResponse<bool>> DeleteUser(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return ApiResponse<bool>.Fail("用户ID不能为空");

        var user = await _dbContext.SysUsers.FindAsync(userId);
        if (user == null)
            return ApiResponse<bool>.Fail("用户未找到");

        if (user.UserName == "admin")
            return ApiResponse<bool>.Fail("不允许删除管理员账号");

        // 删除用户角色映射
        var userRoles = await _dbContext.SysUserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync();
        _dbContext.SysUserRoles.RemoveRange(userRoles);

        _dbContext.SysUsers.Remove(user);
        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    /// <summary>
    /// 更新用户状态。
    /// PATCH /api/settings/users/{userId}/status
    /// </summary>
    [HttpPatch("users/{userId}/status")]
    public async Task<ApiResponse<bool>> UpdateUserStatus(string userId, [FromBody] UpdateStatusRequest request)
    {
        if (string.IsNullOrEmpty(userId))
            return ApiResponse<bool>.Fail("用户ID不能为空");

        var user = await _dbContext.SysUsers.FindAsync(userId);
        if (user == null)
            return ApiResponse<bool>.Fail("用户未找到");

        user.IsActive = request.Status == "Active";
        user.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    public record UpdateStatusRequest(string Status);

    /// <summary>
    /// 获取所有角色（含用户数量）。
    /// GET /api/settings/roles
    /// </summary>
    [HttpGet("roles")]
    public async Task<ApiResponse<List<RoleInfoDto>>> GetAllRoles()
    {
        var userRoleCounts = await _dbContext.SysUserRoles
            .GroupBy(ur => ur.RoleId)
            .Select(g => new { RoleId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RoleId, x => x.Count);

        var roles = await _dbContext.SysRoles
            .OrderBy(r => r.RoleName)
            .ToListAsync();

        var dtos = roles.Select(r => new RoleInfoDto(
            r.RoleId,
            r.RoleId, // RoleCode derived from RoleId
            r.RoleName,
            r.Description ?? string.Empty,
            r.Level >= 1, // IsSystem based on level
            userRoleCounts.GetValueOrDefault(r.RoleId, 0),
            r.CreatedAt
        )).ToList();

        return ApiResponse<List<RoleInfoDto>>.Ok(dtos);
    }

    /// <summary>
    /// 保存角色。
    /// POST /api/settings/roles
    /// </summary>
    [HttpPost("roles")]
    public async Task<ApiResponse<bool>> SaveRole([FromBody] RoleInfoDto dto)
    {
        if (string.IsNullOrEmpty(dto.RoleId))
            return ApiResponse<bool>.Fail("角色ID不能为空");

        var existing = await _dbContext.SysRoles.FindAsync(dto.RoleId);

        if (existing == null)
        {
            var newRole = new MES.Infrastructure.Persistence.Entities.SysRole
            {
                RoleId = dto.RoleId,
                RoleName = dto.RoleName,
                Description = dto.Description,
                Level = dto.IsSystem ? 1 : 0,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.SysRoles.Add(newRole);
        }
        else
        {
            existing.RoleName = dto.RoleName;
            existing.Description = dto.Description;
        }

        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    /// <summary>
    /// 删除角色。
    /// DELETE /api/settings/roles/{roleId}
    /// </summary>
    [HttpDelete("roles/{roleId}")]
    public async Task<ApiResponse<bool>> DeleteRole(string roleId)
    {
        if (string.IsNullOrEmpty(roleId))
            return ApiResponse<bool>.Fail("角色ID不能为空");

        var role = await _dbContext.SysRoles.FindAsync(roleId);
        if (role == null)
            return ApiResponse<bool>.Fail("角色未找到");

        if (role.Level >= 1)
            return ApiResponse<bool>.Fail("系统角色不允许删除");

        var userRoles = await _dbContext.SysUserRoles
            .Where(ur => ur.RoleId == roleId)
            .ToListAsync();
        _dbContext.SysUserRoles.RemoveRange(userRoles);

        _dbContext.SysRoles.Remove(role);
        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    /// <summary>
    /// 获取所有权限（从菜单表按模块分组）。
    /// GET /api/settings/permissions
    /// </summary>
    [HttpGet("permissions")]
    public async Task<ApiResponse<List<PermissionInfoDto>>> GetAllPermissions()
    {
        var menus = await _dbContext.SysMenus
            .Where(m => !string.IsNullOrEmpty(m.PermissionCode))
            .OrderBy(m => m.ModuleKey)
            .ThenBy(m => m.SortOrder)
            .ToListAsync();

        var dtos = menus.Select(m => new PermissionInfoDto(
            m.MenuId,
            m.PermissionCode ?? string.Empty,
            m.MenuName,
            m.ModuleKey ?? string.Empty,
            "Full", // ActionType default
            m.ViewName ?? string.Empty
        )).ToList();

        return ApiResponse<List<PermissionInfoDto>>.Ok(dtos);
    }

    /// <summary>
    /// 获取用户角色映射。
    /// GET /api/settings/user-roles?userId=
    /// </summary>
    [HttpGet("user-roles")]
    public async Task<ApiResponse<List<UserRoleMappingDto>>> GetUserRoleMappings([FromQuery] string? userId = null)
    {
        var query = from ur in _dbContext.SysUserRoles
                    join user in _dbContext.SysUsers on ur.UserId equals user.UserId into uu
                    from user in uu.DefaultIfEmpty()
                    join role in _dbContext.SysRoles on ur.RoleId equals role.RoleId into rr
                    from role in rr.DefaultIfEmpty()
                    select new { ur, user, role };

        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(x => x.ur.UserId == userId);

        var result = await query.ToListAsync();

        var dtos = result.Select(x => new UserRoleMappingDto(
            x.ur.Id.ToString(),
            x.ur.UserId,
            x.user?.UserName ?? x.ur.UserId,
            x.ur.RoleId,
            x.role?.RoleName ?? x.ur.RoleId,
            x.ur.CreatedAt,
            string.Empty // AssignedBy not available in entity
        )).ToList();

        return ApiResponse<List<UserRoleMappingDto>>.Ok(dtos);
    }

    /// <summary>
    /// 分配角色给用户。
    /// POST /api/settings/user-roles
    /// </summary>
    [HttpPost("user-roles")]
    public async Task<ApiResponse<bool>> AssignRoleToUser([FromBody] AssignRoleRequest request)
    {
        if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.RoleId))
            return ApiResponse<bool>.Fail("用户ID和角色ID不能为空");

        var existing = await _dbContext.SysUserRoles
            .AnyAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId);

        if (existing)
            return ApiResponse<bool>.Ok(true); // Already assigned

        var mapping = new MES.Infrastructure.Persistence.Entities.SysUserRole
        {
            UserId = request.UserId,
            RoleId = request.RoleId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.SysUserRoles.Add(mapping);
        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    public record AssignRoleRequest(string UserId, string RoleId, string? AssignedBy);

    /// <summary>
    /// 移除用户角色映射。
    /// DELETE /api/settings/user-roles/{mappingId}
    /// </summary>
    [HttpDelete("user-roles/{mappingId}")]
    public async Task<ApiResponse<bool>> RemoveRoleFromUser(long mappingId)
    {
        var mapping = await _dbContext.SysUserRoles.FindAsync(mappingId);
        if (mapping == null)
            return ApiResponse<bool>.Fail("映射未找到");

        _dbContext.SysUserRoles.Remove(mapping);
        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    // ============================================================================
    // Operation Logs (combined from SysLoginLog + ProdAuditTrail)
    // ============================================================================

    /// <summary>
    /// 获取操作日志（登录日志 + 审计日志合并）。
    /// GET /api/settings/operation-logs?startDate=&endDate=&module=&operatorName=&operationType=
    /// </summary>
    [HttpGet("operation-logs")]
    public async Task<ApiResponse<List<OperationLogDto>>> GetOperationLogs(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? module = null,
        [FromQuery] string? operatorName = null,
        [FromQuery] string? operationType = null)
    {
        var result = new List<OperationLogDto>();

        // 登录日志
        var loginQuery = _dbContext.SysLoginLogs.AsQueryable();
        if (startDate.HasValue) loginQuery = loginQuery.Where(l => l.LoginTime >= startDate.Value);
        if (endDate.HasValue) loginQuery = loginQuery.Where(l => l.LoginTime <= endDate.Value);

        var loginLogs = await loginQuery.ToListAsync();
        foreach (var log in loginLogs)
        {
            result.Add(new OperationLogDto(
                $"LOGIN-{log.Id}",
                log.LoginTime ?? DateTime.MinValue,
                log.UserId ?? string.Empty,
                log.UserId ?? string.Empty,
                log.Result == "Success" ? "Login" : "LoginFailed",
                "系统设置",
                "User",
                log.UserId ?? string.Empty,
                log.ErrorMessage ?? "用户登录",
                log.IpAddress ?? string.Empty,
                string.Empty,
                0,
                log.Result ?? string.Empty,
                log.ErrorMessage
            ));
        }

        // 审计日志
        var auditQuery = _dbContext.ProdAuditTrails.AsQueryable();
        if (startDate.HasValue) auditQuery = auditQuery.Where(a => a.Timestamp >= startDate.Value);
        if (endDate.HasValue) auditQuery = auditQuery.Where(a => a.Timestamp <= endDate.Value);
        if (!string.IsNullOrWhiteSpace(module)) auditQuery = auditQuery.Where(a => a.EntityType == module);
        if (!string.IsNullOrWhiteSpace(operatorName)) auditQuery = auditQuery.Where(a => a.OperatorName != null && a.OperatorName.Contains(operatorName));
        if (!string.IsNullOrWhiteSpace(operationType)) auditQuery = auditQuery.Where(a => a.Action == operationType);

        var auditLogs = await auditQuery
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

        foreach (var log in auditLogs)
        {
            result.Add(new OperationLogDto(
                log.AuditId,
                log.Timestamp,
                log.OperatorId,
                log.OperatorName ?? string.Empty,
                log.Action,
                log.EntityType,
                log.EntityType,
                log.EntityId,
                log.Reason ?? (log.Detail != null ? log.Detail.Substring(0, Math.Min(100, log.Detail.Length)) : string.Empty) ?? string.Empty,
                string.Empty,
                string.Empty,
                0,
                "Success",
                log.ApprovedBy != null ? $"审批人: {log.ApprovedBy}" : null
            ));
        }

        var sortedResult = result
            .OrderByDescending(l => l.OperationTime)
            .ToList();

        return ApiResponse<List<OperationLogDto>>.Ok(sortedResult);
    }

    /// <summary>
    /// 获取单个操作日志详情。
    /// GET /api/settings/operation-logs/{logId}
    /// </summary>
    [HttpGet("operation-logs/{logId}")]
    public async Task<ApiResponse<OperationLogDto>> GetOperationLog(string logId)
    {
        if (string.IsNullOrEmpty(logId))
            return ApiResponse<OperationLogDto>.Fail("日志ID不能为空");

        // 先查审计日志
        if (!logId.StartsWith("LOGIN-"))
        {
            var audit = await _dbContext.ProdAuditTrails.FindAsync(logId);
            if (audit != null)
            {
                var dto = new OperationLogDto(
                    audit.AuditId,
                    audit.Timestamp,
                    audit.OperatorId,
                    audit.OperatorName ?? string.Empty,
                    audit.Action,
                    audit.EntityType,
                    audit.EntityType,
                    audit.EntityId,
                    audit.Reason ?? (audit.Detail != null ? audit.Detail.Substring(0, Math.Min(100, audit.Detail.Length)) : string.Empty) ?? string.Empty,
                    string.Empty,
                    string.Empty,
                    0,
                    "Success",
                    audit.ApprovedBy != null ? $"审批人: {audit.ApprovedBy}" : null
                );
                return ApiResponse<OperationLogDto>.Ok(dto);
            }
        }

        // 查登录日志
        if (logId.StartsWith("LOGIN-") && long.TryParse(logId["LOGIN-".Length..], out var loginId))
        {
            var login = await _dbContext.SysLoginLogs.FindAsync(loginId);
            if (login != null)
            {
                var dto = new OperationLogDto(
                    logId,
                    login.LoginTime ?? DateTime.MinValue,
                    login.UserId ?? string.Empty,
                    login.UserId ?? string.Empty,
                    login.Result == "Success" ? "Login" : "LoginFailed",
                    "系统设置",
                    "User",
                    login.UserId ?? string.Empty,
                    login.ErrorMessage ?? "用户登录",
                    login.IpAddress ?? string.Empty,
                    string.Empty,
                    0,
                    login.Result ?? string.Empty,
                    login.ErrorMessage
                );
                return ApiResponse<OperationLogDto>.Ok(dto);
            }
        }

        return ApiResponse<OperationLogDto>.Fail("日志未找到");
    }

    /// <summary>
    /// 获取操作日志数量。
    /// GET /api/settings/operation-logs/count
    /// </summary>
    [HttpGet("operation-logs/count")]
    public async Task<ApiResponse<long>> GetOperationLogCount(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var loginCount = await _dbContext.SysLoginLogs.CountAsync();
        var auditCountQuery = _dbContext.ProdAuditTrails.AsQueryable();
        if (startDate.HasValue) auditCountQuery = auditCountQuery.Where(a => a.Timestamp >= startDate.Value);
        if (endDate.HasValue) auditCountQuery = auditCountQuery.Where(a => a.Timestamp <= endDate.Value);
        var auditCount = await auditCountQuery.CountAsync();

        return ApiResponse<long>.Ok(loginCount + auditCount);
    }

    /// <summary>
    /// 获取不同的模块列表。
    /// GET /api/settings/operation-logs/modules
    /// </summary>
    [HttpGet("operation-logs/modules")]
    public async Task<ApiResponse<List<string>>> GetDistinctModules()
    {
        var modules = await _dbContext.ProdAuditTrails
            .Select(a => a.EntityType)
            .Where(e => !string.IsNullOrEmpty(e))
            .Distinct()
            .ToListAsync();

        modules.Add("系统设置");
        modules.Sort();

        return ApiResponse<List<string>>.Ok(modules);
    }

    /// <summary>
    /// 获取不同的操作类型列表。
    /// GET /api/settings/operation-logs/types
    /// </summary>
    [HttpGet("operation-logs/types")]
    public async Task<ApiResponse<List<string>>> GetDistinctOperationTypes()
    {
        var types = await _dbContext.ProdAuditTrails
            .Select(a => a.Action)
            .Where(a => !string.IsNullOrEmpty(a))
            .Distinct()
            .ToListAsync();

        types.Add("Login");
        types.Add("LoginFailed");
        types.Sort();

        return ApiResponse<List<string>>.Ok(types);
    }

    // ============================================================================
    // System Monitor (mock/runtime data)
    // ============================================================================

    /// <summary>
    /// 获取监控指标（模拟数据）。
    /// GET /api/settings/monitor/metrics?metricType=&hours=24
    /// </summary>
    [HttpGet("monitor/metrics")]
    public async Task<ApiResponse<List<SystemMonitorMetricDto>>> GetMonitorMetrics(
        [FromQuery] string? metricType = null,
        [FromQuery] int hours = 24)
    {
        await Task.Delay(1);
        var rnd = new Random(42);
        var metricTypes = string.IsNullOrWhiteSpace(metricType)
            ? new[] { "CPU", "Memory", "Disk", "Network", "Requests", "Errors" }
            : new[] { metricType };

        var result = new List<SystemMonitorMetricDto>();

        foreach (var type in metricTypes)
        {
            for (int i = 0; i < 20; i++)
            {
                double value = type switch
                {
                    "CPU" => rnd.NextDouble() * 80 + 10,
                    "Memory" => rnd.NextDouble() * 40 + 40,
                    "Disk" => rnd.NextDouble() * 30 + 50,
                    "Network" => rnd.NextDouble() * 100,
                    "Requests" => rnd.Next(100, 10000),
                    "Errors" => rnd.Next(0, 20),
                    _ => rnd.NextDouble() * 100
                };

                var threshold = type switch { "CPU" => 85.0, "Memory" => 90.0, "Disk" => 90.0, _ => 100.0 };

                result.Add(new SystemMonitorMetricDto(
                    $"MET-{DateTime.Now:yyyyMMdd}{type}{i:D4}",
                    DateTime.Now.AddHours(-rnd.Next(0, hours)),
                    $"{type} Usage",
                    type,
                    Math.Round(value, 2),
                    type switch { "CPU" => "%", "Memory" => "%", "Disk" => "%", "Network" => "Mbps", _ => "" },
                    threshold,
                    value > threshold,
                    "MES-Server-01",
                    string.Empty
                ));
            }
        }

        return ApiResponse<List<SystemMonitorMetricDto>>.Ok(result.OrderBy(x => x.Timestamp).ToList());
    }

    /// <summary>
    /// 获取服务状态（模拟数据）。
    /// GET /api/settings/monitor/services
    /// </summary>
    [HttpGet("monitor/services")]
    public async Task<ApiResponse<List<ServiceStatusDto>>> GetServiceStatuses()
    {
        await Task.Delay(1);
        var now = DateTime.Now;
        var result = new List<ServiceStatusDto>
        {
            new("SVC001", "MES API Service", "Running", now.AddSeconds(-5), 45, "http://localhost:5000/api", "3.2.1", 1523456, 234, string.Empty),
            new("SVC002", "Equipment Gateway", "Running", now.AddSeconds(-10), 120, "http://localhost:5001/gateway", "2.1.0", 523456, 89, string.Empty),
            new("SVC003", "Recipe Service", "Running", now.AddSeconds(-15), 78, "http://localhost:5002/recipe", "1.5.0", 123456, 12, string.Empty),
            new("SVC004", "Quality Service", "Running", now.AddSeconds(-8), 95, "http://localhost:5003/quality", "2.0.3", 345678, 45, string.Empty),
            new("SVC005", "Report Service", "Degraded", now.AddSeconds(-30), 500, "http://localhost:5004/report", "1.3.0", 89012, 156, string.Empty),
            new("SVC006", "Notification Service", "Stopped", now.AddHours(-2), 0, "http://localhost:5005/notify", "1.1.0", 45678, 2345, string.Empty),
        };

        return ApiResponse<List<ServiceStatusDto>>.Ok(result);
    }

    /// <summary>
    /// 获取数据库状态。
    /// GET /api/settings/monitor/database
    /// </summary>
    [HttpGet("monitor/database")]
    public async Task<ApiResponse<DatabaseConnectionStatusDto>> GetDatabaseStatus()
    {
        var result = new DatabaseConnectionStatusDto(
            "DB001",
            "mes_prod",
            "Connected",
            45,
            200,
            15360,
            DateTime.Now.AddHours(-6),
            "MySQL 8.0.35",
            12.5,
            23
        );

        return ApiResponse<DatabaseConnectionStatusDto>.Ok(result);
    }

    /// <summary>
    /// 获取系统健康报告（聚合数据）。
    /// GET /api/settings/monitor/health
    /// </summary>
    [HttpGet("monitor/health")]
    public async Task<ApiResponse<SystemHealthReportDto>> GetSystemHealthReport()
    {
        var totalUsers = await _dbContext.SysUsers.CountAsync(u => u.IsActive);
        var totalAlarms = await _dbContext.ProdAlarms.CountAsync(a => a.Status != "Resolved");

        var alerts = new List<HealthAlertDto>
        {
            new("ALT001", DateTime.Now.AddHours(-1), "Performance", "Warning", "Report Service 响应时间超过阈值", "MES-Server-01", false, null, null),
            new("ALT002", DateTime.Now.AddHours(-2), "Service", "Critical", "Notification Service 服务已停止", "MES-Server-01", true, "admin", DateTime.Now.AddHours(-1.5)),
            new("ALT003", DateTime.Now.AddHours(-3), "Database", "Warning", "慢查询数量增加", "MySQL", false, null, null),
        };

        var report = new SystemHealthReportDto(
            $"HR-{DateTime.Now:yyyyMMdd}",
            DateTime.Now,
            "Warning",
            62.5,
            73.2,
            81.5,
            45.8,
            totalUsers,
            12,
            45,
            67,
            99.85,
            "Connected",
            "Active",
            "Processing",
            alerts
        );

        return ApiResponse<SystemHealthReportDto>.Ok(report);
    }

    /// <summary>
    /// 获取健康告警（模拟数据）。
    /// GET /api/settings/monitor/alerts?isAcknowledged=
    /// </summary>
    [HttpGet("monitor/alerts")]
    public async Task<ApiResponse<List<HealthAlertDto>>> GetHealthAlerts([FromQuery] bool? isAcknowledged = null)
    {
        await Task.Delay(1);
        var now = DateTime.Now;

        var alerts = new List<HealthAlertDto>
        {
            new("ALT001", now.AddHours(-1), "Performance", "Warning", "Report Service 响应时间超过阈值", "MES-Server-01", false, null, null),
            new("ALT002", now.AddHours(-2), "Service", "Critical", "Notification Service 服务已停止", "MES-Server-01", true, "admin", now.AddHours(-1.5)),
            new("ALT003", now.AddHours(-3), "Database", "Warning", "慢查询数量增加", "MySQL", false, null, null),
            new("ALT004", now.AddHours(-5), "Disk", "Warning", "磁盘使用率超过80%", "MES-Server-01", true, "admin", now.AddHours(-4)),
        };

        var filtered = isAcknowledged.HasValue
            ? alerts.Where(a => a.IsAcknowledged == isAcknowledged.Value).ToList()
            : alerts;

        return ApiResponse<List<HealthAlertDto>>.Ok(filtered);
    }

    /// <summary>
    /// 确认健康告警。
    /// POST /api/settings/monitor/alerts/{alertId}/acknowledge
    /// </summary>
    [HttpPost("monitor/alerts/{alertId}/acknowledge")]
    public async Task<ApiResponse<bool>> AcknowledgeAlert(string alertId, [FromBody] AcknowledgeAlertRequest request)
    {
        // 模拟操作，无持久化
        await Task.Delay(1);
        _logger.LogInformation("Alert {AlertId} acknowledged by {AcknowledgedBy}", alertId, request.AcknowledgedBy);
        return ApiResponse<bool>.Ok(true);
    }

    public record AcknowledgeAlertRequest(string AcknowledgedBy);

    // ============================================================================
    // External System
    // ============================================================================

    /// <summary>
    /// 获取所有外部系统配置。
    /// GET /api/settings/external-systems
    /// </summary>
    [HttpGet("external-systems")]
    public async Task<ApiResponse<List<ExternalSystemConfigDto>>> GetAllExternalSystems()
    {
        var configs = await _dbContext.ExtSystemConfigs
            .OrderBy(c => c.SystemName)
            .ToListAsync();

        var dtos = configs.Select(c => new ExternalSystemConfigDto(
            c.SystemId,
            c.SystemId, // SystemCode derived from SystemId
            c.SystemName,
            c.SystemType,
            c.Endpoint,
            c.AuthType,
            c.IsEnabled,
            c.TimeoutSeconds,
            c.MaxRetries,
            null, // LastSyncTime not in entity
            string.Empty, // LastSyncStatus not in entity
            0, // TotalSyncCount not in entity
            0, // FailedSyncCount not in entity
            string.Empty
        )).ToList();

        return ApiResponse<List<ExternalSystemConfigDto>>.Ok(dtos);
    }

    /// <summary>
    /// 获取单个外部系统配置。
    /// GET /api/settings/external-systems/{systemId}
    /// </summary>
    [HttpGet("external-systems/{systemId}")]
    public async Task<ApiResponse<ExternalSystemConfigDto>> GetExternalSystem(string systemId)
    {
        if (string.IsNullOrEmpty(systemId))
            return ApiResponse<ExternalSystemConfigDto>.Fail("系统ID不能为空");

        var config = await _dbContext.ExtSystemConfigs.FindAsync(systemId);
        if (config == null)
            return ApiResponse<ExternalSystemConfigDto>.Fail("外部系统配置未找到");

        var dto = new ExternalSystemConfigDto(
            config.SystemId,
            config.SystemId,
            config.SystemName,
            config.SystemType,
            config.Endpoint,
            config.AuthType,
            config.IsEnabled,
            config.TimeoutSeconds,
            config.MaxRetries,
            null,
            string.Empty,
            0,
            0,
            string.Empty
        );

        return ApiResponse<ExternalSystemConfigDto>.Ok(dto);
    }

    /// <summary>
    /// 保存外部系统配置。
    /// POST /api/settings/external-systems
    /// </summary>
    [HttpPost("external-systems")]
    public async Task<ApiResponse<bool>> SaveExternalSystem([FromBody] ExternalSystemConfigDto dto)
    {
        if (string.IsNullOrEmpty(dto.SystemId))
            return ApiResponse<bool>.Fail("系统ID不能为空");

        var existing = await _dbContext.ExtSystemConfigs.FindAsync(dto.SystemId);

        if (existing == null)
        {
            var newConfig = new MES.Infrastructure.Persistence.Entities.ExtSystemConfig
            {
                SystemId = dto.SystemId,
                SystemName = dto.SystemName,
                SystemType = dto.SystemType,
                Endpoint = dto.Endpoint,
                AuthType = dto.AuthType,
                IsEnabled = dto.IsEnabled,
                TimeoutSeconds = dto.Timeout,
                MaxRetries = dto.RetryCount,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.ExtSystemConfigs.Add(newConfig);
        }
        else
        {
            existing.SystemName = dto.SystemName;
            existing.SystemType = dto.SystemType;
            existing.Endpoint = dto.Endpoint;
            existing.AuthType = dto.AuthType;
            existing.IsEnabled = dto.IsEnabled;
            existing.TimeoutSeconds = dto.Timeout;
            existing.MaxRetries = dto.RetryCount;
        }

        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    /// <summary>
    /// 删除外部系统配置。
    /// DELETE /api/settings/external-systems/{systemId}
    /// </summary>
    [HttpDelete("external-systems/{systemId}")]
    public async Task<ApiResponse<bool>> DeleteExternalSystem(string systemId)
    {
        if (string.IsNullOrEmpty(systemId))
            return ApiResponse<bool>.Fail("系统ID不能为空");

        var config = await _dbContext.ExtSystemConfigs.FindAsync(systemId);
        if (config == null)
            return ApiResponse<bool>.Fail("外部系统配置未找到");

        _dbContext.ExtSystemConfigs.Remove(config);
        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    /// <summary>
    /// 更新外部系统状态。
    /// PATCH /api/settings/external-systems/{systemId}/status
    /// </summary>
    [HttpPatch("external-systems/{systemId}/status")]
    public async Task<ApiResponse<bool>> UpdateExternalSystemStatus(string systemId, [FromBody] UpdateSystemStatusRequest request)
    {
        if (string.IsNullOrEmpty(systemId))
            return ApiResponse<bool>.Fail("系统ID不能为空");

        var config = await _dbContext.ExtSystemConfigs.FindAsync(systemId);
        if (config == null)
            return ApiResponse<bool>.Fail("外部系统配置未找到");

        config.IsEnabled = request.IsEnabled;
        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    public record UpdateSystemStatusRequest(bool IsEnabled);

    /// <summary>
    /// 获取同步记录（模拟数据）。
    /// GET /api/settings/external-systems/sync-records?systemId=&limit=50
    /// </summary>
    [HttpGet("external-systems/sync-records")]
    public async Task<ApiResponse<List<SyncRecordDto>>> GetSyncRecords(
        [FromQuery] string? systemId = null,
        [FromQuery] int limit = 50)
    {
        await Task.Delay(1);
        var rnd = new Random(42);

        // 获取外部系统列表用于模拟数据
        var extSystems = await _dbContext.ExtSystemConfigs.ToListAsync();
        var systems = extSystems.Count > 0 ? extSystems : new List<MES.Infrastructure.Persistence.Entities.ExtSystemConfig>
        {
            new() { SystemId = "ES001", SystemName = "ERP系统" },
            new() { SystemId = "ES002", SystemName = "设备自动化平台" },
            new() { SystemId = "ES003", SystemName = "仓储管理系统" },
        };

        var syncTypes = new[] { "WorkOrder", "Lot", "Product", "Material", "Equipment", "Quality" };
        var syncStatuses = new[] { "Success", "Failed", "Partial" };

        var result = new List<SyncRecordDto>();
        for (int i = 0; i < Math.Min(limit, 30); i++)
        {
            var sysIdx = rnd.Next(systems.Count);
            var sys = systems[sysIdx];
            var status = syncStatuses[rnd.Next(syncStatuses.Length)];
            var count = rnd.Next(10, 500);

            if (!string.IsNullOrWhiteSpace(systemId) && sys.SystemId != systemId)
                continue;

            result.Add(new SyncRecordDto(
                $"SR-{i + 1:D6}",
                sys.SystemId,
                sys.SystemName,
                DateTime.Now.AddHours(-rnd.Next(0, 72)),
                syncTypes[rnd.Next(syncTypes.Length)],
                status,
                count,
                status == "Failed" ? 0 : status == "Partial" ? rnd.Next(1, count) : count,
                status == "Success" ? 0 : status == "Partial" ? count - rnd.Next(1, count) : count,
                rnd.Next(100, 30000),
                status == "Failed" ? "连接超时" : null,
                rnd.Next(2) == 0 ? "System" : "admin"
            ));
        }

        return ApiResponse<List<SyncRecordDto>>.Ok(result.OrderByDescending(r => r.SyncTime).ToList());
    }

    /// <summary>
    /// 获取外部系统事件。
    /// GET /api/settings/external-systems/events?systemId=&status=
    /// </summary>
    [HttpGet("external-systems/events")]
    public async Task<ApiResponse<List<ExternalSystemEventDto>>> GetExternalEvents(
        [FromQuery] string? systemId = null,
        [FromQuery] string? status = null)
    {
        var query = _dbContext.ExtSystemEvents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(systemId))
            query = query.Where(e => e.TargetSystem == systemId || e.SourceSystem == systemId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status == status);

        var events = await query
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        // 获取外部系统名称
        var systemMap = await _dbContext.ExtSystemConfigs
            .ToDictionaryAsync(c => c.SystemId, c => c.SystemName);

        var dtos = events.Select(e => new ExternalSystemEventDto(
            e.EventId,
            e.CreatedAt,
            e.TargetSystem,
            systemMap.GetValueOrDefault(e.TargetSystem, e.TargetSystem),
            e.EventType,
            e.Payload ?? string.Empty,
            e.Status,
            e.Status == "Completed" ? "System" : null,
            e.SentAt,
            e.ErrorMessage
        )).ToList();

        return ApiResponse<List<ExternalSystemEventDto>>.Ok(dtos);
    }

    /// <summary>
    /// 处理外部系统事件。
    /// POST /api/settings/external-systems/events/{eventId}/process
    /// </summary>
    [HttpPost("external-systems/events/{eventId}/process")]
    public async Task<ApiResponse<bool>> ProcessEvent(string eventId, [FromBody] ProcessEventRequest request)
    {
        if (string.IsNullOrEmpty(eventId))
            return ApiResponse<bool>.Fail("事件ID不能为空");

        var evt = await _dbContext.ExtSystemEvents.FindAsync(eventId);
        if (evt == null)
            return ApiResponse<bool>.Fail("事件未找到");

        evt.Status = "Completed";
        evt.SentAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true);
    }

    public record ProcessEventRequest(string ProcessedBy);

    /// <summary>
    /// 触发外部系统同步。
    /// POST /api/settings/external-systems/{systemId}/sync
    /// </summary>
    [HttpPost("external-systems/{systemId}/sync")]
    public async Task<ApiResponse<bool>> TriggerSync(string systemId, [FromBody] TriggerSyncRequest? request = null)
    {
        if (string.IsNullOrEmpty(systemId))
            return ApiResponse<bool>.Fail("系统ID不能为空");

        var config = await _dbContext.ExtSystemConfigs.FindAsync(systemId);
        if (config == null)
            return ApiResponse<bool>.Fail("外部系统配置未找到");

        // 创建一个同步事件作为同步记录
        var syncEvent = new MES.Infrastructure.Persistence.Entities.ExtSystemEvent
        {
            EventId = $"SYNC-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..30],
            EventType = "ManualSync",
            SourceSystem = "MES",
            TargetSystem = systemId,
            Payload = $"{{\"triggeredBy\": \"{request?.TriggeredBy ?? "admin"}\"}}",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ExtSystemEvents.Add(syncEvent);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true);
    }

    public record TriggerSyncRequest(string? TriggeredBy);
}
