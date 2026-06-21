namespace MES.Contracts.SystemMgmt;

/// <summary>
/// 角色创建请求
/// </summary>
public class RoleCreateRequest
{
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Level { get; set; }
}

/// <summary>
/// 角色更新请求
/// </summary>
public class RoleUpdateRequest
{
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Level { get; set; }
}

/// <summary>
/// 角色响应
/// </summary>
public class RoleResponse
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Level { get; set; }
    public List<string> MenuIds { get; set; } = [];
    public List<string> PermissionCodes { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
