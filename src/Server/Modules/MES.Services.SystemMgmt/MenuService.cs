using MES.Contracts.SystemMgmt;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.SystemMgmt;

public class MenuService : IMenuService
{
    private readonly MesDbContext _context;

    public MenuService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuResponse>> GetAllMenusAsync()
    {
        var menus = await _context.SysMenus
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.MenuId)
            .ToListAsync();

        return BuildMenuTree(menus, null);
    }

    public async Task<List<MenuTreeNode>> GetUserMenusAsync(string userId)
    {
        // Get user's role IDs
        var roleIds = await _context.SysUserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        if (roleIds.Count == 0)
            return [];

        // Get menu IDs accessible by user's roles
        var accessibleMenuIds = await _context.SysRoleMenus
            .Where(rm => roleIds.Contains(rm.RoleId))
            .Select(rm => rm.MenuId)
            .Distinct()
            .ToListAsync();

        if (accessibleMenuIds.Count == 0)
            return [];

        // Get menus that are visible and accessible
        var menus = await _context.SysMenus
            .Where(m => m.IsVisible && accessibleMenuIds.Contains(m.MenuId))
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.MenuId)
            .ToListAsync();

        return BuildMenuTreeNodes(menus, null);
    }

    public async Task<MenuResponse> CreateMenuAsync(MenuCreateRequest request)
    {
        var menuId = GenerateMenuId();

        var menu = new SysMenu
        {
            MenuId = menuId,
            MenuName = request.MenuName,
            ParentId = request.ParentId,
            Icon = request.Icon,
            ViewName = request.ViewName,
            ModuleKey = request.ModuleKey,
            PermissionCode = request.PermissionCode,
            SortOrder = request.SortOrder,
            IsVisible = request.IsVisible,
            CreatedAt = DateTime.UtcNow
        };

        await _context.SysMenus.AddAsync(menu);
        await _context.SaveChangesAsync();

        return MapToResponse(menu);
    }

    public async Task<bool> UpdateMenuAsync(string menuId, MenuUpdateRequest request)
    {
        var menu = await _context.SysMenus.FindAsync(menuId);
        if (menu == null) return false;

        menu.MenuName = request.MenuName;
        menu.ParentId = request.ParentId;
        menu.Icon = request.Icon;
        menu.ViewName = request.ViewName;
        menu.ModuleKey = request.ModuleKey;
        menu.PermissionCode = request.PermissionCode;
        menu.SortOrder = request.SortOrder;
        menu.IsVisible = request.IsVisible;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteMenuAsync(string menuId)
    {
        var menu = await _context.SysMenus.FindAsync(menuId);
        if (menu == null) return false;

        // Check if any child menus exist
        var hasChildren = await _context.SysMenus.AnyAsync(m => m.ParentId == menuId);
        if (hasChildren) return false;

        // Remove role-menu mappings
        var roleMenus = await _context.SysRoleMenus
            .Where(rm => rm.MenuId == menuId)
            .ToListAsync();
        if (roleMenus.Count > 0)
        {
            _context.SysRoleMenus.RemoveRange(roleMenus);
        }

        _context.SysMenus.Remove(menu);
        await _context.SaveChangesAsync();
        return true;
    }

    private static List<MenuResponse> BuildMenuTree(List<SysMenu> menus, string? parentId)
    {
        var result = new List<MenuResponse>();

        var children = menus
            .Where(m => m.ParentId == parentId)
            .OrderBy(m => m.SortOrder)
            .ToList();

        foreach (var menu in children)
        {
            var response = MapToResponse(menu);
            response.Children = BuildMenuTree(menus, menu.MenuId);
            result.Add(response);
        }

        return result;
    }

    private static List<MenuTreeNode> BuildMenuTreeNodes(List<SysMenu> menus, string? parentId)
    {
        var result = new List<MenuTreeNode>();

        var children = menus
            .Where(m => m.ParentId == parentId)
            .OrderBy(m => m.SortOrder)
            .ToList();

        foreach (var menu in children)
        {
            var node = MapToTreeNode(menu);
            node.Children = BuildMenuTreeNodes(menus, menu.MenuId);
            result.Add(node);
        }

        return result;
    }

    private static MenuResponse MapToResponse(SysMenu menu) => new()
    {
        MenuId = menu.MenuId,
        MenuName = menu.MenuName,
        ParentId = menu.ParentId,
        Icon = menu.Icon,
        ViewName = menu.ViewName,
        ModuleKey = menu.ModuleKey,
        PermissionCode = menu.PermissionCode,
        SortOrder = menu.SortOrder,
        IsVisible = menu.IsVisible,
        CreatedAt = menu.CreatedAt,
        Children = []
    };

    private static MenuTreeNode MapToTreeNode(SysMenu menu) => new()
    {
        MenuId = menu.MenuId,
        MenuName = menu.MenuName,
        ParentId = menu.ParentId,
        Icon = menu.Icon,
        ViewName = menu.ViewName,
        ModuleKey = menu.ModuleKey,
        PermissionCode = menu.PermissionCode,
        SortOrder = menu.SortOrder,
        Children = []
    };

    private static string GenerateMenuId()
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 999).ToString("D3");
        return $"MENU-{now:yyyyMMdd}-{seq}";
    }
}
