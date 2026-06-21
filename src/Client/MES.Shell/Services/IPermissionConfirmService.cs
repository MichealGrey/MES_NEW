namespace MES.Shell.Services;

public interface IPermissionConfirmService
{
    /// <summary>
    /// 确认权限：验证用户密码并检查用户角色级别是否满足操作所需级别。
    /// </summary>
    /// <param name="operationType">操作类型，如 TrackIn, Scrap 等</param>
    /// <param name="employeeId">员工工号</param>
    /// <param name="password">密码（用于二次验证）</param>
    /// <param name="requiredLevel">所需权限级别（L1~L4）</param>
    /// <returns>true 表示认证成功且级别足够</returns>
    Task<bool> ConfirmPermissionAsync(string operationType, string employeeId, string password, string requiredLevel);

    /// <summary>
    /// 根据操作类型获取所需权限级别。
    /// </summary>
    string GetRequiredLevel(string operationType);

    /// <summary>
    /// 记录权限确认结果（成功或失败）。
    /// </summary>
    Task RecordConfirmAsync(string operationType, string employeeId, string requiredLevel, bool success);
}
