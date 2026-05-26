using System.Threading.Tasks;

namespace MES.Shared.Services;

/// <summary>
/// 用户会话服务接口 — 存放于 Shared 层以便各模块引用。
/// 由 MES.Shell.SessionService 实现。
/// </summary>
public interface ISessionService
{
    string EmployeeId { get; }
    string DisplayName { get; }
    string RoleId { get; }
    string RoleName { get; }
    string DepartmentId { get; }
    string DepartmentName { get; }
    bool IsLoggedIn { get; }

    Task StartSession(string employeeId, string displayName, string roleId, string roleName, string departmentId, string departmentName);
    void EndSession();
    bool HasPermission(string moduleKey, string? viewName = null);
    bool HasModuleAccess(string moduleKey);
}