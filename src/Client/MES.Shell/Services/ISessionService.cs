using MES.Shared.Services;

namespace MES.Shell.Services;

/// <summary>
/// Shell 层扩展的会话服务接口，继承 Shared 层的基础接口。
/// 添加了锁屏、切换用户等 Shell 专属功能。
/// </summary>
public interface ISessionService : MES.Shared.Services.ISessionService
{
    bool IsLocked { get; }

    void LockScreen();
    bool UnlockScreen(string password, IUserAuthenticationService authService);
    Task<bool> SwitchUserAsync(string employeeId, string password, IUserAuthenticationService authService);
    List<string> GetFrequentUsers();
    void AddToFrequentUsers(string employeeId);
}
