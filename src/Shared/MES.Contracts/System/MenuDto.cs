namespace MES.Contracts.System;

public class MenuDto
{
    public string MenuId { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? Icon { get; set; }
    public string? ViewName { get; set; }
    public string? ModuleKey { get; set; }
    public string? PermissionCode { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; }
    public List<MenuDto> Children { get; set; } = [];
}

public class UserRoleDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
}

public class RoleMenuDto
{
    public long Id { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public string MenuId { get; set; } = string.Empty;
}

public class RolePermissionDto
{
    public long Id { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
}
