using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Lot.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace MES.Modules.Lot.ViewModels;

public class LotListViewModel : BindableBase
{
    private readonly ILotService _lotService;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<LotInfo> _lots = [];
    private ObservableCollection<LotInfo> _pagedLots = [];
    private string _searchText = string.Empty;
    private LotInfo? _selectedLot;
    private string? _filterStatus;
    private string? _filterStage;
    private int _currentPage = 1;
    private int _pageSize = 20;
    private string? _errorMessage;

    public ObservableCollection<LotInfo> Lots
    {
        get => _lots;
        set => SetProperty(ref _lots, value);
    }

    public ObservableCollection<LotInfo> PagedLots
    {
        get => _pagedLots;
        set => SetProperty(ref _pagedLots, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                ApplyFilterAndPaging();
        }
    }

    public LotInfo? SelectedLot
    {
        get => _selectedLot;
        set
        {
            if (SetProperty(ref _selectedLot, value))
                RaiseCanExecuteChanged();
        }
    }

    public string? FilterStatus
    {
        get => _filterStatus;
        set
        {
            if (SetProperty(ref _filterStatus, value))
                ApplyFilterAndPaging();
        }
    }

    public string? FilterStage
    {
        get => _filterStage;
        set
        {
            if (SetProperty(ref _filterStage, value))
                ApplyFilterAndPaging();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public int CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetProperty(ref _pageSize, value) && value > 0)
                ApplyFilterAndPaging();
        }
    }

    public int FilteredCount { get; private set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)FilteredCount / PageSize) : 1;
    public string PageInfo => TotalPages > 0 ? $"第 {CurrentPage}/{TotalPages} 页 (共 {FilteredCount} 条)" : $"共 {FilteredCount} 条";

    public int TotalCount => Lots.Count;
    public int CreatedCount => Lots.Count(l => l.Status == "Created");
    public int InProgressCount => Lots.Count(l => l.Status == "InProgress");
    public int CompletedCount => Lots.Count(l => l.Status == "Completed");
    public int HoldCount => Lots.Count(l => l.Status == "Hold");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<LotInfo?> HoldCommand { get; }
    public DelegateCommand<LotInfo?> ReleaseCommand { get; }
    public DelegateCommand<LotInfo?> ArchiveCommand { get; }
    public DelegateCommand<LotInfo?> ViewDetailCommand { get; }
    public DelegateCommand FirstPageCommand { get; }
    public DelegateCommand PreviousPageCommand { get; }
    public DelegateCommand NextPageCommand { get; }
    public DelegateCommand LastPageCommand { get; }

    public LotListViewModel(ILotService lotService, IRegionManager regionManager)
    {
        _lotService = lotService;
        _regionManager = regionManager;

        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        HoldCommand = new DelegateCommand<LotInfo?>(OnHold, l => l != null && (l.Status == "InProgress" || l.Status == "Created"));
        ReleaseCommand = new DelegateCommand<LotInfo?>(OnRelease, l => l != null && l.Status == "Hold");
        ArchiveCommand = new DelegateCommand<LotInfo?>(OnArchive, l => l != null && (l.Status == "Completed" || l.Status == "Closed"));
        ViewDetailCommand = new DelegateCommand<LotInfo?>(OnViewDetail, l => l != null);

        FirstPageCommand = new DelegateCommand(OnFirstPage, () => CurrentPage > 1);
        PreviousPageCommand = new DelegateCommand(OnPreviousPage, () => CurrentPage > 1);
        NextPageCommand = new DelegateCommand(OnNextPage, () => CurrentPage < TotalPages);
        LastPageCommand = new DelegateCommand(OnLastPage, () => CurrentPage < TotalPages);

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
        Lots = new ObservableCollection<LotInfo>(lots);
        ApplyFilterAndPaging();
        UpdateStatistics();
        RaiseCanExecuteChanged();
    }

    private List<LotInfo> GetFilteredLots()
    {
        var query = Lots.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var key = SearchText.Trim().ToLower();
            query = query.Where(l =>
                l.LotId.ToLower().Contains(key)
                || l.ProductName.ToLower().Contains(key)
                || l.OrderId.ToLower().Contains(key)
                || l.DieName.ToLower().Contains(key));
        }

        if (!string.IsNullOrWhiteSpace(FilterStatus))
            query = query.Where(l => l.Status == FilterStatus);

        if (!string.IsNullOrWhiteSpace(FilterStage))
            query = query.Where(l => l.ProcessStage == FilterStage);

        return query.ToList();
    }

    private void ApplyFilterAndPaging()
    {
        var filtered = GetFilteredLots();
        FilteredCount = filtered.Count;

        var totalPages = TotalPages;
        if (CurrentPage > totalPages) CurrentPage = Math.Max(1, totalPages);
        if (CurrentPage < 1) CurrentPage = 1;

        var pagedData = filtered.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        PagedLots = new ObservableCollection<LotInfo>(pagedData);

        RaisePropertyChanged(nameof(FilteredCount));
        RaisePropertyChanged(nameof(TotalPages));
        RaisePropertyChanged(nameof(PageInfo));

        FirstPageCommand.RaiseCanExecuteChanged();
        PreviousPageCommand.RaiseCanExecuteChanged();
        NextPageCommand.RaiseCanExecuteChanged();
        LastPageCommand.RaiseCanExecuteChanged();
    }

    private void OnFirstPage() { CurrentPage = 1; ApplyFilterAndPaging(); }
    private void OnPreviousPage() { if (CurrentPage > 1) { CurrentPage--; ApplyFilterAndPaging(); } }
    private void OnNextPage() { if (CurrentPage < TotalPages) { CurrentPage++; ApplyFilterAndPaging(); } }
    private void OnLastPage() { CurrentPage = TotalPages; ApplyFilterAndPaging(); }

    private async void OnRefresh()
    {
        try
        {
            ErrorMessage = null;
            CurrentPage = 1;
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"刷新失败: {ex.Message}";
        }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        FilterStatus = null;
        FilterStage = null;
    }

    private async void OnHold(LotInfo? lot)
    {
        if (lot == null) return;
        try
        {
            var reason = Microsoft.VisualBasic.Interaction.InputBox("请输入Hold原因:", "批次Hold", "");
            if (string.IsNullOrWhiteSpace(reason)) return;

            await _lotService.HoldLotAsync(lot.LotId, reason);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hold失败: {ex.Message}";
        }
    }

    private async void OnRelease(LotInfo? lot)
    {
        if (lot == null) return;
        try
        {
            if (System.Windows.MessageBox.Show($"确定释放批次 {lot.LotId}?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
                return;

            await _lotService.ReleaseLotAsync(lot.LotId);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"释放失败: {ex.Message}";
        }
    }

    private async void OnArchive(LotInfo? lot)
    {
        if (lot == null) return;
        try
        {
            if (System.Windows.MessageBox.Show($"确定归档批次 {lot.LotId}?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
                return;

            await _lotService.ArchiveLotAsync(lot.LotId);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"归档失败: {ex.Message}";
        }
    }

    private void OnViewDetail(LotInfo? lot)
    {
        if (lot == null) return;
        var parameters = new NavigationParameters { { "LotId", lot.LotId } };
        _regionManager.RequestNavigate("MainContentRegion", "LotDetailView", parameters);
    }

    private void RaiseCanExecuteChanged()
    {
        HoldCommand.RaiseCanExecuteChanged();
        ReleaseCommand.RaiseCanExecuteChanged();
        ArchiveCommand.RaiseCanExecuteChanged();
        ViewDetailCommand.RaiseCanExecuteChanged();
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(CreatedCount));
        RaisePropertyChanged(nameof(InProgressCount));
        RaisePropertyChanged(nameof(CompletedCount));
        RaisePropertyChanged(nameof(HoldCount));
    }
}
