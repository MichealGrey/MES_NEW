using MES.Shell.Models;

namespace MES.Shell.Services;

public interface IUserAuthenticationService
{
    /// <summary>
    /// 验证用户凭证。
    /// 如果 password 为 null 或空字符串，则仅按工号查找（厂牌扫描模式）。
    /// 返回 null 表示认证失败。
    /// </summary>
    Task<UserInfo?> AuthenticateAsync(string employeeId, string? password);
}