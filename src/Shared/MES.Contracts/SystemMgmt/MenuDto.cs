namespace MES.Contracts.SystemMgmt;

/// <summary>
/// 菜单创建请求
/// </summary>
public class MenuCreateRequest
{
    public string MenuName { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? Icon { get; set; }
    public string? ViewName { get; set; }
    public string? ModuleKey { get; set; }
    public string? PermissionCode { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}

/// <summary>
/// 菜单更新请求
/// </summary>
public class MenuUpdateRequest
{
    public string MenuName { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? Icon { get; set; }
    public string? ViewName { get; set; }
    public string? ModuleKey { get; set; }
    public string? PermissionCode { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}

/// <summary>
/// 菜单响应
/// </summary>
public class MenuResponse
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
    public DateTime CreatedAt { get; set; }
    public List<MenuResponse> Children { get; set; } = [];
}

/// <summary>
/// 菜单树节点
/// </summary>
public class MenuTreeNode
{
    public string MenuId { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? Icon { get; set; }
    public string? ViewName { get; set; }
    public string? ModuleKey { get; set; }
    public string? PermissionCode { get; set; }
    public int SortOrder { get; set; }
    public List<MenuTreeNode> Children { get; set; } = [];
}
