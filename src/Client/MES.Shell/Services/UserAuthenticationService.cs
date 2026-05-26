using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Shell.Helpers;
using MES.Shell.Models;

namespace MES.Shell.Services;

public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly IRepository<SysUser> _userRepo;

    public UserAuthenticationService(IRepository<SysUser> userRepo) => _userRepo = userRepo;

    public async Task<UserInfo?> AuthenticateAsync(string employeeId, string? password)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
            return null;

        var userId = employeeId.Trim();
        var user = await _userRepo.GetByIdAsync(userId);
        if (user is null || !user.IsActive)
            return null;

        // 密码验证：如果有密码输入则验证
        if (!string.IsNullOrEmpty(password))
        {
            if (string.IsNullOrEmpty(user.PasswordHash))
                return null;

            if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
                return null;
        }

        // 认证成功，返回用户信息
        return new UserInfo
        {
            EmployeeId = userId,
            DisplayName = user.UserName,
            DepartmentId = user.DeptId,
            RoleId = user.RoleId,
            IsActive = true,
            LastLoginTime = DateTime.Now
        };
    }
}
