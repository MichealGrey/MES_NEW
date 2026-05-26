using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class SystemMonitorViewModel : BindableBase
{
    private readonly IReportService _reportService;

    private SystemMonitor? _monitor;
    private ObservableCollection<AlertInfo> _alerts = [];
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public SystemMonitor? Monitor
    {
        get => _monitor;
        set => SetProperty(ref _monitor, value);
    }

    public ObservableCollection<AlertInfo> Alerts
    {
        get => _alerts;
        set => SetProperty(ref _alerts, value);
    }

    public ICommand RefreshCommand { get; }

    public SystemMonitorViewModel(IReportService reportService)
    {
        _reportService = reportService;
        RefreshCommand = new DelegateCommand(async () => await RefreshAsync());
    }

    private async System.Threading.Tasks.Task RefreshAsync()
    {
        Monitor = await _reportService.GetSystemSnapshotAsync();
        if (Monitor is not null)
        {
            Alerts = new ObservableCollection<AlertInfo>(Monitor.Alerts);
        }
    }
}
