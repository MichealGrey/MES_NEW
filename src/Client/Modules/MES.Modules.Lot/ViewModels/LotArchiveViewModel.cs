using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Modules.Lot.Services;

namespace MES.Modules.Lot.ViewModels;

public class LotArchiveViewModel : BindableBase
{
    private readonly ILotService _lotService;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<LotInfo> _archivedLots = [];
    private LotInfo? _selectedLot;
    private string _searchText = string.Empty;
    private string? _errorMessage;

    public ObservableCollection<LotInfo> ArchivedLots
    {
        get => _archivedLots;
        set => SetProperty(ref _archivedLots, value);
    }

    public LotInfo? SelectedLot
    {
        get => _selectedLot;
        set => SetProperty(ref _selectedLot, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                ApplyFilter();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public int ArchivedCount => ArchivedLots.Count;

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand<LotInfo?> ViewDetailCommand { get; }

    public LotArchiveViewModel(ILotService lotService, IRegionManager regionManager)
    {
        _lotService = lotService;
        _regionManager = regionManager;

        RefreshCommand = new DelegateCommand(OnRefresh);
        ViewDetailCommand = new DelegateCommand<LotInfo?>(OnViewDetail, l => l != null);

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
        var lots = await _lotService.GetArchivedLotsAsync();
        ArchivedLots = new ObservableCollection<LotInfo>(lots);
        RaisePropertyChanged(nameof(ArchivedCount));
        ViewDetailCommand.RaiseCanExecuteChanged();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            _ = ReloadDataAsync();
            return;
        }

        var key = SearchText.Trim().ToLower();
        var filtered = ArchivedLots.Where(l =>
            l.LotId.ToLower().Contains(key)
            || l.ProductName.ToLower().Contains(key)
            || l.OrderId.ToLower().Contains(key)).ToList();

        ArchivedLots = new ObservableCollection<LotInfo>(filtered);
    }

    private async void OnRefresh()
    {
        try
        {
            ErrorMessage = null;
            SearchText = string.Empty;
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"刷新失败: {ex.Message}";
        }
    }

    private void OnViewDetail(LotInfo? lot)
    {
        if (lot == null) return;
        var parameters = new NavigationParameters { { "LotId", lot.LotId } };
        _regionManager.RequestNavigate("MainContentRegion", "LotDetailView", parameters);
    }
}
