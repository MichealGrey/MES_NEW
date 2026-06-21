using Prism.Mvvm;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace MES.Shell.ViewModels
{
    public class HeaderViewModel : BindableBase
    {
        private string _currentModule = "首页";
        public string CurrentModule
        {
            get => _currentModule;
            set => SetProperty(ref _currentModule, value);
        }

        private string _breadcrumb = "OSAT MES > 首页";
        public string Breadcrumb
        {
            get => _breadcrumb;
            set => SetProperty(ref _breadcrumb, value);
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private string _userName = "张明";
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _userRole = "操作员 · WB-Line03";
        public string UserRole
        {
            get => _userRole;
            set => SetProperty(ref _userRole, value);
        }

        private int _notificationCount = 3;
        public int NotificationCount
        {
            get => _notificationCount;
            set => SetProperty(ref _notificationCount, value);
        }

        public ObservableCollection<ModuleInfo> Modules { get; } = new()
        {
            new ModuleInfo { Name = "🏠 首页" },
            new ModuleInfo { Name = "📋 工单" },
            new ModuleInfo { Name = "🏭 生产" },
            new ModuleInfo { Name = "📦 批次" },
            new ModuleInfo { Name = "⚙ 设备" },
            new ModuleInfo { Name = "🔬 质量" },
        };

        public DelegateCommand<string> SwitchModuleCommand { get; private set; }
        public DelegateCommand SearchCommand { get; private set; }
        public DelegateCommand OpenNotificationsCommand { get; private set; }

        public HeaderViewModel()
        {
            SwitchModuleCommand = new DelegateCommand<string>(SwitchModule);
            SearchCommand = new DelegateCommand(ExecuteSearch);
            OpenNotificationsCommand = new DelegateCommand(OpenNotifications);
        }

        private void SwitchModule(string module)
        {
            CurrentModule = module;
            Breadcrumb = $"OSAT MES > {module}";
        }

        private void ExecuteSearch()
        {
            // TODO: 实现全局搜索逻辑
        }

        private void OpenNotifications()
        {
            // TODO: 打开通知面板
        }
    }

    public class ModuleInfo
    {
        public string Name { get; set; } = string.Empty;
    }
}
