using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Modules.SystemSettings.Models;
using MES.Modules.SystemSettings.Services;

namespace MES.Modules.SystemSettings.ViewModels;

public class SystemParamViewModel : BindableBase
{
    private readonly ISettingsService _service;
    private ObservableCollection<SystemParameter> _parameters = [];
    private SystemParameter? _selectedParameter;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private string? _filterCategory;
    private bool _isLoading;

    public ObservableCollection<SystemParameter> Parameters { get => _parameters; set => SetProperty(ref _parameters, value); }
    public SystemParameter? SelectedParameter { get => _selectedParameter; set { if (SetProperty(ref _selectedParameter, value)) RaiseCanExecuteChanged(); } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public string? FilterCategory { get => _filterCategory; set { if (SetProperty(ref _filterCategory, value)) ApplyFilter(); } }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int TotalCount => Parameters.Count;
    public int SystemParamCount => Parameters.Count(p => p.IsSystem);
    public List<string> Categories => Parameters.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<SystemParameter?> EditCommand { get; }
    public DelegateCommand<SystemParameter?> DeleteCommand { get; }
    public DelegateCommand SaveCommand { get; }

    public SystemParamViewModel(ISettingsService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        EditCommand = new DelegateCommand<SystemParameter?>(OnEdit, p => p != null);
        DeleteCommand = new DelegateCommand<SystemParameter?>(OnDelete, p => p != null && !p.IsSystem);
        SaveCommand = new DelegateCommand(OnSave);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"参数加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task ReloadDataAsync()
    {
        var items = await _service.GetAllParametersAsync();
        Parameters = new ObservableCollection<SystemParameter>(items.OrderBy(p => p.Category).ThenBy(p => p.ParamCode));
        UpdateStatistics();
        RaisePropertyChanged(nameof(Categories));
    }

    private void ApplyFilter()
    {
        var query = Parameters.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var k = SearchText.Trim().ToLower();
            query = query.Where(p => p.ParamName.ToLower().Contains(k) || p.ParamCode.ToLower().Contains(k) ||
                                     p.ParamValue.ToLower().Contains(k) || p.Description.ToLower().Contains(k));
        }
        if (!string.IsNullOrWhiteSpace(FilterCategory))
            query = query.Where(p => p.Category == FilterCategory);

        Parameters = new ObservableCollection<SystemParameter>(query.OrderBy(p => p.ParamCode));
        UpdateStatistics();
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        FilterCategory = null;
        _ = ReloadDataAsync();
    }

    private void OnEdit(SystemParameter? p)
    {
        if (p == null) return;
        var newValue = Microsoft.VisualBasic.Interaction.InputBox(
            $"参数: {p.ParamName}\n当前值: {p.ParamValue}\n\n请输入新值:",
            "编辑参数", p.ParamValue);
        if (!string.IsNullOrEmpty(newValue) && newValue != p.ParamValue)
        {
            _ = UpdateParameterValueAsync(p.ParamCode, newValue);
        }
    }

    private async Task UpdateParameterValueAsync(string paramCode, string value)
    {
        try
        {
            await _service.UpdateParameterValueAsync(paramCode, value, "当前用户");
            await ReloadDataAsync();
        }
        catch (Exception ex) { ErrorMessage = $"更新失败: {ex.Message}"; }
    }

    private async void OnDelete(SystemParameter? p)
    {
        if (p == null || p.IsSystem) return;
        var result = System.Windows.MessageBox.Show($"确定要删除参数 '{p.ParamName}' 吗?", "确认删除",
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _service.DeleteParameterAsync(p.ParamCode);
                await ReloadDataAsync();
            }
            catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; }
        }
    }

    private void OnSave() { }

    private void RaiseCanExecuteChanged() { EditCommand.RaiseCanExecuteChanged(); DeleteCommand.RaiseCanExecuteChanged(); }
    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(SystemParamCount));
    }
}

public class UserPermissionViewModel : BindableBase
{
    private readonly ISettingsService _service;
    private ObservableCollection<UserInfo> _users = [];
    private ObservableCollection<RoleInfo> _roles = [];
    private ObservableCollection<UserRoleMapping> _userRoleMappings = [];
    private ObservableCollection<PermissionInfo> _permissions = [];
    private UserInfo? _selectedUser;
    private RoleInfo? _selectedRole;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private string? _filterStatus;
    private bool _isLoading;
    private string _activeTab = "Users";

    public ObservableCollection<UserInfo> Users { get => _users; set => SetProperty(ref _users, value); }
    public ObservableCollection<RoleInfo> Roles { get => _roles; set => SetProperty(ref _roles, value); }
    public ObservableCollection<UserRoleMapping> UserRoleMappings { get => _userRoleMappings; set => SetProperty(ref _userRoleMappings, value); }
    public ObservableCollection<PermissionInfo> Permissions { get => _permissions; set => SetProperty(ref _permissions, value); }
    public UserInfo? SelectedUser { get => _selectedUser; set { if (SetProperty(ref _selectedUser, value)) { RaiseCanExecuteChanged(); OnUserSelected(); } } }
    public RoleInfo? SelectedRole { get => _selectedRole; set { if (SetProperty(ref _selectedRole, value)) { RaiseCanExecuteChanged(); OnRoleSelected(); } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public string? FilterStatus { get => _filterStatus; set { if (SetProperty(ref _filterStatus, value)) ApplyFilter(); } }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string ActiveTab { get => _activeTab; set => SetProperty(ref _activeTab, value); }

    public int TotalUsers => Users.Count;
    public int ActiveUsers => Users.Count(u => u.Status == "Active");
    public int LockedUsers => Users.Count(u => u.Status == "Locked");
    public int TotalRoles => Roles.Count;
    public int TotalPermissions => Permissions.Count;

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<UserInfo?> EditUserCommand { get; }
    public DelegateCommand<UserInfo?> ToggleUserStatusCommand { get; }
    public DelegateCommand<RoleInfo?> EditRoleCommand { get; }
    public DelegateCommand AssignRoleCommand { get; }
    public DelegateCommand<UserRoleMapping?> RemoveRoleCommand { get; }

    public UserPermissionViewModel(ISettingsService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        EditUserCommand = new DelegateCommand<UserInfo?>(OnEditUser, u => u != null);
        ToggleUserStatusCommand = new DelegateCommand<UserInfo?>(OnToggleUserStatus, u => u != null && u.UserName != "admin");
        EditRoleCommand = new DelegateCommand<RoleInfo?>(OnEditRole, r => r != null);
        AssignRoleCommand = new DelegateCommand(OnAssignRole, () => SelectedUser != null);
        RemoveRoleCommand = new DelegateCommand<UserRoleMapping?>(OnRemoveRole, m => m != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task ReloadDataAsync()
    {
        var users = await _service.GetAllUsersAsync();
        Users = new ObservableCollection<UserInfo>(users.OrderBy(u => u.UserName));
        UpdateUserStatistics();

        var roles = await _service.GetAllRolesAsync();
        Roles = new ObservableCollection<RoleInfo>(roles.OrderBy(r => r.RoleCode));
        UpdateRoleStatistics();

        var permissions = await _service.GetAllPermissionsAsync();
        Permissions = new ObservableCollection<PermissionInfo>(permissions.OrderBy(p => p.Module).ThenBy(p => p.PermissionCode));
    }

    private void ApplyFilter()
    {
        if (ActiveTab == "Users")
        {
            var query = Users.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var k = SearchText.Trim().ToLower();
                query = query.Where(u => u.UserName.ToLower().Contains(k) || u.DisplayName.ToLower().Contains(k) ||
                                         u.Email.ToLower().Contains(k) || u.Department.ToLower().Contains(k));
            }
            if (!string.IsNullOrWhiteSpace(FilterStatus))
                query = query.Where(u => u.Status == FilterStatus);
            Users = new ObservableCollection<UserInfo>(query.OrderBy(u => u.UserName));
            UpdateUserStatistics();
        }
        else if (ActiveTab == "Roles")
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var k = SearchText.Trim().ToLower();
                var query = Roles.Where(r => r.RoleName.ToLower().Contains(k) || r.RoleCode.ToLower().Contains(k) ||
                                             r.Description.ToLower().Contains(k));
                Roles = new ObservableCollection<RoleInfo>(query.OrderBy(r => r.RoleCode));
                UpdateRoleStatistics();
            }
        }
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        FilterStatus = null;
        _ = ReloadDataAsync();
    }

    private async void OnUserSelected()
    {
        if (SelectedUser == null)
        {
            UserRoleMappings = [];
            return;
        }
        try
        {
            var mappings = await _service.GetUserRoleMappingsAsync(SelectedUser.UserId);
            UserRoleMappings = new ObservableCollection<UserRoleMapping>(mappings);
        }
        catch (Exception ex) { ErrorMessage = $"加载用户角色失败: {ex.Message}"; }
    }

    private async void OnRoleSelected()
    {
        if (SelectedRole == null) return;
        try
        {
            var users = await _service.GetUsersByRoleAsync(SelectedRole.RoleId);
            // 可以展示角色下的用户列表
        }
        catch (Exception ex) { ErrorMessage = $"加载角色用户失败: {ex.Message}"; }
    }

    private void OnEditUser(UserInfo? u)
    {
        if (u == null) return;
        System.Windows.MessageBox.Show(
            $"用户详情\n\n用户名: {u.UserName}\n姓名: {u.DisplayName}\n邮箱: {u.Email}\n部门: {u.Department}\n职位: {u.Position}\n电话: {u.Phone}\n状态: {u.Status}\n最后登录: {(u.LastLogin?.ToString("yyyy-MM-dd HH:mm") ?? "从未登录")}\n登录失败次数: {u.LoginFailCount}",
            "用户详情", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    private async void OnToggleUserStatus(UserInfo? u)
    {
        if (u == null || u.UserName == "admin") return;
        var newStatus = u.Status == "Active" ? "Inactive" : "Active";
        var result = System.Windows.MessageBox.Show($"确定要将用户 '{u.DisplayName}' 的状态更改为 {newStatus} 吗?", "确认",
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _service.UpdateUserStatusAsync(u.UserId, newStatus);
                await ReloadDataAsync();
            }
            catch (Exception ex) { ErrorMessage = $"状态更新失败: {ex.Message}"; }
        }
    }

    private void OnEditRole(RoleInfo? r)
    {
        if (r == null) return;
        System.Windows.MessageBox.Show(
            $"角色详情\n\n角色代码: {r.RoleCode}\n角色名称: {r.RoleName}\n描述: {r.Description}\n用户数量: {r.UserCount}\n系统角色: {r.IsSystem}",
            "角色详情", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    private void OnAssignRole()
    {
        if (SelectedUser == null) return;
        System.Windows.MessageBox.Show($"为用户 '{SelectedUser.DisplayName}' 分配角色功能", "分配角色",
            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    private async void OnRemoveRole(UserRoleMapping? m)
    {
        if (m == null) return;
        var result = System.Windows.MessageBox.Show($"确定要移除用户 '{m.UserName}' 的角色 '{m.RoleName}' 吗?", "确认",
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _service.RemoveRoleFromUserAsync(m.MappingId);
                OnUserSelected();
                await ReloadDataAsync();
            }
            catch (Exception ex) { ErrorMessage = $"移除失败: {ex.Message}"; }
        }
    }

    private void RaiseCanExecuteChanged()
    {
        EditUserCommand.RaiseCanExecuteChanged();
        ToggleUserStatusCommand.RaiseCanExecuteChanged();
        AssignRoleCommand.RaiseCanExecuteChanged();
        RemoveRoleCommand.RaiseCanExecuteChanged();
        EditRoleCommand.RaiseCanExecuteChanged();
    }

    private void UpdateUserStatistics()
    {
        RaisePropertyChanged(nameof(TotalUsers));
        RaisePropertyChanged(nameof(ActiveUsers));
        RaisePropertyChanged(nameof(LockedUsers));
    }

    private void UpdateRoleStatistics()
    {
        RaisePropertyChanged(nameof(TotalRoles));
    }
}

public class OperationLogViewModel : BindableBase
{
    private readonly ISettingsService _service;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<OperationLog> _logs = [];
    private OperationLog? _selectedLog;
    private string? _errorMessage;
    private DateTime? _startDate;
    private DateTime? _endDate;
    private string? _filterModule;
    private string? _filterOperator;
    private string? _filterOperationType;
    private bool _isLoading;

    public ObservableCollection<OperationLog> Logs { get => _logs; set => SetProperty(ref _logs, value); }
    public OperationLog? SelectedLog { get => _selectedLog; set { if (SetProperty(ref _selectedLog, value)) RaiseCanExecuteChanged(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public DateTime? StartDate { get => _startDate; set { if (SetProperty(ref _startDate, value)) ApplyFilter(); } }
    public DateTime? EndDate { get => _endDate; set { if (SetProperty(ref _endDate, value)) ApplyFilter(); } }
    public string? FilterModule { get => _filterModule; set { if (SetProperty(ref _filterModule, value)) ApplyFilter(); } }
    public string? FilterOperator { get => _filterOperator; set { if (SetProperty(ref _filterOperator, value)) ApplyFilter(); } }
    public string? FilterOperationType { get => _filterOperationType; set { if (SetProperty(ref _filterOperationType, value)) ApplyFilter(); } }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int TotalCount => Logs.Count;
    public int SuccessCount => Logs.Count(l => l.Result == "Success");
    public int FailedCount => Logs.Count(l => l.Result == "Failed");
    public double AvgDuration => Logs.Any() ? Math.Round(Logs.Average(l => l.Duration), 0) : 0;
    public List<string> Modules { get; private set; } = [];
    public List<string> OperationTypes { get; private set; } = [];

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<OperationLog?> ViewDetailCommand { get; }

    public OperationLogViewModel(ISettingsService service, IRegionManager regionManager)
    {
        _service = service;
        _regionManager = regionManager;
        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        ViewDetailCommand = new DelegateCommand<OperationLog?>(OnViewDetail, l => l != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"日志加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task ReloadDataAsync()
    {
        Modules = await _service.GetDistinctModulesAsync();
        OperationTypes = await _service.GetDistinctOperationTypesAsync();
        RaisePropertyChanged(nameof(Modules));
        RaisePropertyChanged(nameof(OperationTypes));

        var filterStartDate = StartDate ?? DateTime.Now.AddDays(-7);
        var filterEndDate = EndDate ?? DateTime.Now;
        var logs = await _service.GetOperationLogsAsync(filterStartDate, filterEndDate, FilterModule, FilterOperator, FilterOperationType);
        Logs = new ObservableCollection<OperationLog>(logs);
        UpdateStatistics();
    }

    private void ApplyFilter()
    {
        _ = ReloadDataAsync();
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void OnClearFilter()
    {
        StartDate = null;
        EndDate = null;
        FilterModule = null;
        FilterOperator = null;
        FilterOperationType = null;
        _ = ReloadDataAsync();
    }

    private void OnViewDetail(OperationLog? l)
    {
        if (l == null) return;
        var parameters = new NavigationParameters { { "LogId", l.LogId } };
        _regionManager.RequestNavigate("MainContentRegion", "OperationLogDetailView", parameters);
    }

    private void RaiseCanExecuteChanged() { ViewDetailCommand.RaiseCanExecuteChanged(); }
    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(SuccessCount));
        RaisePropertyChanged(nameof(FailedCount));
        RaisePropertyChanged(nameof(AvgDuration));
    }
}

public class SystemMonitorViewModel : BindableBase
{
    private readonly ISettingsService _service;
    private ObservableCollection<SystemMonitorMetric> _metrics = [];
    private ObservableCollection<ServiceStatus> _services = [];
    private DatabaseConnectionStatus? _databaseStatus;
    private string? _errorMessage;
    private string? _filterMetricType;
    private bool _isLoading;

    public ObservableCollection<SystemMonitorMetric> Metrics { get => _metrics; set => SetProperty(ref _metrics, value); }
    public ObservableCollection<ServiceStatus> Services { get => _services; set => SetProperty(ref _services, value); }
    public DatabaseConnectionStatus? DatabaseStatus { get => _databaseStatus; set => SetProperty(ref _databaseStatus, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public string? FilterMetricType { get => _filterMetricType; set { if (SetProperty(ref _filterMetricType, value)) ApplyFilter(); } }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int TotalServices => Services.Count;
    public int RunningServices => Services.Count(s => s.Status == "Running");
    public int DegradedServices => Services.Count(s => s.Status == "Degraded");
    public int StoppedServices => Services.Count(s => s.Status == "Stopped");
    public double AvgResponseTime => Services.Any() ? Math.Round(Services.Where(s => s.ResponseTime > 0).Average(s => s.ResponseTime), 0) : 0;
    public List<string> MetricTypes { get; private set; } = [];

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }

    public SystemMonitorViewModel(ISettingsService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"监控数据加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task ReloadDataAsync()
    {
        var metrics = await _service.GetMonitorMetricsAsync();
        Metrics = new ObservableCollection<SystemMonitorMetric>(metrics.OrderByDescending(m => m.Timestamp).Take(100));
        MetricTypes = Metrics.Select(m => m.MetricType).Distinct().OrderBy(t => t).ToList();
        RaisePropertyChanged(nameof(MetricTypes));

        var services = await _service.GetServiceStatusesAsync();
        Services = new ObservableCollection<ServiceStatus>(services.OrderBy(s => s.ServiceName));
        UpdateServiceStatistics();

        DatabaseStatus = await _service.GetDatabaseStatusAsync();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterMetricType))
        {
            var metrics = _metrics.OrderByDescending(m => m.Timestamp).Take(100).ToList();
            Metrics = new ObservableCollection<SystemMonitorMetric>(metrics);
        }
        else
        {
            var metrics = _metrics.Where(m => m.MetricType == FilterMetricType)
                .OrderByDescending(m => m.Timestamp).Take(100).ToList();
            Metrics = new ObservableCollection<SystemMonitorMetric>(metrics);
        }
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void OnClearFilter()
    {
        FilterMetricType = null;
        ApplyFilter();
    }

    private void UpdateServiceStatistics()
    {
        RaisePropertyChanged(nameof(TotalServices));
        RaisePropertyChanged(nameof(RunningServices));
        RaisePropertyChanged(nameof(DegradedServices));
        RaisePropertyChanged(nameof(StoppedServices));
        RaisePropertyChanged(nameof(AvgResponseTime));
    }
}

public class SystemHealthViewModel : BindableBase
{
    private readonly ISettingsService _service;
    private SystemHealthReport? _healthReport;
    private ObservableCollection<HealthAlert> _alerts = [];
    private string? _errorMessage;
    private bool _isLoading;
    private bool? _filterAcknowledged;

    public SystemHealthReport? HealthReport { get => _healthReport; set => SetProperty(ref _healthReport, value); }
    public ObservableCollection<HealthAlert> Alerts { get => _alerts; set => SetProperty(ref _alerts, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool? FilterAcknowledged { get => _filterAcknowledged; set { if (SetProperty(ref _filterAcknowledged, value)) ApplyFilter(); } }

    public int TotalAlerts => Alerts.Count;
    public int UnacknowledgedCount => Alerts.Count(a => !a.IsAcknowledged);
    public int CriticalCount => Alerts.Count(a => a.Severity == "Critical");
    public int WarningCount => Alerts.Count(a => a.Severity == "Warning");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand<HealthAlert?> AcknowledgeCommand { get; }
    public DelegateCommand LoadReportCommand { get; }

    public SystemHealthViewModel(ISettingsService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AcknowledgeCommand = new DelegateCommand<HealthAlert?>(OnAcknowledge, a => a != null && !a.IsAcknowledged);
        LoadReportCommand = new DelegateCommand(OnLoadReport);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"健康报告加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task ReloadDataAsync()
    {
        HealthReport = await _service.GetSystemHealthReportAsync();
        var alerts = await _service.GetHealthAlertsAsync();
        Alerts = new ObservableCollection<HealthAlert>(alerts);
        UpdateStatistics();
    }

    private void ApplyFilter()
    {
        _ = LoadAlertsAsync();
    }

    private async Task LoadAlertsAsync()
    {
        try
        {
            var alerts = await _service.GetHealthAlertsAsync(FilterAcknowledged);
            Alerts = new ObservableCollection<HealthAlert>(alerts);
            UpdateStatistics();
        }
        catch (Exception ex) { ErrorMessage = $"告警加载失败: {ex.Message}"; }
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void OnAcknowledge(HealthAlert? alert)
    {
        if (alert == null || alert.IsAcknowledged) return;
        try
        {
            await _service.AcknowledgeAlertAsync(alert.AlertId, "当前用户");
            await LoadAlertsAsync();
            HealthReport = await _service.GetSystemHealthReportAsync();
        }
        catch (Exception ex) { ErrorMessage = $"确认失败: {ex.Message}"; }
    }

    private async void OnLoadReport()
    {
        try { HealthReport = await _service.GetSystemHealthReportAsync(); }
        catch (Exception ex) { ErrorMessage = $"报告加载失败: {ex.Message}"; }
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalAlerts));
        RaisePropertyChanged(nameof(UnacknowledgedCount));
        RaisePropertyChanged(nameof(CriticalCount));
        RaisePropertyChanged(nameof(WarningCount));
    }
}

public class ExternalSystemViewModel : BindableBase
{
    private readonly ISettingsService _service;
    private ObservableCollection<ExternalSystemConfig> _systems = [];
    private ObservableCollection<SyncRecord> _syncRecords = [];
    private ObservableCollection<ExternalSystemEvent> _events = [];
    private ExternalSystemConfig? _selectedSystem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isLoading;
    private string _activeTab = "Systems";

    public ObservableCollection<ExternalSystemConfig> Systems { get => _systems; set => SetProperty(ref _systems, value); }
    public ObservableCollection<SyncRecord> SyncRecords { get => _syncRecords; set => SetProperty(ref _syncRecords, value); }
    public ObservableCollection<ExternalSystemEvent> Events { get => _events; set => SetProperty(ref _events, value); }
    public ExternalSystemConfig? SelectedSystem { get => _selectedSystem; set { if (SetProperty(ref _selectedSystem, value)) { RaiseCanExecuteChanged(); OnSystemSelected(); } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string ActiveTab { get => _activeTab; set => SetProperty(ref _activeTab, value); }

    public int TotalSystems => Systems.Count;
    public int EnabledSystems => Systems.Count(s => s.IsEnabled);
    public int DisabledSystems => Systems.Count(s => !s.IsEnabled);
    public int TotalSyncCount => Systems.Sum(s => s.TotalSyncCount);
    public int TotalFailedSync => Systems.Sum(s => s.FailedSyncCount);
    public int PendingEvents => Events.Count(e => e.ProcessingStatus == "Pending");
    public int FailedEvents => Events.Count(e => e.ProcessingStatus == "Failed");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand<ExternalSystemConfig?> EditCommand { get; }
    public DelegateCommand<ExternalSystemConfig?> ToggleCommand { get; }
    public DelegateCommand<ExternalSystemConfig?> TriggerSyncCommand { get; }
    public DelegateCommand<ExternalSystemEvent?> ProcessEventCommand { get; }

    public ExternalSystemViewModel(ISettingsService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        EditCommand = new DelegateCommand<ExternalSystemConfig?>(OnEdit, s => s != null);
        ToggleCommand = new DelegateCommand<ExternalSystemConfig?>(OnToggle, s => s != null);
        TriggerSyncCommand = new DelegateCommand<ExternalSystemConfig?>(OnTriggerSync, s => s != null && s.IsEnabled);
        ProcessEventCommand = new DelegateCommand<ExternalSystemEvent?>(OnProcessEvent, e => e != null && e.ProcessingStatus == "Pending");
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"外部系统数据加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task ReloadDataAsync()
    {
        var systems = await _service.GetAllExternalSystemsAsync();
        Systems = new ObservableCollection<ExternalSystemConfig>(systems.OrderBy(s => s.SystemName));
        UpdateStatistics();
    }

    private void ApplyFilter()
    {
        if (ActiveTab == "Systems")
        {
            var query = Systems.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var k = SearchText.Trim().ToLower();
                query = query.Where(s => s.SystemName.ToLower().Contains(k) || s.SystemCode.ToLower().Contains(k) ||
                                         s.SystemType.ToLower().Contains(k));
            }
            Systems = new ObservableCollection<ExternalSystemConfig>(query.OrderBy(s => s.SystemName));
            UpdateStatistics();
        }
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void OnSystemSelected()
    {
        if (SelectedSystem == null)
        {
            SyncRecords = [];
            Events = [];
            return;
        }
        try
        {
            var records = await _service.GetSyncRecordsAsync(SelectedSystem.SystemId);
            SyncRecords = new ObservableCollection<SyncRecord>(records);

            var events = await _service.GetExternalEventsAsync(SelectedSystem.SystemId);
            Events = new ObservableCollection<ExternalSystemEvent>(events);
        }
        catch (Exception ex) { ErrorMessage = $"加载同步记录失败: {ex.Message}"; }
    }

    private void OnEdit(ExternalSystemConfig? s)
    {
        if (s == null) return;
        System.Windows.MessageBox.Show(
            $"外部系统配置详情\n\n系统名称: {s.SystemName}\n系统代码: {s.SystemCode}\n系统类型: {s.SystemType}\n端点: {s.Endpoint}\n认证方式: {s.AuthType}\n已启用: {s.IsEnabled}\n超时: {s.Timeout}秒\n重试次数: {s.RetryCount}\n最后同步: {s.LastSyncTime}\n同步状态: {s.LastSyncStatus}\n总同步次数: {s.TotalSyncCount}\n失败次数: {s.FailedSyncCount}",
            "系统配置详情", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    private async void OnToggle(ExternalSystemConfig? s)
    {
        if (s == null) return;
        var newStatus = !s.IsEnabled;
        var result = System.Windows.MessageBox.Show($"确定要{(newStatus ? "启用" : "禁用")}系统 '{s.SystemName}' 吗?", "确认",
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _service.UpdateExternalSystemStatusAsync(s.SystemId, newStatus);
                await ReloadDataAsync();
            }
            catch (Exception ex) { ErrorMessage = $"状态更新失败: {ex.Message}"; }
        }
    }

    private async void OnTriggerSync(ExternalSystemConfig? s)
    {
        if (s == null || !s.IsEnabled) return;
        var result = System.Windows.MessageBox.Show($"确定要手动触发系统 '{s.SystemName}' 的同步吗?", "确认同步",
            System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                await _service.TriggerSyncAsync(s.SystemId, "当前用户");
                await ReloadDataAsync();
                OnSystemSelected();
            }
            catch (Exception ex) { ErrorMessage = $"同步触发失败: {ex.Message}"; }
        }
    }

    private async void OnProcessEvent(ExternalSystemEvent? e)
    {
        if (e == null || e.ProcessingStatus != "Pending") return;
        try
        {
            await _service.ProcessEventAsync(e.EventId, "当前用户");
            OnSystemSelected();
        }
        catch (Exception ex) { ErrorMessage = $"事件处理失败: {ex.Message}"; }
    }

    private void RaiseCanExecuteChanged()
    {
        EditCommand.RaiseCanExecuteChanged();
        ToggleCommand.RaiseCanExecuteChanged();
        TriggerSyncCommand.RaiseCanExecuteChanged();
        ProcessEventCommand.RaiseCanExecuteChanged();
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalSystems));
        RaisePropertyChanged(nameof(EnabledSystems));
        RaisePropertyChanged(nameof(DisabledSystems));
        RaisePropertyChanged(nameof(TotalSyncCount));
        RaisePropertyChanged(nameof(TotalFailedSync));
        RaisePropertyChanged(nameof(PendingEvents));
        RaisePropertyChanged(nameof(FailedEvents));
    }
}
