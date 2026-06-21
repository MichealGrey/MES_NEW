using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MES.Api.Configuration;
using MES.Contracts.Auth;
using MES.Contracts.Common;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly MesDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        MesDbContext context,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthController> logger)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    [HttpPost("login")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<ApiResponse<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        try
        {
            // Find user
            var user = await _context.SysUsers
                .FirstOrDefaultAsync(u => u.UserId == request.UserId && u.IsActive);

            if (user == null)
            {
                await LogLoginAsync(request.UserId, ipAddress, "Failed", "User not found or inactive");
                return ApiResponse<LoginResponse>.Fail("Invalid user ID or password");
            }

            // Validate password
            bool isPasswordValid = ValidatePassword(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                await LogLoginAsync(request.UserId, ipAddress, "Failed", "Invalid password");
                return ApiResponse<LoginResponse>.Fail("Invalid user ID or password");
            }

            // Get role information
            var role = await _context.SysRoles
                .FirstOrDefaultAsync(r => r.RoleId == user.RoleId);

            // Generate JWT token
            var token = GenerateJwtToken(user, role);

            // Update user's last activity
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Log successful login
            await LogLoginAsync(request.UserId, ipAddress, "Success", null);

            _logger.LogInformation("User {UserId} ({UserName}) logged in successfully from {IpAddress}",
                user.UserId, user.UserName, ipAddress);

            return ApiResponse<LoginResponse>.Ok(new LoginResponse
            {
                Token = token,
                UserId = user.UserId,
                UserName = user.UserName,
                RoleId = user.RoleId,
                RoleName = role?.RoleName ?? string.Empty,
                DeptId = user.DeptId,
                Permissions = await GetUserPermissionsAsync(user.UserId),
                Menus = await GetUserMenusAsync(user.RoleId)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user {UserId} from {IpAddress}",
                request.UserId, ipAddress);

            await LogLoginAsync(request.UserId, ipAddress, "Failed", ex.Message);
            return ApiResponse<LoginResponse>.Fail("Login failed. Please try again later.");
        }
    }

    private bool ValidatePassword(string password, string? passwordHash)
    {
        if (string.IsNullOrEmpty(passwordHash))
        {
            _logger.LogWarning("[ValidatePassword] Password hash is null or empty");
            return false;
        }

        // If it's a placeholder hash, accept "123456" as default password
        if (passwordHash.Contains("placeholder", StringComparison.OrdinalIgnoreCase))
            return password == "123456";

        // BCrypt hashes start with $2a$, $2b$, or $2y$
        bool isBcryptFormat = passwordHash.StartsWith("$2");

        if (isBcryptFormat)
        {
            // Validate using BCrypt only
            try
            {
                if (BCrypt.Net.BCrypt.Verify(password, passwordHash))
                    return true;

                _logger.LogWarning("[ValidatePassword] BCrypt verification failed for hash starting with: {Prefix}", passwordHash.Substring(0, Math.Min(7, passwordHash.Length)));
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ValidatePassword] BCrypt verification threw exception");
                return false;
            }
        }

        // Legacy SHA256 fallback (for old seed data)
        try
        {
            var sha256Hash = ComputeSha256Hash(password);
            if (string.Equals(sha256Hash, passwordHash, StringComparison.Ordinal))
                return true;

            _logger.LogWarning("[ValidatePassword] SHA256 verification failed for non-BCrypt hash");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ValidatePassword] SHA256 verification threw exception");
            return false;
        }
    }

    private static string ComputeSha256Hash(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }

    private string GenerateJwtToken(SysUser user, SysRole? role)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim("userId", user.UserId),
            new Claim("userName", user.UserName),
            new Claim("roleId", user.RoleId),
            new Claim("roleName", role?.RoleName ?? string.Empty),
            new Claim("deptId", user.DeptId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task LogLoginAsync(string userId, string ipAddress, string result, string? errorMessage)
    {
        var loginLog = new SysLoginLog
        {
            UserId = userId,
            EmployeeId = userId,
            LoginTime = DateTime.UtcNow,
            IpAddress = ipAddress,
            Result = result,
            ErrorMessage = errorMessage
        };

        await _context.SysLoginLogs.AddAsync(loginLog);
        await _context.SaveChangesAsync();
    }

    private async Task<List<string>> GetUserPermissionsAsync(string userId)
    {
        var permissions = await _context.SysUserPermissions
            .Where(p => p.UserId == userId)
            .Select(p => p.PermissionCode)
            .ToListAsync();

        return permissions;
    }

    private async Task<List<MES.Contracts.System.MenuDto>> GetUserMenusAsync(string roleId)
    {
        var menuIds = await _context.SysRoleMenus
            .Where(rm => rm.RoleId == roleId)
            .Select(rm => rm.MenuId)
            .ToListAsync();

        var menus = await _context.SysMenus
            .Where(m => menuIds.Contains(m.MenuId) && m.IsVisible)
            .OrderBy(m => m.SortOrder)
            .Select(m => new MES.Contracts.System.MenuDto
            {
                MenuId = m.MenuId,
                MenuName = m.MenuName,
                ParentId = m.ParentId,
                Icon = m.Icon,
                ViewName = m.ViewName,
                ModuleKey = m.ModuleKey,
                PermissionCode = m.PermissionCode,
                SortOrder = m.SortOrder,
                IsVisible = m.IsVisible
            })
            .ToListAsync();

        return menus;
    }

    /// <summary>
    /// 初始化管理员账号（仅开发环境使用）
    /// </summary>
    [AllowAnonymous]
    [HttpPost("seed-admin")]
    public async Task<IActionResult> SeedAdmin()
    {
        // 确保 USR-ADMIN 存在
        var usrAdmin = await _context.SysUsers
            .FirstOrDefaultAsync(u => u.UserId == "USR-ADMIN");

        if (usrAdmin == null)
        {
            var adminHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@2026");
            usrAdmin = new SysUser
            {
                UserId = "USR-ADMIN",
                UserName = "系统管理员",
                PasswordHash = adminHash,
                RoleId = "ROLE-ADMIN",
                DeptId = "DEPT-IT",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.SysUsers.Add(usrAdmin);
            await _context.SaveChangesAsync();
        }
        else
        {
            // 确保密码和角色正确
            usrAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@2026");
            usrAdmin.IsActive = true;
            usrAdmin.RoleId = "ROLE-ADMIN";
            usrAdmin.DeptId = "DEPT-IT";
            await _context.SaveChangesAsync();
        }

        // 兼容旧的 admin 账号
        var existing = await _context.SysUsers
            .FirstOrDefaultAsync(u => u.UserId == "10001");

        if (existing != null)
        {
            existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
            existing.IsActive = true;
            existing.RoleId = "ROLE-ADMIN";
            existing.DeptId = "DEPT-IT";
            await _context.SaveChangesAsync();
            return Ok(new { message = "Users seeded successfully", users = new[] {
                new { userId = "USR-ADMIN", userName = "系统管理员", password = "SuperAdmin@2026" },
                new { userId = "10001", userName = existing.UserName, password = "admin123" }
            }});
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        var admin = new SysUser
        {
            UserId = "10001",
            UserName = "admin",
            PasswordHash = passwordHash,
            RoleId = "ROLE-ADMIN",
            DeptId = "DEPT-IT",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.SysUsers.Add(admin);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Users seeded successfully", users = new[] {
            new { userId = "USR-ADMIN", userName = "系统管理员", password = "SuperAdmin@2026" },
            new { userId = "10001", userName = "admin", password = "admin123" }
        }});
    }
}
