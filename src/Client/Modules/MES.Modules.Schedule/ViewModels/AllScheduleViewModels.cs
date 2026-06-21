using Prism.Mvvm;
using Prism.Commands;
using MES.Modules.Schedule.Models;
using MES.Modules.Schedule.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace MES.Modules.Schedule.ViewModels;

public class DispatchBoardViewModel : BindableBase
{
    private readonly IScheduleService _service;
    private ObservableCollection<DispatchQueueItem> _items = [];
    private DispatchQueueItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;
    private string _equipmentFilter = string.Empty;
    private string _searchText = string.Empty;

    public ObservableCollection<DispatchQueueItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public DispatchQueueItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string EquipmentFilter { get => _equipmentFilter; set { SetProperty(ref _equipmentFilter, value); ApplyFilter(); } }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int TotalQueueCount => Items.Count;
    public int HighPriorityCount => Items.Count(x => x.Priority <= 2);
    public int OverdueCount => Items.Count(x => x.DueDate < DateTime.Now);

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }

    public DispatchBoardViewModel(IScheduleService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        ClearFilterCommand = new DelegateCommand(ClearFilter);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<DispatchQueueItem>(await _service.GetDispatchQueueAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Filter = FilterItems;
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private bool FilterItems(object obj)
    {
        if (obj is not DispatchQueueItem item) return false;
        var matchEquip = string.IsNullOrEmpty(EquipmentFilter) || item.EquipmentId.Contains(EquipmentFilter, StringComparison.OrdinalIgnoreCase);
        var matchSearch = string.IsNullOrEmpty(SearchText) ||
            item.LotId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Product.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Customer.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        return matchEquip && matchSearch;
    }

    private void ApplyFilter() => CollectionView?.Refresh();
    private void ClearFilter() { EquipmentFilter = string.Empty; SearchText = string.Empty; }
}

public class DispatchRuleConfigViewModel : BindableBase
{
    private readonly IScheduleService _service;
    private ObservableCollection<DispatchRuleItem> _items = [];
    private DispatchRuleItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;

    public ObservableCollection<DispatchRuleItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public DispatchRuleItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int EnabledRuleCount => Items.Count(x => x.IsEnabled);
    public int DisabledRuleCount => Items.Count(x => !x.IsEnabled);

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ToggleRuleCommand { get; }

    public DispatchRuleConfigViewModel(IScheduleService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        ToggleRuleCommand = new DelegateCommand(ToggleRule, CanToggleRule);
        _ = LoadDataAsync();
    }

    private bool CanToggleRule() => SelectedItem != null;

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<DispatchRuleItem>(await _service.GetDispatchRulesAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void ToggleRule()
    {
        if (SelectedItem != null)
        {
            SelectedItem.IsEnabled = !SelectedItem.IsEnabled;
        }
    }
}

public class CapacityAnalysisViewModel : BindableBase
{
    private readonly IScheduleService _service;
    private ObservableCollection<CapacityItem> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private double _avgUtilization;
    private ICollectionView _collectionView = null!;
    private string _typeFilter = string.Empty;

    public ObservableCollection<CapacityItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public double AvgUtilization { get => _avgUtilization; set => SetProperty(ref _avgUtilization, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string TypeFilter { get => _typeFilter; set { SetProperty(ref _typeFilter, value); ApplyFilter(); } }

    public int OverUtilizedCount => Items.Count(x => x.ActualUtilization > x.PlannedUtilization);
    public int UnderUtilizedCount => Items.Count(x => x.ActualUtilization < x.PlannedUtilization);
    public int TotalWip => Items.Sum(x => x.WipCount);

    public DelegateCommand RefreshCommand { get; }

    public CapacityAnalysisViewModel(IScheduleService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<CapacityItem>(await _service.GetCapacityAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Filter = FilterItems;
            CollectionView.Refresh();
            AvgUtilization = Items.Any() ? Items.Average(x => x.ActualUtilization) : 0;
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private bool FilterItems(object obj)
    {
        if (obj is not CapacityItem item) return false;
        return string.IsNullOrEmpty(TypeFilter) || item.Type.Contains(TypeFilter, StringComparison.OrdinalIgnoreCase);
    }

    private void ApplyFilter()
    {
        CollectionView?.Refresh();
        var filtered = Items.Where(x => string.IsNullOrEmpty(TypeFilter) || x.Type.Contains(TypeFilter, StringComparison.OrdinalIgnoreCase));
        AvgUtilization = filtered.Any() ? filtered.Average(x => x.ActualUtilization) : 0;
    }
}

public class WorkOrderScheduleViewModel : BindableBase
{
    private readonly IScheduleService _service;
    private ObservableCollection<WorkOrderScheduleItem> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _delayedCount;
    private ICollectionView _collectionView = null!;
    private string _statusFilter = string.Empty;

    public ObservableCollection<WorkOrderScheduleItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int DelayedCount { get => _delayedCount; set => SetProperty(ref _delayedCount, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string StatusFilter { get => _statusFilter; set { SetProperty(ref _statusFilter, value); ApplyFilter(); } }

    public int TotalOrders => Items.Count;
    public int CompletedOrders => Items.Count(x => x.Status == "Completed");
    public int InProgressOrders => Items.Count(x => x.Status == "InProgress");
    public double AvgProgress => Items.Any() ? Items.Average(x => x.ProgressPercent) : 0;

    public DelegateCommand RefreshCommand { get; }

    public WorkOrderScheduleViewModel(IScheduleService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<WorkOrderScheduleItem>(await _service.GetWorkOrderSchedulesAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Filter = FilterItems;
            CollectionView.Refresh();
            DelayedCount = Items.Count(x => x.Status == "Delayed");
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private bool FilterItems(object obj)
    {
        if (obj is not WorkOrderScheduleItem item) return false;
        return string.IsNullOrEmpty(StatusFilter) || item.Status == StatusFilter;
    }

    private void ApplyFilter()
    {
        CollectionView?.Refresh();
        DelayedCount = Items.Count(x => x.Status == "Delayed");
    }
}

public class MrpViewModel : BindableBase
{
    private readonly IScheduleService _service;
    private ObservableCollection<MrpItem> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _shortageCount;
    private ICollectionView _collectionView = null!;
    private bool _showShortageOnly;

    public ObservableCollection<MrpItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int ShortageCount { get => _shortageCount; set => SetProperty(ref _shortageCount, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public bool ShowShortageOnly { get => _showShortageOnly; set { SetProperty(ref _showShortageOnly, value); ApplyFilter(); } }

    public int TotalMaterials => Items.Count;
    public int TotalShortageQty => Items.Sum(x => x.ShortageQty);

    public DelegateCommand RefreshCommand { get; }

    public MrpViewModel(IScheduleService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<MrpItem>(await _service.GetMrpDataAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Filter = FilterItems;
            CollectionView.Refresh();
            ShortageCount = Items.Count(x => x.ShortageQty > 0);
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private bool FilterItems(object obj)
    {
        if (obj is not MrpItem item) return false;
        return !ShowShortageOnly || item.ShortageQty > 0;
    }

    private void ApplyFilter() => CollectionView?.Refresh();
}

public class DeliveryManageViewModel : BindableBase
{
    private readonly IScheduleService _service;
    private ObservableCollection<DeliveryRecord> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;
    private string _statusFilter = string.Empty;
    private string _customerFilter = string.Empty;

    public ObservableCollection<DeliveryRecord> Items { get => _items; set => SetProperty(ref _items, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string StatusFilter { get => _statusFilter; set { SetProperty(ref _statusFilter, value); ApplyFilter(); } }
    public string CustomerFilter { get => _customerFilter; set { SetProperty(ref _customerFilter, value); ApplyFilter(); } }

    public int TotalDeliveries => Items.Count;
    public int PendingCount => Items.Count(x => x.Status == "Pending");
    public int CompletedCount => Items.Count(x => x.Status == "Completed");
    public int TotalPlanQty => Items.Sum(x => x.PlanQty);
    public int TotalDeliverQty => Items.Sum(x => x.DeliverQty);

    public DelegateCommand RefreshCommand { get; }

    public DeliveryManageViewModel(IScheduleService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<DeliveryRecord>(await _service.GetDeliveryRecordsAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Filter = FilterItems;
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private bool FilterItems(object obj)
    {
        if (obj is not DeliveryRecord item) return false;
        var matchStatus = string.IsNullOrEmpty(StatusFilter) || item.Status == StatusFilter;
        var matchCustomer = string.IsNullOrEmpty(CustomerFilter) || item.CustomerName.Contains(CustomerFilter, StringComparison.OrdinalIgnoreCase);
        return matchStatus && matchCustomer;
    }

    private void ApplyFilter() => CollectionView?.Refresh();
}

public class CapacityBalanceViewModel : BindableBase
{
    private readonly IScheduleService _service;
    private ObservableCollection<CapacityBalanceItem> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;
    private bool _showOverloadedOnly;

    public ObservableCollection<CapacityBalanceItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public bool ShowOverloadedOnly { get => _showOverloadedOnly; set { SetProperty(ref _showOverloadedOnly, value); ApplyFilter(); } }

    public int OverloadedProcessCount => Items.Count(x => x.BalanceHours < 0);
    public int AvailableProcessCount => Items.Count(x => x.BalanceHours >= 0);
    public double TotalBalanceHours => Items.Sum(x => x.BalanceHours);
    public double AvgUtilization => Items.Any() ? Items.Average(x => x.Utilization) : 0;

    public DelegateCommand RefreshCommand { get; }

    public CapacityBalanceViewModel(IScheduleService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<CapacityBalanceItem>(await _service.GetCapacityBalanceAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Filter = FilterItems;
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private bool FilterItems(object obj)
    {
        if (obj is not CapacityBalanceItem item) return false;
        return !ShowOverloadedOnly || item.BalanceHours < 0;
    }

    private void ApplyFilter() => CollectionView?.Refresh();
}
