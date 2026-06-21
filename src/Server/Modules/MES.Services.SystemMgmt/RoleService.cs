using MES.Contracts.SystemMgmt;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.SystemMgmt;

public class RoleService : IRoleService
{
    private readonly MesDbContext _context;

    public RoleService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleResponse>> GetRolesAsync()
    {
        var roles = await _context.SysRoles
            .OrderBy(r => r.Level)
            .ThenBy(r => r.RoleName)
            .ToListAsync();

        var results = new List<RoleResponse>();
        foreach (var role in roles)
        {
            var response = MapToResponse(role);

            var menuIds = await _context.SysRoleMenus
                .Where(rm => rm.RoleId == role.RoleId)
                .Select(rm => rm.MenuId)
                .ToListAsync();
            response.MenuIds = menuIds;

            var permCodes = await _context.SysRolePermissions
                .Where(rp => rp.RoleId == role.RoleId)
                .Select(rp => rp.PermissionCode)
                .ToListAsync();
            response.PermissionCodes = permCodes;

            results.Add(response);
        }

        return results;
    }

    public async Task<RoleResponse?> GetRoleAsync(string roleId)
    {
        var role = await _context.SysRoles.FindAsync(roleId);
        if (role == null) return null;

        var response = MapToResponse(role);

        var menuIds = await _context.SysRoleMenus
            .Where(rm => rm.RoleId == roleId)
            .Select(rm => rm.MenuId)
            .ToListAsync();
        response.MenuIds = menuIds;

        var permCodes = await _context.SysRolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionCode)
            .ToListAsync();
        response.PermissionCodes = permCodes;

        return response;
    }

    public async Task<RoleResponse> CreateRoleAsync(RoleCreateRequest request)
    {
        var roleId = GenerateRoleId();

        var role = new SysRole
        {
            RoleId = roleId,
            RoleName = request.RoleName,
            Description = request.Description,
            Level = request.Level,
            CreatedAt = DateTime.UtcNow
        };

        await _context.SysRoles.AddAsync(role);
        await _context.SaveChangesAsync();

        return MapToResponse(role);
    }

    public async Task<bool> UpdateRoleAsync(string roleId, RoleUpdateRequest request)
    {
        var role = await _context.SysRoles.FindAsync(roleId);
        if (role == null) return false;

        role.RoleName = request.RoleName;
        role.Description = request.Description;
        role.Level = request.Level;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRoleAsync(string roleId)
    {
        var role = await _context.SysRoles.FindAsync(roleId);
        if (role == null) return false;

        // Check if any users are assigned to this role
        var hasUsers = await _context.SysUserRoles.AnyAsync(ur => ur.RoleId == roleId);
        if (hasUsers) return false;

        // Remove role-menu mappings
        var roleMenus = await _context.SysRoleMenus
            .Where(rm => rm.RoleId == roleId)
            .ToListAsync();
        if (roleMenus.Count > 0)
        {
            _context.SysRoleMenus.RemoveRange(roleMenus);
        }

        // Remove role-permission mappings
        var rolePerms = await _context.SysRolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();
        if (rolePerms.Count > 0)
        {
            _context.SysRolePermissions.RemoveRange(rolePerms);
        }

        _context.SysRoles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignMenusAsync(string roleId, List<string> menuIds)
    {
        var role = await _context.SysRoles.FindAsync(roleId);
        if (role == null) return false;

        // Remove existing mappings
        var existingMenus = await _context.SysRoleMenus
            .Where(rm => rm.RoleId == roleId)
            .ToListAsync();
        if (existingMenus.Count > 0)
        {
            _context.SysRoleMenus.RemoveRange(existingMenus);
        }

        // Add new mappings
        if (menuIds.Count > 0)
        {
            var newMappings = menuIds.Select(menuId => new SysRoleMenu
            {
                RoleId = roleId,
                MenuId = menuId
            }).ToList();
            await _context.SysRoleMenus.AddRangeAsync(newMappings);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignPermissionsAsync(string roleId, List<string> permissionCodes)
    {
        var role = await _context.SysRoles.FindAsync(roleId);
        if (role == null) return false;

        // Remove existing mappings
        var existingPerms = await _context.SysRolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();
        if (existingPerms.Count > 0)
        {
            _context.SysRolePermissions.RemoveRange(existingPerms);
        }

        // Add new mappings
        if (permissionCodes.Count > 0)
        {
            var newMappings = permissionCodes.Select(code => new SysRolePermission
            {
                RoleId = roleId,
                PermissionCode = code
            }).ToList();
            await _context.SysRolePermissions.AddRangeAsync(newMappings);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private static RoleResponse MapToResponse(SysRole role) => new()
    {
        RoleId = role.RoleId,
        RoleName = role.RoleName,
        Description = role.Description,
        Level = role.Level,
        CreatedAt = role.CreatedAt,
        MenuIds = [],
        PermissionCodes = []
    };

    private static string GenerateRoleId()
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 999).ToString("D3");
        return $"ROLE-{now:yyyyMMdd}-{seq}";
    }
}
