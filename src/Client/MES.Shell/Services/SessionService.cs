using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Shared.Services;

namespace MES.Shell.Services;

public class SessionService : ISessionService
{
    private readonly IRepository<SysUserPermission> _permRepo;
    private HashSet<string> _permissions = [];

    public string EmployeeId { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string RoleId { get; private set; } = string.Empty;
    public string RoleName { get; private set; } = string.Empty;
    public string DepartmentId { get; private set; } = string.Empty;
    public string DepartmentName { get; private set; } = string.Empty;
    public bool IsLoggedIn { get; private set; }

    public SessionService(IRepository<SysUserPermission> permRepo) => _permRepo = permRepo;

    public async Task StartSession(string employeeId, string displayName, string roleId, string roleName, string departmentId, string departmentName)
    {
        EmployeeId = employeeId;
        DisplayName = displayName;
        RoleId = roleId;
        RoleName = roleName;
        DepartmentId = departmentId;
        DepartmentName = departmentName;
        IsLoggedIn = true;

        // 从数据库加载该用户的权限列表
        var perms = await _permRepo.GetWhereAsync(p => p.UserId == employeeId);
        _permissions = perms.Select(p => p.PermissionCode).ToHashSet();
    }

    public void EndSession()
    {
        EmployeeId = string.Empty;
        DisplayName = string.Empty;
        RoleId = string.Empty;
        RoleName = string.Empty;
        DepartmentId = string.Empty;
        DepartmentName = string.Empty;
        IsLoggedIn = false;
        _permissions = [];
    }

    /// <summary>
    /// 模块 Key 到数据库权限前缀的映射
    /// </summary>
    private static readonly Dictionary<string, string> ModulePermissionPrefix = new()
    {
        { "Production",  "prod" },
        { "Equipment",   "equip" },
        { "Recipe",      "eng" },
        { "Quality",     "qa" },
        { "Warehouse",   "wh" },
        { "Schedule",    "prod" },
        { "Alarm",       "equip" },
        { "Trace",       "prod" },
        { "Yield",       "prod" },
        { "EHS",         "ehs" },
        { "admin",       "admin" },
    };

    public bool HasPermission(string moduleKey, string? viewName = null)
    {
        if (!IsLoggedIn) return false;
        if (_permissions.Contains("*:*")) return true;
        if (_permissions.Contains("admin.all")) return true;

        // 尝试 PascalCase:ViewName 格式
        var target = string.IsNullOrEmpty(viewName)
            ? moduleKey
            : $"{moduleKey}:{viewName}";

        if (_permissions.Contains(target)) return true;
        if (!string.IsNullOrEmpty(viewName) && _permissions.Contains($"{moduleKey}:*"))
            return true;

        // 尝试小写.格式（数据库实际格式）
        if (ModulePermissionPrefix.TryGetValue(moduleKey, out var prefix))
        {
            if (string.IsNullOrEmpty(viewName))
            {
                // 检查是否有任何该前缀的权限
                if (_permissions.Any(p => p.StartsWith(prefix + ".")))
                    return true;
            }
            else
            {
                // 将 viewName 转为小写并匹配，如 TrackInView -> trackin
                var viewLower = viewName.Replace("View", "").ToLowerInvariant();
                var permCode = $"{prefix}.{viewLower}";
                if (_permissions.Contains(permCode))
                    return true;
                // 通配符
                if (_permissions.Contains($"{prefix}.*"))
                    return true;
            }
        }

        return false;
    }

    public bool HasModuleAccess(string moduleKey)
    {
        if (!IsLoggedIn) return false;
        if (_permissions.Contains("*:*")) return true;
        if (_permissions.Contains("admin.all")) return true;
        if (_permissions.Contains($"{moduleKey}:*")) return true;

        // PascalCase 格式匹配
        if (_permissions.Any(p =>
        {
            var parts = p.Split(':');
            return parts.Length == 2 && parts[1] == "*" && parts[0] == moduleKey;
        }))
            return true;

        if (_permissions.Any(p =>
        {
            var parts = p.Split(':');
            return parts.Length == 2 && parts[0] == moduleKey;
        }))
            return true;

        // 小写.格式匹配（数据库实际格式）
        if (ModulePermissionPrefix.TryGetValue(moduleKey, out var prefix))
        {
            if (_permissions.Any(p => p.StartsWith(prefix + ".")))
                return true;
        }

        return false;
    }
}
