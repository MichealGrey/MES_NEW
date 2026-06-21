using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using MES.Modules.Execute.Services;

namespace MES.Modules.Execute.ViewModels;

public class WipOverviewViewModel : BindableBase
{
    private readonly IExecuteService _executeService;
    private ObservableCollection<WipInfo> _wipList = [];
    private string? _errorMessage;

    public ObservableCollection<WipInfo> WipList
    {
        get => _wipList;
        set => SetProperty(ref _wipList, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public int TotalWip => WipList.Count;
    public int FrontendWip => WipList.Count(w => w.ProcessStage == "Frontend");
    public int BackendWip => WipList.Count(w => w.ProcessStage == "Backend");
    public double? AvgCycleTime => WipList.Any(w => w.ElapsedMinutes.HasValue) 
        ? WipList.Where(w => w.ElapsedMinutes.HasValue).Average(w => w.ElapsedMinutes!.Value) 
        : null;

    public DelegateCommand RefreshCommand { get; }

    public WipOverviewViewModel(IExecuteService executeService)
    {
        _executeService = executeService;

        RefreshCommand = new DelegateCommand(OnRefresh);

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
        var wip = await _executeService.GetAllWipAsync();
        WipList = new ObservableCollection<WipInfo>(wip);
        UpdateStatistics();
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

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalWip));
        RaisePropertyChanged(nameof(FrontendWip));
        RaisePropertyChanged(nameof(BackendWip));
        RaisePropertyChanged(nameof(AvgCycleTime));
    }
}
