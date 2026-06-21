using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Modules.Order.Models;
using MES.Modules.Order.Services;
using MES.Domain.Production;

namespace MES.Modules.Order.ViewModels;

public class WorkOrderCloseViewModel : BindableBase
{
    private readonly IProductionDataService _dataService;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<WorkOrderInfo> _closableOrders = [];
    private WorkOrderInfo? _selectedOrder;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private string _closeReason = string.Empty;

    public ObservableCollection<WorkOrderInfo> ClosableOrders
    {
        get => _closableOrders;
        set => SetProperty(ref _closableOrders, value);
    }

    public WorkOrderInfo? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            if (SetProperty(ref _selectedOrder, value))
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

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public string CloseReason
    {
        get => _closeReason;
        set => SetProperty(ref _closeReason, value);
    }

    public int TotalCount => ClosableOrders.Count;
    public int ReadyToCloseCount => ClosableOrders.Count(w => w.ProgressPercent >= 100);
    public int PartialCloseCount => ClosableOrders.Count(w => w.ProgressPercent > 0 && w.ProgressPercent < 100);
    public int PendingCloseCount => ClosableOrders.Count(w => w.ProgressPercent == 0);

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand<WorkOrderInfo?> CloseOrderCommand { get; }
    public DelegateCommand<WorkOrderInfo?> ForceCloseCommand { get; }
    public DelegateCommand<WorkOrderInfo?> ViewDetailCommand { get; }

    public WorkOrderCloseViewModel(IProductionDataService dataService, IRegionManager regionManager)
    {
        _dataService = dataService;
        _regionManager = regionManager;

        RefreshCommand = new DelegateCommand(OnRefresh);
        CloseOrderCommand = new DelegateCommand<WorkOrderInfo?>(OnCloseOrder, wo => wo != null);
        ForceCloseCommand = new DelegateCommand<WorkOrderInfo?>(OnForceClose, wo => wo != null);
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
        ClosableOrders = new ObservableCollection<WorkOrderInfo>(orders
            .Where(w => w.Status == ProcessStatus.InProgress || w.Status == ProcessStatus.Released || w.Status == ProcessStatus.Hold)
            .OrderBy(w => w.PlannedEndDate)
            .ThenBy(w => w.OrderId));
        UpdateStatistics();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            _ = ReloadDataAsync();
            return;
        }

        var key = SearchText.Trim().ToLower();
        var filtered = ClosableOrders.Where(wo =>
            wo.OrderId.ToLower().Contains(key)
            || wo.ProductName.ToLower().Contains(key)
            || wo.RouteName.ToLower().Contains(key)
            || wo.CustomerName.ToLower().Contains(key)).ToList();

        ClosableOrders = new ObservableCollection<WorkOrderInfo>(filtered);
        UpdateStatistics();
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

    private async void OnCloseOrder(WorkOrderInfo? wo)
    {
        if (wo == null) return;

        if (wo.ProgressPercent < 100)
        {
            var result = System.Windows.MessageBox.Show(
                $"工单 {wo.OrderId} 进度为 {wo.ProgressPercent}%，尚未完成。\n\n是否继续关闭?",
                "确认关闭未完成工单",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result != System.Windows.MessageBoxResult.Yes)
                return;
        }

        try
        {
            await _dataService.UpdateWorkOrderStatusAsync(wo.OrderId, ProcessStatus.Completed);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"关闭工单失败: {ex.Message}";
        }
    }

    private async void OnForceClose(WorkOrderInfo? wo)
    {
        if (wo == null) return;

        var confirm = System.Windows.MessageBox.Show(
            $"强制关闭工单 {wo.OrderId}?\n\n" +
            $"当前状态: {wo.Status}\n" +
            $"当前进度: {wo.ProgressPercent}%\n" +
            $"计划数量: {wo.PlannedQty}\n" +
            $"完成数量: {wo.CompletedQty}\n\n" +
            "强制关闭后将无法恢复，是否继续?",
            "强制关闭确认",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Error);

        if (confirm != System.Windows.MessageBoxResult.Yes)
            return;

        try
        {
            await _dataService.UpdateWorkOrderStatusAsync(wo.OrderId, ProcessStatus.Closed);
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"强制关闭失败: {ex.Message}";
        }
    }

    private void OnViewDetail(WorkOrderInfo? wo)
    {
        if (wo == null) return;
        var parameters = new NavigationParameters { { "OrderId", wo.OrderId } };
        _regionManager.RequestNavigate("MainContentRegion", "WorkOrderDetailView", parameters);
    }

    private void RaiseCanExecuteChanged()
    {
        CloseOrderCommand.RaiseCanExecuteChanged();
        ForceCloseCommand.RaiseCanExecuteChanged();
        ViewDetailCommand.RaiseCanExecuteChanged();
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(ReadyToCloseCount));
        RaisePropertyChanged(nameof(PartialCloseCount));
        RaisePropertyChanged(nameof(PendingCloseCount));
    }
}
