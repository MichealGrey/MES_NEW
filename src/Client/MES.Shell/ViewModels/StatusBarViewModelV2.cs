using System;
using Prism.Mvvm;
using System.Windows.Threading;

namespace MES.Shell.ViewModels
{
    public class StatusBarViewModelV2 : BindableBase
    {
        private DateTime _systemTime;
        public DateTime SystemTime
        {
            get => _systemTime;
            set => SetProperty(ref _systemTime, value);
        }

        private bool _isConnected = true;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private string _statusMessage = "系统运行正常";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private string _currentUser = "当前用户：张明";
        public string CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        private string _shiftInfo = "班组：早班 08:00-20:00";
        public string ShiftInfo
        {
            get => _shiftInfo;
            set => SetProperty(ref _shiftInfo, value);
        }

        private int _activeAlarmCount = 0;
        public int ActiveAlarmCount
        {
            get => _activeAlarmCount;
            set => SetProperty(ref _activeAlarmCount, value);
        }

        public bool HasAlarms => ActiveAlarmCount > 0;

        private string _appVersion = "V2.0.0";
        public string AppVersion
        {
            get => _appVersion;
            set => SetProperty(ref _appVersion, value);
        }

        private readonly DispatcherTimer _timer;

        public StatusBarViewModelV2()
        {
            SystemTime = DateTime.Now;
            
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (s, e) => SystemTime = DateTime.Now;
            _timer.Start();
        }
    }
}
