using Prism.Mvvm;
using MES.Shared.Services;
using System.Windows.Threading;

namespace MES.Shell.ViewModels;

public class StatusBarViewModel : BindableBase
{
    private readonly ISessionService _session;

    private string _currentUser = "未登录";
    private string _currentRole = "—";
    private bool _isConnected = true;
    private int _activeAlarmCount = 0;
    private DateTime _systemTime = DateTime.Now;
    private readonly DispatcherTimer _timer;

    public string CurrentUser
    {
        get => _session.IsLoggedIn ? $"{_session.DisplayName}({_session.EmployeeId})" : "未登录";
        set => SetProperty(ref _currentUser, value);
    }

    public string CurrentRole
    {
        get => _session.IsLoggedIn ? _session.RoleName : "—";
        set => SetProperty(ref _currentRole, value);
    }

    public bool IsConnected { get => _isConnected; set => SetProperty(ref _isConnected, value); }
    public int ActiveAlarmCount { get => _activeAlarmCount; set => SetProperty(ref _activeAlarmCount, value); }
    public DateTime SystemTime { get => _systemTime; set => SetProperty(ref _systemTime, value); }

    public StatusBarViewModel(ISessionService session)
    {
        _session = session;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (s, e) =>
        {
            SystemTime = DateTime.Now;

            // 定时刷新用户信息（当 session 更新时自动反映）
            RaisePropertyChanged(nameof(CurrentUser));
            RaisePropertyChanged(nameof(CurrentRole));
        };
        _timer.Start();
    }
}