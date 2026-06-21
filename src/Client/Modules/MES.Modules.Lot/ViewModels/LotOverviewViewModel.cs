using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using MES.Modules.Lot.Services;

namespace MES.Modules.Lot.ViewModels;

public class LotOverviewViewModel : BindableBase
{
    private readonly ILotService _lotService;
    private ObservableCollection<LotInfo> _allLots = [];
    private string? _errorMessage;

    public ObservableCollection<LotInfo> AllLots
    {
        get => _allLots;
        set => SetProperty(ref _allLots, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public int TotalCount => AllLots.Count;
    public int CreatedCount => AllLots.Count(l => l.Status == "Created");
    public int InProgressCount => AllLots.Count(l => l.Status == "InProgress");
    public int CompletedCount => AllLots.Count(l => l.Status == "Completed");
    public int HoldCount => AllLots.Count(l => l.Status == "Hold");
    public int ArchivedCount => AllLots.Count(l => l.IsArchived);
    public int FrontendCount => AllLots.Count(l => l.ProcessStage == "Frontend");
    public int BackendCount => AllLots.Count(l => l.ProcessStage == "Backend");

    public DelegateCommand RefreshCommand { get; }

    public LotOverviewViewModel(ILotService lotService)
    {
        _lotService = lotService;

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
        var lots = await _lotService.GetAllLotsAsync();
        AllLots = new ObservableCollection<LotInfo>(lots);
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
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(CreatedCount));
        RaisePropertyChanged(nameof(InProgressCount));
        RaisePropertyChanged(nameof(CompletedCount));
        RaisePropertyChanged(nameof(HoldCount));
        RaisePropertyChanged(nameof(ArchivedCount));
        RaisePropertyChanged(nameof(FrontendCount));
        RaisePropertyChanged(nameof(BackendCount));
    }
}
