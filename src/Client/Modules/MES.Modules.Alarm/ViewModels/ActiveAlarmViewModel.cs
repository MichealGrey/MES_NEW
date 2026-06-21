using Prism.Mvvm;
using Prism.Commands;
using MES.Modules.Alarm.Models;
using MES.Modules.Alarm.Services;
using System.Collections.ObjectModel;

namespace MES.Modules.Alarm.ViewModels;

public class ActiveAlarmViewModel : BindableBase
{
    private readonly IAlarmService _alarmService;
    private ObservableCollection<ActiveAlarmItem> _items = [];
    private ActiveAlarmItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _totalCount;
    private int _criticalCount;
    private int _majorCount;
    private int _minorCount;

    public ObservableCollection<ActiveAlarmItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public ActiveAlarmItem? SelectedItem { get => _selectedItem; set { SetProperty(ref _selectedItem, value); UpdateCommandStates(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int CriticalCount { get => _criticalCount; set => SetProperty(ref _criticalCount, value); }
    public int MajorCount { get => _majorCount; set => SetProperty(ref _majorCount, value); }
    public int MinorCount { get => _minorCount; set => SetProperty(ref _minorCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AcknowledgeCommand { get; }
    public DelegateCommand CloseCommand { get; }

    public ActiveAlarmViewModel(IAlarmService alarmService)
    {
        _alarmService = alarmService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        AcknowledgeCommand = new DelegateCommand(async () => await AcknowledgeSelectedAsync(), () => SelectedItem?.Status == "Active");
        CloseCommand = new DelegateCommand(async () => await CloseSelectedAsync(), () => SelectedItem?.Status == "Acknowledged");
        LoadMockData();
    }

    private void UpdateCommandStates()
    {
        AcknowledgeCommand.RaiseCanExecuteChanged();
        CloseCommand.RaiseCanExecuteChanged();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _alarmService.GetActiveAlarmsAsync();
            Items = new ObservableCollection<ActiveAlarmItem>(data);
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task AcknowledgeSelectedAsync()
    {
        if (SelectedItem == null || SelectedItem.Status != "Active") return;
        IsLoading = true;
        try
        {
            await _alarmService.AcknowledgeAlarmAsync(SelectedItem.AlarmId, "CurrentUser");
            SelectedItem.Status = "Acknowledged";
            SelectedItem.AckBy = "CurrentUser";
            UpdateStatistics();
            UpdateCommandStates();
            ErrorMessage = $"告警 {SelectedItem.AlarmId} 已确认";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"确认失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CloseSelectedAsync()
    {
        if (SelectedItem == null || SelectedItem.Status != "Acknowledged") return;
        IsLoading = true;
        try
        {
            await _alarmService.CloseAlarmAsync(SelectedItem.AlarmId, "CurrentUser");
            SelectedItem.Status = "Closed";
            UpdateStatistics();
            UpdateCommandStates();
            ErrorMessage = $"告警 {SelectedItem.AlarmId} 已关闭";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"关闭失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        CriticalCount = Items.Count(a => a.Severity == "Critical");
        MajorCount = Items.Count(a => a.Severity == "Major");
        MinorCount = Items.Count(a => a.Severity == "Minor");
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<ActiveAlarmItem>
        {
            new() { AlarmId = "ALM-001", AlarmCode = "E301", EquipmentId = "EQ-003", Severity = "Critical", Text = "腔体温度超上限 (>250C)", AlarmTime = DateTime.Now.AddMinutes(-5), Duration = "00:05:12", Status = "Active" },
            new() { AlarmId = "ALM-002", AlarmCode = "E102", EquipmentId = "EQ-008", Severity = "Major", Text = "RF功率偏差超过10%", AlarmTime = DateTime.Now.AddMinutes(-15), Duration = "00:15:30", Status = "Active" },
            new() { AlarmId = "ALM-003", AlarmCode = "E205", EquipmentId = "EQ-012", Severity = "Minor", Text = "冷却水流量偏低", AlarmTime = DateTime.Now.AddHours(-1), Duration = "01:02:45", Status = "Active" },
            new() { AlarmId = "ALM-004", AlarmCode = "E401", EquipmentId = "EQ-001", Severity = "Critical", Text = "对准标记偏差超限", AlarmTime = DateTime.Now.AddMinutes(-2), Duration = "00:02:18", Status = "Active" },
            new() { AlarmId = "ALM-005", AlarmCode = "E110", EquipmentId = "EQ-015", Severity = "Major", Text = "真空度超出规格", AlarmTime = DateTime.Now.AddMinutes(-30), Duration = "00:30:55", Status = "Active" },
            new() { AlarmId = "ALM-006", AlarmCode = "E308", EquipmentId = "EQ-005", Severity = "Minor", Text = "气体流量波动", AlarmTime = DateTime.Now.AddHours(-2), Duration = "02:15:20", Status = "Active" },
            new() { AlarmId = "ALM-007", AlarmCode = "E501", EquipmentId = "EQ-010", Severity = "Critical", Text = "束流异常中断", AlarmTime = DateTime.Now.AddMinutes(-8), Duration = "00:08:33", Status = "Active" },
            new() { AlarmId = "ALM-008", AlarmCode = "E220", EquipmentId = "EQ-018", Severity = "Minor", Text = "排风系统压力低", AlarmTime = DateTime.Now.AddHours(-3), Duration = "03:10:05", Status = "Active" },
        };
        UpdateStatistics();
    }
}

public class AlarmHistoryViewModel : BindableBase
{
    private readonly IAlarmService _alarmService;
    private ObservableCollection<AlarmHistoryItem> _items = [];
    private AlarmHistoryItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private DateTime? _startDate;
    private DateTime? _endDate;
    private int _totalCount;
    private int _closedCount;

    public ObservableCollection<AlarmHistoryItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public AlarmHistoryItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public DateTime? StartDate { get => _startDate; set => SetProperty(ref _startDate, value); }
    public DateTime? EndDate { get => _endDate; set => SetProperty(ref _endDate, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int ClosedCount { get => _closedCount; set => SetProperty(ref _closedCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SearchCommand { get; }

    public AlarmHistoryViewModel(IAlarmService alarmService)
    {
        _alarmService = alarmService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SearchCommand = new DelegateCommand(async () => await SearchHistoryAsync());
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _alarmService.GetAlarmHistoryAsync();
            Items = new ObservableCollection<AlarmHistoryItem>(data);
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SearchHistoryAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _alarmService.GetAlarmHistoryAsync(StartDate, EndDate);
            Items = new ObservableCollection<AlarmHistoryItem>(data);
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"搜索失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        ClosedCount = Items.Count(h => h.Status == "Closed");
    }

    private void LoadMockData()
    {
        var now = DateTime.Now;
        Items = new ObservableCollection<AlarmHistoryItem>
        {
            new() { AlarmId = "ALM-101", AlarmCode = "E301", EquipmentId = "EQ-003", Severity = "Critical", Text = "腔体温度超上限", AlarmTime = now.AddHours(-5), AckTime = now.AddHours(-5).AddMinutes(3), AckBy = "Zhang Wei", CloseTime = now.AddHours(-4), Status = "Closed" },
            new() { AlarmId = "ALM-102", AlarmCode = "E102", EquipmentId = "EQ-008", Severity = "Major", Text = "RF功率偏差", AlarmTime = now.AddHours(-8), AckTime = now.AddHours(-8).AddMinutes(10), AckBy = "Li Ming", CloseTime = now.AddHours(-6), Status = "Closed" },
            new() { AlarmId = "ALM-103", AlarmCode = "E205", EquipmentId = "EQ-012", Severity = "Minor", Text = "冷却水流量偏低", AlarmTime = now.AddHours(-12), AckTime = now.AddHours(-12).AddMinutes(15), AckBy = "Wang Fang", CloseTime = now.AddHours(-10), Status = "Closed" },
            new() { AlarmId = "ALM-104", AlarmCode = "E401", EquipmentId = "EQ-001", Severity = "Critical", Text = "对准标记偏差超限", AlarmTime = now.AddDays(-1), AckTime = now.AddDays(-1).AddMinutes(5), AckBy = "Liu Jun", CloseTime = now.AddDays(-1).AddHours(2), Status = "Closed" },
            new() { AlarmId = "ALM-105", AlarmCode = "E110", EquipmentId = "EQ-015", Severity = "Major", Text = "真空度超出规格", AlarmTime = now.AddDays(-1).AddHours(-3), AckTime = now.AddDays(-1).AddHours(-3).AddMinutes(8), AckBy = "Chen Lei", CloseTime = now.AddDays(-1).AddHours(-1), Status = "Closed" },
            new() { AlarmId = "ALM-106", AlarmCode = "E308", EquipmentId = "EQ-005", Severity = "Minor", Text = "气体流量波动", AlarmTime = now.AddDays(-2), AckTime = now.AddDays(-2).AddMinutes(20), AckBy = "Zhang Wei", CloseTime = now.AddDays(-2).AddHours(3), Status = "Closed" },
            new() { AlarmId = "ALM-107", AlarmCode = "E501", EquipmentId = "EQ-010", Severity = "Critical", Text = "束流异常中断", AlarmTime = now.AddDays(-2).AddHours(-5), AckTime = now.AddDays(-2).AddHours(-5).AddMinutes(2), AckBy = "Li Ming", CloseTime = now.AddDays(-2).AddHours(-3), Status = "Closed" },
            new() { AlarmId = "ALM-108", AlarmCode = "E220", EquipmentId = "EQ-018", Severity = "Minor", Text = "排风系统压力低", AlarmTime = now.AddDays(-3), AckTime = now.AddDays(-3).AddMinutes(30), AckBy = "Wang Fang", CloseTime = now.AddDays(-3).AddHours(5), Status = "Closed" },
        };
        UpdateStatistics();
    }
}

public class AlarmRuleConfigViewModel : BindableBase
{
    private readonly IAlarmService _alarmService;
    private ObservableCollection<AlarmRuleItem> _items = [];
    private AlarmRuleItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _totalCount;
    private int _enabledCount;

    public ObservableCollection<AlarmRuleItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public AlarmRuleItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int EnabledCount { get => _enabledCount; set => SetProperty(ref _enabledCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand DeleteCommand { get; }

    public AlarmRuleConfigViewModel(IAlarmService alarmService)
    {
        _alarmService = alarmService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveSelectedRuleAsync());
        DeleteCommand = new DelegateCommand(async () => await DeleteSelectedRuleAsync());
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _alarmService.GetAlarmRulesAsync();
            Items = new ObservableCollection<AlarmRuleItem>(data);
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SaveSelectedRuleAsync()
    {
        if (SelectedItem == null) return;
        IsLoading = true;
        try
        {
            await _alarmService.SaveAlarmRuleAsync(SelectedItem);
            await LoadDataAsync();
            ErrorMessage = "规则保存成功";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteSelectedRuleAsync()
    {
        if (SelectedItem == null) return;
        IsLoading = true;
        try
        {
            await _alarmService.DeleteAlarmRuleAsync(SelectedItem.AlarmCode);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        EnabledCount = Items.Count(r => r.Enabled);
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<AlarmRuleItem>
        {
            new() { AlarmCode = "E301", AlarmName = "温度超限", EquipmentType = "MoldingPress", Severity = "Critical", AutoHold = true, Description = "模压温度超过185C时触发", Enabled = true },
            new() { AlarmCode = "E102", AlarmName = "超声波功率异常", EquipmentType = "WireBonder", Severity = "Major", AutoHold = true, Description = "超声波功率偏差超过10%", Enabled = true },
            new() { AlarmCode = "E205", AlarmName = "冷却水流量低", EquipmentType = "DicingSaw", Severity = "Minor", AutoHold = false, Description = "冷却水流量低于5L/min", Enabled = true },
            new() { AlarmCode = "E401", AlarmName = "贴装偏差超限", EquipmentType = "DieBonder", Severity = "Critical", AutoHold = true, Description = "芯片贴装偏移超过50μm", Enabled = true },
            new() { AlarmCode = "E110", AlarmName = "真空度异常", EquipmentType = "MoldingPress", Severity = "Major", AutoHold = true, Description = "模压腔体真空度超出设定值", Enabled = false },
            new() { AlarmCode = "E308", AlarmName = "EMC流量波动", EquipmentType = "MoldingPress", Severity = "Minor", AutoHold = false, Description = "EMC注塑流量波动超过5%", Enabled = true },
            new() { AlarmCode = "E501", AlarmName = "测试分选异常", EquipmentType = "TestHandler", Severity = "Critical", AutoHold = true, Description = "测试分选良率低于设定下限", Enabled = true },
            new() { AlarmCode = "E220", AlarmName = "排风压力低", EquipmentType = "All", Severity = "Minor", AutoHold = false, Description = "排风系统压力低于设定值", Enabled = true },
        };
        UpdateStatistics();
    }
}

public class AlarmRuleViewModel : BindableBase
{
    private readonly IAlarmService _alarmService;
    private ObservableCollection<AlarmRuleItem> _items = [];
    private AlarmRuleItem? _selectedItem;
    private string _searchText = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _totalCount;
    private int _enabledCount;
    private int _criticalRules;
    private int _autoHoldCount;

    public ObservableCollection<AlarmRuleItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public AlarmRuleItem? SelectedItem { get => _selectedItem; set { SetProperty(ref _selectedItem, value); UpdateCommandStates(); } }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int EnabledCount { get => _enabledCount; set => SetProperty(ref _enabledCount, value); }
    public int CriticalRules { get => _criticalRules; set => SetProperty(ref _criticalRules, value); }
    public int AutoHoldCount { get => _autoHoldCount; set => SetProperty(ref _autoHoldCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddRuleCommand { get; }
    public DelegateCommand SaveRuleCommand { get; }
    public DelegateCommand DeleteRuleCommand { get; }
    public DelegateCommand ToggleEnabledCommand { get; }

    public AlarmRuleViewModel(IAlarmService alarmService)
    {
        _alarmService = alarmService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        AddRuleCommand = new DelegateCommand(AddNewRule);
        SaveRuleCommand = new DelegateCommand(async () => await SaveSelectedRuleAsync(), () => SelectedItem != null);
        DeleteRuleCommand = new DelegateCommand(async () => await DeleteSelectedRuleAsync(), () => SelectedItem != null);
        ToggleEnabledCommand = new DelegateCommand(async () => await ToggleRuleEnabledAsync(), () => SelectedItem != null);
        LoadMockData();
    }

    private void UpdateCommandStates()
    {
        SaveRuleCommand.RaiseCanExecuteChanged();
        DeleteRuleCommand.RaiseCanExecuteChanged();
        ToggleEnabledCommand.RaiseCanExecuteChanged();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _alarmService.GetAlarmRulesAsync();
            Items = new ObservableCollection<AlarmRuleItem>(data);
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SaveSelectedRuleAsync()
    {
        if (SelectedItem == null) return;
        IsLoading = true;
        try
        {
            await _alarmService.SaveAlarmRuleAsync(SelectedItem);
            ErrorMessage = "规则保存成功";
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteSelectedRuleAsync()
    {
        if (SelectedItem == null) return;
        IsLoading = true;
        try
        {
            await _alarmService.DeleteAlarmRuleAsync(SelectedItem.AlarmCode);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ToggleRuleEnabledAsync()
    {
        if (SelectedItem == null) return;
        SelectedItem.Enabled = !SelectedItem.Enabled;
        IsLoading = true;
        try
        {
            await _alarmService.ToggleRuleStatusAsync(SelectedItem.AlarmCode, SelectedItem.Enabled);
            UpdateStatistics();
            ErrorMessage = $"规则 {SelectedItem.AlarmCode} 已{(SelectedItem.Enabled ? "启用" : "禁用")}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"切换状态失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddNewRule()
    {
        var newRule = new AlarmRuleItem
        {
            AlarmCode = $"E{new Random().Next(600, 999)}",
            AlarmName = "新规则",
            EquipmentType = "All",
            Severity = "Minor",
            AutoHold = false,
            Enabled = true,
            Description = "请输入规则描述"
        };
        Items.Add(newRule);
        SelectedItem = newRule;
        TotalCount = Items.Count;
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            LoadMockData();
            return;
        }

        var filtered = Items.Where(r =>
            r.AlarmName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            r.AlarmCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            r.EquipmentType.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

        Items = new ObservableCollection<AlarmRuleItem>(filtered);
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        EnabledCount = Items.Count(r => r.Enabled);
        CriticalRules = Items.Count(r => r.Severity == "Critical");
        AutoHoldCount = Items.Count(r => r.AutoHold);
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<AlarmRuleItem>
        {
            new() { AlarmCode = "E301", AlarmName = "温度超限", EquipmentType = "MoldingPress", Severity = "Critical", AutoHold = true, Description = "模压温度超过185C时触发", Enabled = true },
            new() { AlarmCode = "E102", AlarmName = "超声波功率异常", EquipmentType = "WireBonder", Severity = "Major", AutoHold = true, Description = "超声波功率偏差超过10%", Enabled = true },
            new() { AlarmCode = "E205", AlarmName = "冷却水流量低", EquipmentType = "DicingSaw", Severity = "Minor", AutoHold = false, Description = "冷却水流量低于5L/min", Enabled = true },
            new() { AlarmCode = "E401", AlarmName = "贴装偏差超限", EquipmentType = "DieBonder", Severity = "Critical", AutoHold = true, Description = "芯片贴装偏移超过50μm", Enabled = true },
            new() { AlarmCode = "E110", AlarmName = "真空度异常", EquipmentType = "MoldingPress", Severity = "Major", AutoHold = true, Description = "模压腔体真空度超出设定值", Enabled = false },
            new() { AlarmCode = "E308", AlarmName = "EMC流量波动", EquipmentType = "MoldingPress", Severity = "Minor", AutoHold = false, Description = "EMC注塑流量波动超过5%", Enabled = true },
            new() { AlarmCode = "E501", AlarmName = "测试分选异常", EquipmentType = "TestHandler", Severity = "Critical", AutoHold = true, Description = "测试分选良率低于设定下限", Enabled = true },
            new() { AlarmCode = "E220", AlarmName = "排风压力低", EquipmentType = "All", Severity = "Minor", AutoHold = false, Description = "排风系统压力低于设定值", Enabled = true },
        };
        UpdateStatistics();
    }
}

public class AlarmStatisticsViewModel : BindableBase
{
    private readonly IAlarmService _alarmService;
    private ObservableCollection<AlarmStatisticsItem> _items = [];
    private AlarmStatisticsItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _totalAlarms;
    private int _criticalTotal;
    private int _majorTotal;
    private int _minorTotal;
    private double _avgAckTime;

    public ObservableCollection<AlarmStatisticsItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public AlarmStatisticsItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int TotalAlarms { get => _totalAlarms; set => SetProperty(ref _totalAlarms, value); }
    public int CriticalTotal { get => _criticalTotal; set => SetProperty(ref _criticalTotal, value); }
    public int MajorTotal { get => _majorTotal; set => SetProperty(ref _majorTotal, value); }
    public int MinorTotal { get => _minorTotal; set => SetProperty(ref _minorTotal, value); }
    public double AvgAckTime { get => _avgAckTime; set => SetProperty(ref _avgAckTime, value); }

    public DelegateCommand RefreshCommand { get; }

    public AlarmStatisticsViewModel(IAlarmService alarmService)
    {
        _alarmService = alarmService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var stats = await _alarmService.GetStatisticsAsync();
            Items = new ObservableCollection<AlarmStatisticsItem>(stats.DailyTrend);
            TotalAlarms = stats.TotalAlarms;
            CriticalTotal = stats.CriticalCount;
            MajorTotal = stats.MajorCount;
            MinorTotal = stats.MinorCount;
            AvgAckTime = stats.AvgAckTimeMinutes;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void LoadMockData()
    {
        var baseDate = DateTime.Today;
        Items = new ObservableCollection<AlarmStatisticsItem>
        {
            new() { Date = baseDate.AddDays(-6), CriticalCount = 2, MajorCount = 5, MinorCount = 12, TotalCount = 19 },
            new() { Date = baseDate.AddDays(-5), CriticalCount = 1, MajorCount = 3, MinorCount = 8, TotalCount = 12 },
            new() { Date = baseDate.AddDays(-4), CriticalCount = 3, MajorCount = 7, MinorCount = 15, TotalCount = 25 },
            new() { Date = baseDate.AddDays(-3), CriticalCount = 0, MajorCount = 2, MinorCount = 6, TotalCount = 8 },
            new() { Date = baseDate.AddDays(-2), CriticalCount = 1, MajorCount = 4, MinorCount = 10, TotalCount = 15 },
            new() { Date = baseDate.AddDays(-1), CriticalCount = 2, MajorCount = 6, MinorCount = 11, TotalCount = 19 },
            new() { Date = baseDate, CriticalCount = 1, MajorCount = 2, MinorCount = 5, TotalCount = 8 },
        };
        TotalAlarms = Items.Sum(i => i.TotalCount);
        CriticalTotal = Items.Sum(i => i.CriticalCount);
        MajorTotal = Items.Sum(i => i.MajorCount);
        MinorTotal = Items.Sum(i => i.MinorCount);
        AvgAckTime = 8.5;
    }
}
