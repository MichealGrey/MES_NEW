namespace MES.Contracts.SystemMgmt;

/// <summary>
/// 用户创建请求
/// </summary>
public class UserCreateRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? RoleId { get; set; }
    public string? DeptId { get; set; }
    public string? Shift { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// 用户更新请求
/// </summary>
public class UserUpdateRequest
{
    public string UserName { get; set; } = string.Empty;
    public string? RoleId { get; set; }
    public string? DeptId { get; set; }
    public string? Shift { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// 用户响应
/// </summary>
public class UserResponse
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? DeptId { get; set; }
    public string? DeptName { get; set; }
    public string? Shift { get; set; }
    public bool IsActive { get; set; }
    public List<string> RoleIds { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 密码重置请求
/// </summary>
public class ResetPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}
