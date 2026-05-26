using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class AlarmDashboardViewModel : BindableBase
{
    private readonly IAlarmService _alarmService;
    private ObservableCollection<AlarmRecord> _alarms = new();
    private ObservableCollection<AlarmRecord> _filteredAlarms = new();
    private int _unresolvedCount;
    private int _todayCount;
    private int _resolvedCount;
    private string _avgResponseTime = "—";
    private int _filterSeverityIndex;
    private int _filterTypeIndex;
    private bool _showUnresolvedOnly;

    public ObservableCollection<AlarmRecord> Alarms
    {
        get => _alarms;
        set => SetProperty(ref _alarms, value);
    }

    public ObservableCollection<AlarmRecord> FilteredAlarms
    {
        get => _filteredAlarms;
        set => SetProperty(ref _filteredAlarms, value);
    }

    public int UnresolvedCount { get => _unresolvedCount; set => SetProperty(ref _unresolvedCount, value); }
    public int TodayCount { get => _todayCount; set => SetProperty(ref _todayCount, value); }
    public int ResolvedCount { get => _resolvedCount; set => SetProperty(ref _resolvedCount, value); }
    public string AvgResponseTime { get => _avgResponseTime; set => SetProperty(ref _avgResponseTime, value); }

    public int FilterSeverityIndex { get => _filterSeverityIndex; set { SetProperty(ref _filterSeverityIndex, value); ApplyFilters(); } }
    public int FilterTypeIndex { get => _filterTypeIndex; set { SetProperty(ref _filterTypeIndex, value); ApplyFilters(); } }
    public bool ShowUnresolvedOnly { get => _showUnresolvedOnly; set { SetProperty(ref _showUnresolvedOnly, value); ApplyFilters(); } }

    public ICommand RefreshCommand { get; }
    public ICommand AcknowledgeAllCommand { get; }
    public ICommand AcknowledgeCommand { get; }

    public AlarmDashboardViewModel(IAlarmService alarmService)
    {
        _alarmService = alarmService;
        RefreshCommand = new DelegateCommand(async () => await RefreshAsync());
        AcknowledgeAllCommand = new DelegateCommand(async () => await AcknowledgeAllAsync());
        AcknowledgeCommand = new DelegateCommand<AlarmRecord>(async (alarm) => await AcknowledgeAsync(alarm));
    }

    private async System.Threading.Tasks.Task RefreshAsync()
    {
        var allAlarms = await _alarmService.GetActiveAlarmsAsync();
        Alarms = new ObservableCollection<AlarmRecord>(allAlarms.OrderByDescending(a => a.TriggeredAt));

        UnresolvedCount = allAlarms.Count(a => !a.IsAcknowledged);
        ResolvedCount = allAlarms.Count(a => a.IsAcknowledged);
        TodayCount = allAlarms.Count(a => a.TriggeredAt.Date == System.DateTime.UtcNow.Date);

        var resolved = allAlarms.Where(a => a.IsAcknowledged && a.ResolvedAt.HasValue).ToList();
        if (resolved.Count > 0)
        {
            var avgMinutes = resolved.Average(a => (a.ResolvedAt!.Value - a.TriggeredAt).TotalMinutes);
            AvgResponseTime = avgMinutes < 60 ? $"{avgMinutes:F0}" : $"{avgMinutes / 60:F1}h";
        }

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = Alarms.ToList();

        if (FilterSeverityIndex > 0)
        {
            var severity = FilterSeverityIndex switch { 1 => "Critical", 2 => "Error", 3 => "Warning", 4 => "Info", _ => "" };
            if (!string.IsNullOrEmpty(severity))
                filtered = filtered.Where(a => a.Severity == severity).ToList();
        }

        if (FilterTypeIndex > 0)
        {
            var type = FilterTypeIndex switch { 1 => "LowYield", 2 => "QueueTimeout", 3 => "EquipmentDown", 4 => "QueueBacklog", _ => "" };
            if (!string.IsNullOrEmpty(type))
                filtered = filtered.Where(a => a.AlarmType == type).ToList();
        }

        if (ShowUnresolvedOnly)
            filtered = filtered.Where(a => !a.IsAcknowledged).ToList();

        FilteredAlarms = new ObservableCollection<AlarmRecord>(filtered);
    }

    private async System.Threading.Tasks.Task AcknowledgeAllAsync()
    {
        foreach (var alarm in Alarms.Where(a => !a.IsAcknowledged))
        {
            await _alarmService.AcknowledgeAlarmAsync(alarm.AlarmId, "System");
        }
        await RefreshAsync();
    }

    private async System.Threading.Tasks.Task AcknowledgeAsync(AlarmRecord? alarm)
    {
        if (alarm is null) return;
        await _alarmService.AcknowledgeAlarmAsync(alarm.AlarmId, "Operator");
        await RefreshAsync();
    }
}
