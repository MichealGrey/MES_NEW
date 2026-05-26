using Prism.Commands;
using Prism.Mvvm;
using MES.Domain.Production;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using MES.Shared.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace MES.Modules.Production.ViewModels;

public class LotHoldViewModel : BindableBase
{
    // --- 字段 ---
    private readonly IProductionDataService _dataService;
    private readonly ISessionService _session;
    private ObservableCollection<LotInfo> _allHoldLots = [];
    private ObservableCollection<LotInfo> _pagedHoldLots = [];
    private LotInfo? _selectedHoldLot;
    private string? _filterHoldType;
    private string? _filterHoldReason;
    private DateTime? _filterDateFrom;
    private DateTime? _filterDateTo;
    private string _searchText = string.Empty;
    private int _currentPage = 1;
    private int _pageSize = 20;
    private string? _errorMessage;

    // --- 属性 ---
    public ObservableCollection<LotInfo> AllHoldLots
    {
        get => _allHoldLots;
        set => SetProperty(ref _allHoldLots, value);
    }

    public ObservableCollection<LotInfo> PagedHoldLots
    {
        get => _pagedHoldLots;
        set => SetProperty(ref _pagedHoldLots, value);
    }

    public LotInfo? SelectedHoldLot
    {
        get => _selectedHoldLot;
        set
        {
            if (SetProperty(ref _selectedHoldLot, value))
                RaiseCanExecuteChanged();
        }
    }

    public string? FilterHoldType
    {
        get => _filterHoldType;
        set
        {
            if (SetProperty(ref _filterHoldType, value))
                ApplyFilterAndPaging();
        }
    }

    public string? FilterHoldReason
    {
        get => _filterHoldReason;
        set
        {
            if (SetProperty(ref _filterHoldReason, value))
                ApplyFilterAndPaging();
        }
    }

    public DateTime? FilterDateFrom
    {
        get => _filterDateFrom;
        set
        {
            if (SetProperty(ref _filterDateFrom, value))
                ApplyFilterAndPaging();
        }
    }

    public DateTime? FilterDateTo
    {
        get => _filterDateTo;
        set
        {
            if (SetProperty(ref _filterDateTo, value))
                ApplyFilterAndPaging();
        }
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

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    // --- 分页属性 ---
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

    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling((double)FilteredCount / PageSize)
        : 1;

    public string PageInfo => TotalPages > 0
        ? $"第 {CurrentPage}/{TotalPages} 页 (共 {FilteredCount} 条)"
        : $"共 {FilteredCount} 条";

    // --- 统计属性 ---
    public int TotalHoldCount => AllHoldLots.Count;
    public int EngineeringHoldCount => AllHoldLots.Count(l => l.HoldCategory == HoldType.Engineering);
    public int QualityHoldCount => AllHoldLots.Count(l => l.HoldCategory == HoldType.Quality);
    public int CustomerHoldCount => AllHoldLots.Count(l => l.HoldCategory == HoldType.Customer);
    public int MaterialHoldCount => AllHoldLots.Count(l => l.HoldCategory == HoldType.Material);
    public int EquipmentHoldCount => AllHoldLots.Count(l => l.HoldCategory == HoldType.Equipment);
    public int OverdueCount => AllHoldLots.Count(l => l.IsHoldOverdue);
    public int YieldHoldCount => AllHoldLots.Count(l => l.HoldCategory == HoldType.YieldHold);
    public int DataHoldCount => AllHoldLots.Count(l => l.HoldCategory == HoldType.DataHold);
    public int MrbHoldCount => AllHoldLots.Count(l => l.HoldCategory == HoldType.MRB);

    // --- 筛选可选项 ---
    public string[] HoldTypeOptions { get; } = Enum.GetNames<HoldType>();
    public string[] HoldReasonOptions { get; } =
        ["品质异常待检", "等物料(EMC/金线)", "工程指示", "设备维护", "工艺变更待确认", "客户要求暂停", "良率低于目标值", "测试数据未上传"];

    // --- 命令 ---
    public DelegateCommand HoldLotCommand { get; }
    public DelegateCommand<LotInfo?> ReleaseLotCommand { get; }
    public DelegateCommand BatchReleaseCommand { get; }
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand FirstPageCommand { get; }
    public DelegateCommand PreviousPageCommand { get; }
    public DelegateCommand NextPageCommand { get; }
    public DelegateCommand LastPageCommand { get; }

    public LotHoldViewModel(IProductionDataService dataService, ISessionService session)
    {
        _dataService = dataService;
        _session = session;

        HoldLotCommand = new DelegateCommand(OnHoldLot);
        ReleaseLotCommand = new DelegateCommand<LotInfo?>(OnReleaseLot, lot => lot != null);
        BatchReleaseCommand = new DelegateCommand(OnBatchRelease, () => AllHoldLots.Any(l => l.Status == "Hold"));
        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);

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
            await _dataService.EnsureSeededAsync();
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private async Task ReloadDataAsync()
    {
        var lots = await _dataService.GetAllHoldLotsAsync();
        AllHoldLots = new ObservableCollection<LotInfo>(lots);
        CurrentPage = 1;
        ApplyFilterAndPaging();
        UpdateStatistics();
        RaiseCanExecuteChanged();
        BatchReleaseCommand.RaiseCanExecuteChanged();
    }

    // --- 筛选 + 分页 ---
    private List<LotInfo> GetFilteredLots()
    {
        var query = AllHoldLots.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var key = SearchText.Trim().ToLower();
            query = query.Where(l =>
                l.LotId.ToLower().Contains(key)
                || l.OrderId.ToLower().Contains(key)
                || l.ProductName.ToLower().Contains(key)
                || l.DieName.ToLower().Contains(key)
                || l.CurrentStep.ToLower().Contains(key)
                || l.CurrentEquipment.ToLower().Contains(key));
        }

        if (!string.IsNullOrWhiteSpace(FilterHoldType) && Enum.TryParse<HoldType>(FilterHoldType, out var ht))
            query = query.Where(l => l.HoldCategory == ht);

        if (!string.IsNullOrWhiteSpace(FilterHoldReason))
            query = query.Where(l => l.HoldReason == FilterHoldReason);

        if (FilterDateFrom.HasValue)
            query = query.Where(l => l.HoldTime >= FilterDateFrom.Value);
        if (FilterDateTo.HasValue)
            query = query.Where(l => l.HoldTime < FilterDateTo.Value.AddDays(1));

        return query.ToList();
    }

    private void ApplyFilterAndPaging()
    {
        var filtered = GetFilteredLots();
        FilteredCount = filtered.Count;

        var totalPages = TotalPages;
        if (CurrentPage > totalPages) CurrentPage = Math.Max(1, totalPages);
        if (CurrentPage < 1) CurrentPage = 1;

        var pagedData = filtered
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        PagedHoldLots = new ObservableCollection<LotInfo>(pagedData);

        RaisePropertyChanged(nameof(FilteredCount));
        RaisePropertyChanged(nameof(TotalPages));
        RaisePropertyChanged(nameof(PageInfo));

        FirstPageCommand.RaiseCanExecuteChanged();
        PreviousPageCommand.RaiseCanExecuteChanged();
        NextPageCommand.RaiseCanExecuteChanged();
        LastPageCommand.RaiseCanExecuteChanged();
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalHoldCount));
        RaisePropertyChanged(nameof(EngineeringHoldCount));
        RaisePropertyChanged(nameof(QualityHoldCount));
        RaisePropertyChanged(nameof(CustomerHoldCount));
        RaisePropertyChanged(nameof(MaterialHoldCount));
        RaisePropertyChanged(nameof(EquipmentHoldCount));
        RaisePropertyChanged(nameof(OverdueCount));
        RaisePropertyChanged(nameof(YieldHoldCount));
        RaisePropertyChanged(nameof(DataHoldCount));
    }

    // --- 分页命令 ---
    private void OnFirstPage() { CurrentPage = 1; ApplyFilterAndPaging(); }
    private void OnPreviousPage() { if (CurrentPage > 1) { CurrentPage--; ApplyFilterAndPaging(); } }
    private void OnNextPage() { if (CurrentPage < TotalPages) { CurrentPage++; ApplyFilterAndPaging(); } }
    private void OnLastPage() { CurrentPage = TotalPages; ApplyFilterAndPaging(); }

    // --- 操作命令 ---
    private async void OnHoldLot()
    {
        if (!_session.HasPermission("Production", "LotHoldView"))
        {
            System.Windows.MessageBox.Show("您没有执行 Hold 操作的权限。", "权限不足",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            var dialog = new Views.HoldReasonDialog();
            if (dialog.ShowDialog() != true) return;

            var newLot = new LotInfo
            {
                LotId = $"LOT-{DateTime.Now:MMddHHmmss}",
                OrderId = "WO-2026001",
                ProductId = "PRD-001",
                ProductName = "IC-7nm-Logic",
                DieName = "IC-7nm-Logic",
                PackageType = PackageType.BGA,
                CurrentStep = "DieAttach",
                CurrentEquipment = "DB-01",
                Status = "Hold",
                UnitCount = 250,
                StripCount = 10,
                Priority = "High",
                CarrierType = CarrierType.Strip,
                CarrierId = $"STR-{DateTime.Now:MMddHHmmss}",
                HoldCategory = HoldType.Quality,
                HoldReason = dialog.HoldReason,
                HoldTime = DateTime.Now,
                HoldOperator = "Admin",
                ReleaseCondition = dialog.Remark
            };

            await _dataService.HoldLotAsync(newLot);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hold失败: {ex.Message}";
        }
    }

    private async void OnReleaseLot(LotInfo? lot)
    {
        if (lot == null) return;
        if (!_session.HasPermission("Production", "LotHoldView"))
        {
            System.Windows.MessageBox.Show("您没有释放批次的权限。", "权限不足",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            var result = System.Windows.MessageBox.Show(
                $"确认释放批次 {lot.LotId}？\n\n" +
                $"Hold原因: {lot.HoldReason}\n" +
                $"Hold时长: {lot.HoldDuration}\n\n" +
                $"释放后批次将恢复为 Waiting 状态，继续执行当前工序。",
                "确认释放",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result != System.Windows.MessageBoxResult.Yes) return;

            await _dataService.ReleaseLotAsync(lot.LotId);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"释放失败: {ex.Message}";
        }
    }

    private async void OnBatchRelease()
    {
        var holdCount = AllHoldLots.Count(l => l.Status == "Hold");
        if (holdCount == 0) return;
        if (!_session.HasPermission("Production", "LotHoldView"))
        {
            System.Windows.MessageBox.Show("您没有批量释放批次的权限。", "权限不足",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            var result = System.Windows.MessageBox.Show(
                $"确认批量释放所有 Hold 批次？\n\n当前共有 {holdCount} 个 Hold 批次将被释放。",
                "批量释放确认",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result != System.Windows.MessageBoxResult.Yes) return;

            await _dataService.BatchReleaseAsync();
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"批量释放失败: {ex.Message}";
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

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        FilterHoldType = null;
        FilterHoldReason = null;
        FilterDateFrom = null;
        FilterDateTo = null;
    }

    private void RaiseCanExecuteChanged()
    {
        ReleaseLotCommand.RaiseCanExecuteChanged();
    }
}
