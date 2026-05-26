using System.Security.Cryptography;
using System.Text;

namespace MES.Shell.Helpers;

/// <summary>
/// 密码加盐哈希工具（SHA256）
/// </summary>
public static class PasswordHelper
{
    private const int SaltLength = 16;

    /// <summary>生成随机盐值 (Base64)</summary>
    public static string GenerateSalt()
    {
        var salt = RandomNumberGenerator.GetBytes(SaltLength);
        return Convert.ToBase64String(salt);
    }

    /// <summary>计算密码哈希: SHA256(password)</summary>
    public static string HashPassword(string password)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }

    /// <summary>验证密码是否匹配</summary>
    public static bool VerifyPassword(string inputPassword, string storedHash)
    {
        var inputHash = HashPassword(inputPassword);
        return string.Equals(inputHash, storedHash, StringComparison.Ordinal);
    }
}