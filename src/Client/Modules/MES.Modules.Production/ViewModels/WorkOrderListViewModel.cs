using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using MES.Domain.Production;
using System.Collections.ObjectModel;
using System.Linq;

namespace MES.Modules.Production.ViewModels;

public class WorkOrderListViewModel : BindableBase
{
    // --- 字段 ---
    private readonly IProductionDataService _dataService;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<WorkOrderInfo> _workOrders = [];
    private ObservableCollection<WorkOrderInfo> _pagedWorkOrders = [];
    private string _searchText = string.Empty;
    private WorkOrderInfo? _selectedWorkOrder;
    private string? _filterStatus;
    private string? _filterPriority;
    private DateTime? _filterDateFrom;
    private DateTime? _filterDateTo;
    private int _currentPage = 1;
    private int _pageSize = 20;
    private string? _errorMessage;

    // --- 属性 ---
    public ObservableCollection<WorkOrderInfo> WorkOrders
    {
        get => _workOrders;
        set => SetProperty(ref _workOrders, value);
    }

    public ObservableCollection<WorkOrderInfo> PagedWorkOrders
    {
        get => _pagedWorkOrders;
        set => SetProperty(ref _pagedWorkOrders, value);
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

    public WorkOrderInfo? SelectedWorkOrder
    {
        get => _selectedWorkOrder;
        set
        {
            if (SetProperty(ref _selectedWorkOrder, value))
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

    public string? FilterPriority
    {
        get => _filterPriority;
        set
        {
            if (SetProperty(ref _filterPriority, value))
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
    public int TotalCount => WorkOrders.Count;
    public int CreatedCount => WorkOrders.Count(w => w.Status == ProcessStatus.Created);
    public int InProgressCount => WorkOrders.Count(w => w.Status == ProcessStatus.InProgress);
    public int CompletedCount => WorkOrders.Count(w => w.Status == ProcessStatus.Completed || w.Status == ProcessStatus.Closed);
    public int HoldCount => WorkOrders.Count(w => w.Status == ProcessStatus.Hold);

    // --- 命令 ---
    public DelegateCommand CreateCommand { get; }
    public DelegateCommand<WorkOrderInfo?> DeleteCommand { get; }
    public DelegateCommand<WorkOrderInfo?> ReleaseCommand { get; }
    public DelegateCommand<WorkOrderInfo?> HoldCommand { get; }
    public DelegateCommand<WorkOrderInfo?> ReleaseHoldCommand { get; }
    public DelegateCommand<WorkOrderInfo?> CloseCommand { get; }
    public DelegateCommand<WorkOrderInfo?> CancelCommand { get; }
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<WorkOrderInfo?> ViewDetailCommand { get; }

    public DelegateCommand FirstPageCommand { get; }
    public DelegateCommand PreviousPageCommand { get; }
    public DelegateCommand NextPageCommand { get; }
    public DelegateCommand LastPageCommand { get; }

    public WorkOrderListViewModel(IProductionDataService dataService, IRegionManager regionManager)
    {
        _dataService = dataService;
        _regionManager = regionManager;

        CreateCommand = new DelegateCommand(OnCreate);
        DeleteCommand = new DelegateCommand<WorkOrderInfo?>(OnDelete, wo => wo != null);
        ReleaseCommand = new DelegateCommand<WorkOrderInfo?>(OnRelease,
            wo => wo?.Status == ProcessStatus.Created);
        HoldCommand = new DelegateCommand<WorkOrderInfo?>(OnHold,
            wo => wo?.Status == ProcessStatus.Released || wo?.Status == ProcessStatus.InProgress);
        ReleaseHoldCommand = new DelegateCommand<WorkOrderInfo?>(OnReleaseHold,
            wo => wo?.Status == ProcessStatus.Hold);
        CloseCommand = new DelegateCommand<WorkOrderInfo?>(OnClose,
            wo => wo?.Status == ProcessStatus.InProgress || wo?.Status == ProcessStatus.Released);
        CancelCommand = new DelegateCommand<WorkOrderInfo?>(OnCancel,
            wo => wo?.Status == ProcessStatus.Created || wo?.Status == ProcessStatus.Released);
        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        ViewDetailCommand = new DelegateCommand<WorkOrderInfo?>(OnViewDetail, wo => wo != null);

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
        var orders = await _dataService.GetAllWorkOrdersAsync();
        WorkOrders = new ObservableCollection<WorkOrderInfo>(orders);
        ApplyFilterAndPaging();
        UpdateStatistics();
        RaiseCanExecuteChanged();
    }

    // --- 筛选 + 分页 ---
    private List<WorkOrderInfo> GetFilteredWorkOrders()
    {
        var query = WorkOrders.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var key = SearchText.Trim().ToLower();
            query = query.Where(wo =>
                wo.OrderId.ToLower().Contains(key)
                || wo.ProductName.ToLower().Contains(key)
                || wo.RouteName.ToLower().Contains(key)
                || wo.CustomerName.ToLower().Contains(key)
                || wo.DieName.ToLower().Contains(key)
                || wo.PackageTypeDisplay.ToLower().Contains(key));
        }

        if (!string.IsNullOrWhiteSpace(FilterStatus) && Enum.TryParse<ProcessStatus>(FilterStatus, out var statusFilter))
            query = query.Where(wo => wo.Status == statusFilter);

        if (!string.IsNullOrWhiteSpace(FilterPriority) && Enum.TryParse<WorkOrderPriority>(FilterPriority, out var priorityFilter))
            query = query.Where(wo => wo.Priority == priorityFilter);

        if (FilterDateFrom.HasValue)
            query = query.Where(wo => wo.PlannedStartDate >= FilterDateFrom.Value);
        if (FilterDateTo.HasValue)
            query = query.Where(wo => wo.PlannedStartDate < FilterDateTo.Value.AddDays(1));

        return query.ToList();
    }

    private void ApplyFilterAndPaging()
    {
        var filtered = GetFilteredWorkOrders();
        FilteredCount = filtered.Count;

        var totalPages = TotalPages;
        if (CurrentPage > totalPages) CurrentPage = Math.Max(1, totalPages);
        if (CurrentPage < 1) CurrentPage = 1;

        var pagedData = filtered
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        PagedWorkOrders = new ObservableCollection<WorkOrderInfo>(pagedData);

        RaisePropertyChanged(nameof(FilteredCount));
        RaisePropertyChanged(nameof(TotalPages));
        RaisePropertyChanged(nameof(PageInfo));

        FirstPageCommand.RaiseCanExecuteChanged();
        PreviousPageCommand.RaiseCanExecuteChanged();
        NextPageCommand.RaiseCanExecuteChanged();
        LastPageCommand.RaiseCanExecuteChanged();
    }

    // --- 分页命令 ---
    private void OnFirstPage() { CurrentPage = 1; ApplyFilterAndPaging(); }
    private void OnPreviousPage() { if (CurrentPage > 1) { CurrentPage--; ApplyFilterAndPaging(); } }
    private void OnNextPage() { if (CurrentPage < TotalPages) { CurrentPage++; ApplyFilterAndPaging(); } }
    private void OnLastPage() { CurrentPage = TotalPages; ApplyFilterAndPaging(); }

    // --- 命令实现 ---
    private async void OnCreate()
    {
        try
        {
            var win = new Views.AddWorkOrderWin();
            if (win.ShowDialog() == true)
            {
                var newOrder = new WorkOrderInfo
                {
                    OrderId = win.OrderNo,
                    ProductName = win.ProductName,
                    ProductId = win.ProductId ?? string.Empty,
                    DieName = win.DieName ?? string.Empty,
                    PackageType = win.PackageType,
                    RouteId = win.RouteId ?? string.Empty,
                    PlannedQty = win.PlannedQty,
                    UnitQty = win.UnitQty,
                    Priority = win.Priority,
                    CustomerName = win.CustomerName ?? string.Empty,
                    Area = win.Area ?? string.Empty,
                    Line = win.Line ?? string.Empty,
                    SpecId = win.SpecId ?? string.Empty,
                    PlannedStartDate = win.PlannedStartDate,
                    PlannedEndDate = win.PlannedEndDate,
                    YieldTarget = win.YieldTarget,
                    Remark = win.Remark,
                    Creator = "Admin"
                };
                await _dataService.SaveWorkOrderAsync(newOrder);
                await ReloadDataAsync();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"创建工单失败: {ex.Message}";
        }
    }

    private async void OnDelete(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        try
        {
            if (System.Windows.MessageBox.Show(
                $"确定删除工单 {wo.OrderId}？\n\n注意：半导体MES通常不建议删除工单，建议使用'取消'操作。",
                "确认删除",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning) != System.Windows.MessageBoxResult.Yes)
                return;
            await _dataService.DeleteWorkOrderAsync(wo.OrderId);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
        }
    }

    private async void OnRelease(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        try
        {
            await _dataService.UpdateWorkOrderStatusAsync(wo.OrderId, ProcessStatus.Released);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"下达失败: {ex.Message}";
        }
    }

    private async void OnHold(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        try
        {
            var dialog = new Views.HoldReasonDialog();
            if (dialog.ShowDialog() != true) return;

            await _dataService.HoldWorkOrderAsync(wo.OrderId, dialog.HoldReason, dialog.Remark);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hold失败: {ex.Message}";
        }
    }

    private async void OnReleaseHold(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        try
        {
            await _dataService.ReleaseHoldWorkOrderAsync(wo.OrderId);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"解挂失败: {ex.Message}";
        }
    }

    private async void OnClose(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        try
        {
            await _dataService.UpdateWorkOrderStatusAsync(wo.OrderId, ProcessStatus.Completed);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"关闭失败: {ex.Message}";
        }
    }

    private async void OnCancel(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        try
        {
            await _dataService.UpdateWorkOrderStatusAsync(wo.OrderId, ProcessStatus.Cancelled);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"取消失败: {ex.Message}";
        }
    }

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
        FilterPriority = null;
        FilterDateFrom = null;
        FilterDateTo = null;
    }

    private void OnViewDetail(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        var parameters = new NavigationParameters { { "OrderId", wo.OrderId } };
        _regionManager.RequestNavigate("MainContentRegion", "WorkOrderDetailView", parameters);
    }

    private void RaiseCanExecuteChanged()
    {
        DeleteCommand.RaiseCanExecuteChanged();
        ReleaseCommand.RaiseCanExecuteChanged();
        HoldCommand.RaiseCanExecuteChanged();
        ReleaseHoldCommand.RaiseCanExecuteChanged();
        CloseCommand.RaiseCanExecuteChanged();
        CancelCommand.RaiseCanExecuteChanged();
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
