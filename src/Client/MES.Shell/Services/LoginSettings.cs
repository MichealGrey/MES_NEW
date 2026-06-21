using System.IO;
using System.Text.Json;

namespace MES.Shell.Services;

/// <summary>
/// 登录设置持久化 — 本地文件存储工号和是否勾选"记住密码"
/// 注意：出于安全考虑，仅保存工号，不保存密码明文
/// </summary>
public static class LoginSettings
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SEMI-MES", "login_settings.json");

    public static void Save(string employeeId)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            var data = new { EmployeeId = employeeId };
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(data));
        }
        catch { /* 忽略写入失败 */ }
    }

    public static string? LoadEmployeeId()
    {
        try
        {
            if (!File.Exists(SettingsPath)) return null;
            var json = File.ReadAllText(SettingsPath);
            var data = JsonSerializer.Deserialize<LoginData>(json);
            return data?.EmployeeId;
        }
        catch { return null; }
    }

    public static void Clear()
    {
        try { if (File.Exists(SettingsPath)) File.Delete(SettingsPath); }
        catch { /* 忽略 */ }
    }

    private class LoginData { public string? EmployeeId { get; set; } }
}
