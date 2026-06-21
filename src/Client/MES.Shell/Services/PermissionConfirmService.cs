using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Shared.Services;
using MES.Shell.Helpers;

namespace MES.Shell.Services;

public class PermissionConfirmService : IPermissionConfirmService
{
    private readonly IUserAuthenticationService _authService;
    private readonly ISessionService _session;
    private readonly IRepository<SysRole> _roleRepo;
    private readonly IPermissionConfirmRepository _confirmRepo;

    /// <summary>
    /// 操作类型到所需权限级别的映射。
    /// L1=操作员, L2=QA, L3=工程, L4=主管
    /// </summary>
    private static readonly Dictionary<string, string> OperationLevelMap = new(
        StringComparer.OrdinalIgnoreCase)
    {
        { "TrackIn",        "L1" },
        { "TrackOut",       "L1" },
        { "Hold",           "L2" },
        { "Release",        "L2" },
        { "Scrap",          "L4" },
        { "RouteModify",    "L3" },
        { "RecipeModify",   "L3" },
        { "Split",          "L4" },
        { "Merge",          "L4" },
        { "Rework",         "L3" },
        { "GradeSort",      "L2" },
    };

    public PermissionConfirmService(
        IUserAuthenticationService authService,
        ISessionService session,
        IRepository<SysRole> roleRepo,
        IPermissionConfirmRepository confirmRepo)
    {
        _authService = authService;
        _session = session;
        _roleRepo = roleRepo;
        _confirmRepo = confirmRepo;
    }

    public string GetRequiredLevel(string operationType)
    {
        if (string.IsNullOrWhiteSpace(operationType))
            return "L1";

        return OperationLevelMap.TryGetValue(operationType, out var level)
            ? level
            : "L1";
    }

    public async Task<bool> ConfirmPermissionAsync(
        string operationType,
        string employeeId,
        string password,
        string requiredLevel)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
            return false;

        // 如果未指定所需级别，尝试根据操作类型获取
        var level = string.IsNullOrWhiteSpace(requiredLevel)
            ? GetRequiredLevel(operationType)
            : requiredLevel;

        // 1. 验证用户密码
        var user = await _authService.AuthenticateAsync(employeeId, password);
        if (user == null)
        {
            await RecordConfirmAsync(operationType, employeeId, level, false);
            return false;
        }

        // 2. 获取用户角色级别
        var userSignatureLevel = await GetUserSignatureLevelAsync(employeeId);
        if (string.IsNullOrEmpty(userSignatureLevel))
        {
            await RecordConfirmAsync(operationType, employeeId, level, false);
            return false;
        }

        // 3. 比较级别
        var isSufficient = SignatureLevelHelper.IsLevelSufficient(userSignatureLevel, level);

        await RecordConfirmAsync(operationType, employeeId, level, isSufficient);
        return isSufficient;
    }

    public async Task RecordConfirmAsync(
        string operationType,
        string employeeId,
        string requiredLevel,
        bool success)
    {
        var record = new SysPermissionConfirm
        {
            OperationType = operationType ?? string.Empty,
            EmployeeId = employeeId.Trim(),
            EmployeeName = _session.IsLoggedIn && _session.EmployeeId == employeeId.Trim()
                ? _session.DisplayName
                : null,
            RequiredLevel = requiredLevel ?? string.Empty,
            Success = success,
            ConfirmAt = DateTime.UtcNow,
        };

        await _confirmRepo.AddAsync(record);
    }

    /// <summary>
    /// 获取员工对应的签名级别（L1~L4）。
    /// 通过数据库中的 SysRole.Level 并结合角色 ID 进行映射。
    /// </summary>
    private async Task<string> GetUserSignatureLevelAsync(string employeeId)
    {
        // 优先使用当前会话中的角色
        var roleId = _session.IsLoggedIn && _session.EmployeeId == employeeId.Trim()
            ? _session.RoleId
            : null;

        // 如果会话中没有，尝试通过认证服务获取用户信息
        if (string.IsNullOrEmpty(roleId))
        {
            var user = await _authService.AuthenticateAsync(employeeId, null);
            roleId = user?.RoleId;
        }

        if (string.IsNullOrEmpty(roleId))
            return "L1";

        var role = await _roleRepo.GetByIdAsync(roleId);
        if (role == null)
            return "L1";

        return MapRoleToSignatureLevel(roleId, role.Level);
    }

    /// <summary>
    /// 将 SysRole 映射到签名级别。
    /// SysRole.Level: 1=最高(管理员), 4=最低(操作员)
    /// Signature: L1=操作员, L4=主管
    /// 两者方向相反，需结合角色 ID 综合判断。
    /// </summary>
    private static string MapRoleToSignatureLevel(string roleId, int roleLevel)
    {
        var rid = roleId.ToUpperInvariant();

        // 角色 ID 精确映射（最可靠）
        if (rid.Contains("OPERATOR"))
            return "L1";
        if (rid.Contains("QA") || rid.Contains("QUALITY"))
            return "L2";
        if (rid.Contains("ENGINEER") || rid.Contains("ENG"))
            return "L3";
        if (rid.Contains("SHIFT") || rid.Contains("LEAD") || rid.Contains("LEADER"))
            return "L2"; // 领班/组长可以执行 Hold/Release (L2)
        if (rid.Contains("MGR") || rid.Contains("MANAGER") || rid.Contains("ADMIN"))
            return "L4";

        // 回退：使用 SysRole.Level 数值反推
        // Level 4 = 操作员 = L1
        // Level 3 = 普通角色 = L2
        // Level 2 = 主管 = L4
        // Level 1 = 管理员 = L4
        return roleLevel switch
        {
            4 => "L1",
            3 => "L2",
            2 => "L4",
            1 => "L4",
            _ => "L1",
        };
    }
}
