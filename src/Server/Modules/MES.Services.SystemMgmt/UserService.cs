using BCrypt.Net;
using MES.Contracts.Common;
using MES.Contracts.SystemMgmt;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.SystemMgmt;

public class UserService : IUserService
{
    private readonly MesDbContext _context;

    public UserService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<UserResponse>> GetUsersAsync(int pageIndex, int pageSize, string? search = null)
    {
        var query = _context.SysUsers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => u.UserName.Contains(search) || u.UserId.Contains(search));
        }

        query = query.OrderByDescending(u => u.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var results = new List<UserResponse>();
        foreach (var user in items)
        {
            var response = MapToResponse(user);

            var roles = await _context.SysUserRoles
                .Where(ur => ur.UserId == user.UserId)
                .Select(ur => ur.RoleId)
                .ToListAsync();
            response.RoleIds = roles;

            results.Add(response);
        }

        return new PagedResult<UserResponse>
        {
            Items = results,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<UserResponse?> GetUserAsync(string userId)
    {
        var user = await _context.SysUsers.FindAsync(userId);
        if (user == null) return null;

        var response = MapToResponse(user);

        var roleIds = await _context.SysUserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();
        response.RoleIds = roleIds;

        return response;
    }

    public async Task<UserResponse> CreateUserAsync(UserCreateRequest request)
    {
        var userId = GenerateUserId();

        var user = new SysUser
        {
            UserId = userId,
            UserName = request.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            DeptId = request.DeptId ?? "",
            Shift = request.Shift,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.SysUsers.AddAsync(user);

        if (!string.IsNullOrEmpty(request.RoleId))
        {
            var userRole = new SysUserRole
            {
                UserId = userId,
                RoleId = request.RoleId,
                CreatedAt = DateTime.UtcNow
            };
            await _context.SysUserRoles.AddAsync(userRole);
        }

        await _context.SaveChangesAsync();

        var response = MapToResponse(user);
        if (!string.IsNullOrEmpty(request.RoleId))
        {
            response.RoleIds = [request.RoleId];
        }

        return response;
    }

    public async Task<bool> UpdateUserAsync(string userId, UserUpdateRequest request)
    {
        var user = await _context.SysUsers.FindAsync(userId);
        if (user == null) return false;

        user.UserName = request.UserName;
        user.DeptId = request.DeptId ?? "";
        user.Shift = request.Shift;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(request.RoleId))
        {
            var existingRole = await _context.SysUserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId);

            if (existingRole != null)
            {
                existingRole.RoleId = request.RoleId;
            }
            else
            {
                await _context.SysUserRoles.AddAsync(new SysUserRole
                {
                    UserId = userId,
                    RoleId = request.RoleId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _context.SysUsers.FindAsync(userId);
        if (user == null) return false;

        var userRoles = await _context.SysUserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync();
        _context.SysUserRoles.RemoveRange(userRoles);

        var userPermissions = await _context.SysUserPermissions
            .Where(up => up.UserId == userId)
            .ToListAsync();
        _context.SysUserPermissions.RemoveRange(userPermissions);

        _context.SysUsers.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
    {
        var user = await _context.SysUsers.FindAsync(userId);
        if (user == null) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleUserStatusAsync(string userId)
    {
        var user = await _context.SysUsers.FindAsync(userId);
        if (user == null) return false;

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static UserResponse MapToResponse(SysUser user)
    {
        var response = new UserResponse
        {
            UserId = user.UserId,
            UserName = user.UserName,
            RoleId = user.RoleId,
            DeptId = user.DeptId,
            Shift = user.Shift,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return response;
    }

    private static string GenerateUserId()
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 999).ToString("D3");
        return $"USR-{now:yyyyMMdd}-{seq}";
    }
}
