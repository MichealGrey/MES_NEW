using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class ExternalSystemViewModel : BindableBase
{
    private readonly IExternalSystemService _externalService;
    private ObservableCollection<ExternalSystemEvent> _events = new();
    private ObservableCollection<ExternalSystemConfig> _systemConfigs = new();
    private int _selectedTabIndex;

    public ObservableCollection<ExternalSystemEvent> Events
    {
        get => _events;
        set => SetProperty(ref _events, value);
    }

    public ObservableCollection<ExternalSystemConfig> SystemConfigs
    {
        get => _systemConfigs;
        set => SetProperty(ref _systemConfigs, value);
    }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => SetProperty(ref _selectedTabIndex, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand RetryFailedCommand { get; }

    public ExternalSystemViewModel(IExternalSystemService externalService)
    {
        _externalService = externalService;
        RefreshCommand = new DelegateCommand(async () => await RefreshAsync());
        RetryFailedCommand = new DelegateCommand(async () => await RetryFailedAsync());
    }

    private async System.Threading.Tasks.Task RefreshAsync()
    {
        var events = await _externalService.GetPendingEventsAsync();
        Events = new ObservableCollection<ExternalSystemEvent>(events.OrderByDescending(e => e.CreatedAt));

        var configs = await _externalService.GetAllSystemConfigsAsync();
        SystemConfigs = new ObservableCollection<ExternalSystemConfig>(configs);
    }

    private async System.Threading.Tasks.Task RetryFailedAsync()
    {
        var failedEvents = Events.Where(e => e.Status == "Failed" && e.RetryCount < 3).ToList();
        int retried = 0;

        foreach (var evt in failedEvents)
        {
            // In a real app, this would call the external API again
            evt.RetryCount++;
            evt.Status = "Pending";
            evt.ErrorMessage = null;
            retried++;
        }

        System.Windows.MessageBox.Show($"已重发 {retried} 条失败事件", "重发完成", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        await RefreshAsync();
    }
}
