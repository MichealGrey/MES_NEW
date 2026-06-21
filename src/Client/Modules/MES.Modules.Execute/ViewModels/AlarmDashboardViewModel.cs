using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using MES.Modules.Execute.Services;

namespace MES.Modules.Execute.ViewModels;

public class AlarmDashboardViewModel : BindableBase
{
    private readonly IExecuteService _executeService;
    private ObservableCollection<AlarmInfo> _alarms = [];
    private AlarmInfo? _selectedAlarm;
    private string? _filterSeverity;
    private string? _errorMessage;

    public ObservableCollection<AlarmInfo> Alarms
    {
        get => _alarms;
        set => SetProperty(ref _alarms, value);
    }

    public AlarmInfo? SelectedAlarm
    {
        get => _selectedAlarm;
        set => SetProperty(ref _selectedAlarm, value);
    }

    public string? FilterSeverity
    {
        get => _filterSeverity;
        set
        {
            if (SetProperty(ref _filterSeverity, value))
                ApplyFilter();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public int ActiveCount => Alarms.Count(a => a.Status == "Active");
    public int AcknowledgedCount => Alarms.Count(a => a.Status == "Acknowledged");
    public int ClearedCount => Alarms.Count(a => a.Status == "Cleared");
    public int CriticalCount => Alarms.Count(a => a.Severity == "Critical");
    public int WarningCount => Alarms.Count(a => a.Severity == "Warning");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AcknowledgeCommand { get; }
    public DelegateCommand ClearCommand { get; }

    public AlarmDashboardViewModel(IExecuteService executeService)
    {
        _executeService = executeService;

        RefreshCommand = new DelegateCommand(OnRefresh);
        AcknowledgeCommand = new DelegateCommand(OnAcknowledge, () => SelectedAlarm != null && SelectedAlarm.Status == "Active");
        ClearCommand = new DelegateCommand(OnClear, () => SelectedAlarm != null && (SelectedAlarm.Status == "Active" || SelectedAlarm.Status == "Acknowledged"));

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            ErrorMessage = null;
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private async Task ReloadDataAsync()
    {
        var alarms = await _executeService.GetActiveAlarmsAsync();
        Alarms = new ObservableCollection<AlarmInfo>(alarms);
        UpdateStatistics();
        RaiseCanExecuteChanged();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterSeverity))
        {
            _ = ReloadDataAsync();
            return;
        }

        var filtered = Alarms.Where(a => a.Severity == FilterSeverity).ToList();
        Alarms = new ObservableCollection<AlarmInfo>(filtered);
    }

    private async void OnAcknowledge()
    {
        if (SelectedAlarm == null) return;
        try
        {
            await _executeService.AcknowledgeAlarmAsync(SelectedAlarm.AlarmId);
            SelectedAlarm.Status = "Acknowledged";
            SelectedAlarm.AcknowledgedAt = DateTime.UtcNow;
            UpdateStatistics();
            RaiseCanExecuteChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"确认失败: {ex.Message}";
        }
    }

    private async void OnClear()
    {
        if (SelectedAlarm == null) return;
        try
        {
            await _executeService.ClearAlarmAsync(SelectedAlarm.AlarmId);
            SelectedAlarm.Status = "Cleared";
            SelectedAlarm.ClearedAt = DateTime.UtcNow;
            UpdateStatistics();
            RaiseCanExecuteChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"清除失败: {ex.Message}";
        }
    }

    private async void OnRefresh()
    {
        try
        {
            ErrorMessage = null;
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"刷新失败: {ex.Message}";
        }
    }

    private void RaiseCanExecuteChanged()
    {
        AcknowledgeCommand.RaiseCanExecuteChanged();
        ClearCommand.RaiseCanExecuteChanged();
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(ActiveCount));
        RaisePropertyChanged(nameof(AcknowledgedCount));
        RaisePropertyChanged(nameof(ClearedCount));
        RaisePropertyChanged(nameof(CriticalCount));
        RaisePropertyChanged(nameof(WarningCount));
    }
}
