namespace MES.Shell.Models;

public class UserInfo
{
    public string EmployeeId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DepartmentId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginTime { get; set; }

    /// <summary>是否首次登录（需要修改默认密码）</summary>
    public bool IsFirstLogin { get; set; }
}