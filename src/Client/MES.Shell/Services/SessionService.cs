using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Shared.Services;

namespace MES.Shell.Services;

public class SessionService : ISessionService
{
    private readonly IRepository<SysUserPermission> _permRepo;
    private readonly IRepository<SysRole> _roleRepo;
    private readonly IRepository<SysDepartment> _deptRepo;
    private HashSet<string> _permissions = [];
    private HashSet<string> _frequentUsers = [];
    private bool _isLocked;

    public string EmployeeId { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string RoleId { get; private set; } = string.Empty;
    public string RoleName { get; private set; } = string.Empty;
    public string DepartmentId { get; private set; } = string.Empty;
    public string DepartmentName { get; private set; } = string.Empty;
    public bool IsLoggedIn { get; private set; }
    public bool IsLocked => _isLocked;

    public SessionService(IRepository<SysUserPermission> permRepo, IRepository<SysRole> roleRepo, IRepository<SysDepartment> deptRepo)
    {
        _permRepo = permRepo;
        _roleRepo = roleRepo;
        _deptRepo = deptRepo;
    }

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
        _isLocked = false;
        // Note: _frequentUsers is preserved across session switches
    }

    /// <summary>
    /// 模块 Key 到数据库权限前缀的映射
    /// </summary>
    private static readonly Dictionary<string, string> ModulePermissionPrefix = new()
    {
        { "Production",  "prod" },
        { "Order",       "order" },
        { "Execute",     "prod" },
        { "Lot",         "prod" },
        { "MasterData",  "prod" },
        { "Equipment",   "equip" },
        { "Recipe",      "eng" },
        { "Quality",     "qa" },
        { "Warehouse",   "wh" },
        { "Schedule",    "prod" },
        { "Alarm",       "equip" },
        { "Trace",       "prod" },
        { "Yield",       "prod" },
        { "EHS",         "ehs" },
        { "CustomerComplaint", "qa" },
        { "EngineeringChange", "eng" },
        { "ReportCenter", "prod" },
        { "SystemSettings", "admin" },
        { "admin",       "admin" },
    };

    /// <summary>
    /// 模块别名映射：某些模块的权限实际存储在其他模块名下
    /// </summary>
    private static readonly Dictionary<string, string> ModulePermissionAliases = new()
    {
        { "Execute", "Production" },       // 生产执行模块的权限存储在 Production 模块下
        { "Lot", "Production" },            // 批次管理模块的权限存储在 Production 模块下
        { "MasterData", "Production" },     // 主数据管理模块的权限存储在 Production 模块下
        { "CustomerComplaint", "Quality" }, // 客诉8D模块的权限存储在 Quality 模块下
        { "EngineeringChange", "Quality" }, // 工程变更模块的权限存储在 Quality 模块下
        { "ReportCenter", "Production" },   // 报表中心模块的权限存储在 Production 模块下
        { "SystemSettings", "admin" },      // 系统设置模块的权限使用 admin 前缀
    };

    public bool HasPermission(string moduleKey, string? viewName = null)
    {
        if (!IsLoggedIn) return false;
        if (_permissions.Contains("*:*")) return true;
        if (_permissions.Contains("admin.all")) return true;

        // 尝试当前模块的 PascalCase:ViewName 格式
        if (TryMatchPermission(moduleKey, viewName)) return true;

        // 尝试模块别名（如 Execute → Production）
        if (ModulePermissionAliases.TryGetValue(moduleKey, out var aliasModule))
        {
            if (TryMatchPermission(aliasModule, viewName)) return true;
        }

        return false;
    }

    private bool TryMatchPermission(string moduleKey, string? viewName)
    {
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

        // 尝试模块别名（如 MasterData → Production）
        if (ModulePermissionAliases.TryGetValue(moduleKey, out var aliasModule))
        {
            if (_permissions.Any(p => p.StartsWith($"{aliasModule}:")))
                return true;
            if (ModulePermissionPrefix.TryGetValue(aliasModule, out var aliasPrefix))
            {
                if (_permissions.Any(p => p.StartsWith(aliasPrefix + ".")))
                    return true;
            }
        }

        return false;
    }

    public void LockScreen()
    {
        _isLocked = true;
    }

    public bool UnlockScreen(string password, IUserAuthenticationService authService)
    {
        if (string.IsNullOrEmpty(EmployeeId))
            return false;

        var result = authService.AuthenticateAsync(EmployeeId, password).Result;
        if (result != null)
        {
            _isLocked = false;
            return true;
        }

        return false;
    }

    public async Task<bool> SwitchUserAsync(string employeeId, string password, IUserAuthenticationService authService)
    {
        var userInfo = await authService.AuthenticateAsync(employeeId, password);
        if (userInfo == null)
            return false;

        AddToFrequentUsers(employeeId);

        // Resolve role name and department name
        var role = await _roleRepo.GetByIdAsync(userInfo.RoleId);
        var roleName = role?.RoleName ?? "未知角色";
        var dept = await _deptRepo.GetByIdAsync(userInfo.DepartmentId);
        var deptName = dept?.DeptName ?? "未知部门";

        // End current session
        EndSession();

        // Start new session
        await StartSession(
            userInfo.EmployeeId,
            userInfo.DisplayName,
            userInfo.RoleId,
            roleName,
            userInfo.DepartmentId,
            deptName);

        return true;
    }

    public List<string> GetFrequentUsers()
    {
        return _frequentUsers.ToList();
    }

    public void AddToFrequentUsers(string employeeId)
    {
        if (string.IsNullOrEmpty(employeeId))
            return;

        _frequentUsers.Add(employeeId);
    }
}
