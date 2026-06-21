using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Modules.Order.Models;
using MES.Modules.Order.Services;
using MES.Domain.Production;

namespace MES.Modules.Order.ViewModels;

public class WorkOrderScheduleViewModel : BindableBase
{
    private readonly IProductionDataService _dataService;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<WorkOrderInfo> _workOrders = [];
    private WorkOrderInfo? _selectedWorkOrder;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private string? _filterStatus;
    private string? _filterPriority;

    public ObservableCollection<WorkOrderInfo> WorkOrders
    {
        get => _workOrders;
        set => SetProperty(ref _workOrders, value);
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

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                ApplyFilter();
        }
    }

    public string? FilterStatus
    {
        get => _filterStatus;
        set
        {
            if (SetProperty(ref _filterStatus, value))
                ApplyFilter();
        }
    }

    public string? FilterPriority
    {
        get => _filterPriority;
        set
        {
            if (SetProperty(ref _filterPriority, value))
                ApplyFilter();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public int TotalCount => WorkOrders.Count;
    public int ReleasedCount => WorkOrders.Count(w => w.Status == ProcessStatus.Released);
    public int InProgressCount => WorkOrders.Count(w => w.Status == ProcessStatus.InProgress);
    public int HoldCount => WorkOrders.Count(w => w.Status == ProcessStatus.Hold);
    public int OverdueCount => WorkOrders.Count(w => w.PlannedEndDate.HasValue && w.PlannedEndDate.Value < DateTime.Now && w.Status != ProcessStatus.Completed && w.Status != ProcessStatus.Closed && w.Status != ProcessStatus.Cancelled);

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<WorkOrderInfo?> StartCommand { get; }
    public DelegateCommand<WorkOrderInfo?> PauseCommand { get; }
    public DelegateCommand<WorkOrderInfo?> CompleteCommand { get; }
    public DelegateCommand<WorkOrderInfo?> AdjustPriorityCommand { get; }
    public DelegateCommand<WorkOrderInfo?> ViewDetailCommand { get; }

    public WorkOrderScheduleViewModel(IProductionDataService dataService, IRegionManager regionManager)
    {
        _dataService = dataService;
        _regionManager = regionManager;

        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        StartCommand = new DelegateCommand<WorkOrderInfo?>(OnStart, wo => wo?.Status == ProcessStatus.Released);
        PauseCommand = new DelegateCommand<WorkOrderInfo?>(OnPause, wo => wo?.Status == ProcessStatus.InProgress);
        CompleteCommand = new DelegateCommand<WorkOrderInfo?>(OnComplete, wo => wo?.Status == ProcessStatus.InProgress);
        AdjustPriorityCommand = new DelegateCommand<WorkOrderInfo?>(OnAdjustPriority, wo => wo != null);
        ViewDetailCommand = new DelegateCommand<WorkOrderInfo?>(OnViewDetail, wo => wo != null);

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
        WorkOrders = new ObservableCollection<WorkOrderInfo>(orders
            .Where(w => w.Status == ProcessStatus.Released || w.Status == ProcessStatus.InProgress || w.Status == ProcessStatus.Hold)
            .OrderBy(w => (int)w.Priority)
            .ThenBy(w => w.PlannedStartDate));
        UpdateStatistics();
    }

    private void ApplyFilter()
    {
        var filtered = GetFilteredWorkOrders();
        WorkOrders = new ObservableCollection<WorkOrderInfo>(filtered);
        UpdateStatistics();
    }

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
                || wo.CustomerName.ToLower().Contains(key));
        }

        if (!string.IsNullOrWhiteSpace(FilterStatus) && Enum.TryParse<ProcessStatus>(FilterStatus, out var statusFilter))
            query = query.Where(wo => wo.Status == statusFilter);

        if (!string.IsNullOrWhiteSpace(FilterPriority) && Enum.TryParse<WorkOrderPriority>(FilterPriority, out var priorityFilter))
            query = query.Where(wo => wo.Priority == priorityFilter);

        return query.OrderBy(w => (int)w.Priority)
                    .ThenBy(w => w.PlannedStartDate)
                    .ToList();
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
        FilterStatus = null;
        FilterPriority = null;
        _ = ReloadDataAsync();
    }

    private async void OnStart(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        try
        {
            await _dataService.UpdateWorkOrderStatusAsync(wo.OrderId, ProcessStatus.InProgress);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"开始工单失败: {ex.Message}";
        }
    }

    private async void OnPause(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        try
        {
            await _dataService.HoldWorkOrderAsync(wo.OrderId, "计划暂停", null);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"暂停工单失败: {ex.Message}";
        }
    }

    private async void OnComplete(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        try
        {
            await _dataService.UpdateWorkOrderStatusAsync(wo.OrderId, ProcessStatus.Completed);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"完成工单失败: {ex.Message}";
        }
    }

    private void OnAdjustPriority(WorkOrderInfo? wo)
    {
        if (wo == null) return;

        var result = System.Windows.MessageBox.Show(
            $"是否将工单 {wo.OrderId} 的优先级从 {wo.Priority} 提升为 {GetNextPriority(wo.Priority)}?",
            "调整优先级",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result == System.Windows.MessageBoxResult.Yes)
        {
            wo.Priority = GetNextPriority(wo.Priority);
            RaisePropertyChanged(nameof(WorkOrders));
        }
    }

    private void OnViewDetail(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        var parameters = new NavigationParameters { { "OrderId", wo.OrderId } };
        _regionManager.RequestNavigate("MainContentRegion", "WorkOrderDetailView", parameters);
    }

    private WorkOrderPriority GetNextPriority(WorkOrderPriority current)
    {
        return current switch
        {
            WorkOrderPriority.Normal => WorkOrderPriority.High,
            WorkOrderPriority.High => WorkOrderPriority.Urgent,
            _ => current,
        };
    }

    private void RaiseCanExecuteChanged()
    {
        StartCommand.RaiseCanExecuteChanged();
        PauseCommand.RaiseCanExecuteChanged();
        CompleteCommand.RaiseCanExecuteChanged();
        AdjustPriorityCommand.RaiseCanExecuteChanged();
        ViewDetailCommand.RaiseCanExecuteChanged();
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(ReleasedCount));
        RaisePropertyChanged(nameof(InProgressCount));
        RaisePropertyChanged(nameof(HoldCount));
        RaisePropertyChanged(nameof(OverdueCount));
    }
}
