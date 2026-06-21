using Prism.Mvvm;
using Prism.Commands;
using MES.Modules.Equipment.Models;
using MES.Modules.Equipment.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace MES.Modules.Equipment.ViewModels;

public class EquipmentOverviewViewModel : BindableBase
{
    private readonly IEquipmentService _service;
    private ObservableCollection<EquipmentInfo> _items = [];
    private EquipmentInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string _statusFilter = "All";
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _runningCount;
    private int _idleCount;
    private int _maintenanceCount;
    private double _avgOee;

    public ObservableCollection<EquipmentInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public EquipmentInfo? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public string StatusFilter { get => _statusFilter; set { SetProperty(ref _statusFilter, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int RunningCount { get => _runningCount; set => SetProperty(ref _runningCount, value); }
    public int IdleCount { get => _idleCount; set => SetProperty(ref _idleCount, value); }
    public int MaintenanceCount { get => _maintenanceCount; set => SetProperty(ref _maintenanceCount, value); }
    public double AvgOee { get => _avgOee; set => SetProperty(ref _avgOee, value); }

    public DelegateCommand RefreshCommand { get; }
    public ICollectionView FilteredView { get; }

    public EquipmentOverviewViewModel(IEquipmentService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var data = await _service.GetAllEquipmentAsync();
            Items = new ObservableCollection<EquipmentInfo>(data);
            FilteredView.Refresh();
            UpdateStats();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void ApplyFilter() => FilteredView.Refresh();

    private bool FilterPredicate(object obj)
    {
        if (obj is not EquipmentInfo item) return false;
        var matchSearch = string.IsNullOrWhiteSpace(SearchText) ||
            item.EquipmentId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var matchStatus = StatusFilter == "All" || item.Status == StatusFilter;
        return matchSearch && matchStatus;
    }

    private void UpdateStats()
    {
        RunningCount = Items.Count(x => x.Status == "Running");
        IdleCount = Items.Count(x => x.Status == "Idle");
        MaintenanceCount = Items.Count(x => x.Status == "Maintenance");
        AvgOee = Items.Any() ? Items.Average(x => x.OEE) : 0;
    }
}

public class EquipmentDetailViewModel : BindableBase
{
    private readonly IEquipmentService _service;
    private ObservableCollection<EquipmentHistoryRecord> _historyItems = [];
    private string _selectedEquipmentId = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private EquipmentInfo? _equipmentInfo;

    public ObservableCollection<EquipmentHistoryRecord> HistoryItems { get => _historyItems; set => SetProperty(ref _historyItems, value); }
    public string SelectedEquipmentId { get => _selectedEquipmentId; set { SetProperty(ref _selectedEquipmentId, value); _ = LoadDataAsync(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public EquipmentInfo? EquipmentInfo { get => _equipmentInfo; set => SetProperty(ref _equipmentInfo, value); }

    public DelegateCommand RefreshCommand { get; }

    public EquipmentDetailViewModel(IEquipmentService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
    }

    private async Task LoadDataAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedEquipmentId)) return;
        try
        {
            IsLoading = true;
            EquipmentInfo = await _service.GetEquipmentAsync(SelectedEquipmentId);
            var history = await _service.GetEquipmentHistoryAsync(SelectedEquipmentId);
            HistoryItems = new ObservableCollection<EquipmentHistoryRecord>(history);
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}

public class EapControlViewModel : BindableBase
{
    private readonly IEquipmentService _service;
    private ObservableCollection<EquipmentInfo> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<EquipmentInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public DelegateCommand RefreshCommand { get; }

    public EapControlViewModel(IEquipmentService service)
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
            var data = await _service.GetAllEquipmentAsync();
            Items = new ObservableCollection<EquipmentInfo>(data);
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}

public class PmScheduleViewModel : BindableBase
{
    private readonly IEquipmentService _service;
    private ObservableCollection<PmScheduleItem> _items = [];
    private PmScheduleItem? _selectedItem;
    private string _statusFilter = "All";
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _pendingCount;
    private int _completedCount;
    private int _inProgressCount;

    public ObservableCollection<PmScheduleItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public PmScheduleItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string StatusFilter { get => _statusFilter; set { SetProperty(ref _statusFilter, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int PendingCount { get => _pendingCount; set => SetProperty(ref _pendingCount, value); }
    public int CompletedCount { get => _completedCount; set => SetProperty(ref _completedCount, value); }
    public int InProgressCount { get => _inProgressCount; set => SetProperty(ref _inProgressCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public ICollectionView FilteredView { get; }

    public PmScheduleViewModel(IEquipmentService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), () => SelectedItem is not null);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var data = await _service.GetPmSchedulesAsync();
            Items = new ObservableCollection<PmScheduleItem>(data);
            FilteredView.Refresh();
            UpdateStats();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync()
    {
        if (SelectedItem is null) return;
        try { await _service.SavePmScheduleAsync(SelectedItem); await LoadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; }
    }

    private void ApplyFilter() => FilteredView.Refresh();
    private bool FilterPredicate(object obj) => obj is PmScheduleItem item && (StatusFilter == "All" || item.Status == StatusFilter);

    private void UpdateStats()
    {
        PendingCount = Items.Count(x => x.Status == "Pending");
        CompletedCount = Items.Count(x => x.Status == "Completed");
        InProgressCount = Items.Count(x => x.Status == "InProgress");
    }
}

public class EquipmentAlarmViewModel : BindableBase
{
    private readonly IEquipmentService _service;
    private ObservableCollection<EquipmentAlarmRecord> _items = [];
    private EquipmentAlarmRecord? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<EquipmentAlarmRecord> Items { get => _items; set => SetProperty(ref _items, value); }
    public EquipmentAlarmRecord? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public DelegateCommand RefreshCommand { get; }

    public EquipmentAlarmViewModel(IEquipmentService service)
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
            var all = await _service.GetAllEquipmentAsync();
            var alarms = new List<EquipmentAlarmRecord>();
            foreach (var eq in all)
                alarms.AddRange(await _service.GetEquipmentAlarmsAsync(eq.EquipmentId));
            Items = new ObservableCollection<EquipmentAlarmRecord>(alarms);
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}

public class SparePartViewModel : BindableBase
{
    private readonly IEquipmentService _service;
    private ObservableCollection<SparePartItem> _items = [];
    private SparePartItem? _selectedItem;
    private string _searchText = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _lowStockCount;

    public ObservableCollection<SparePartItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public SparePartItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int LowStockCount { get => _lowStockCount; set => SetProperty(ref _lowStockCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public ICollectionView FilteredView { get; }

    public SparePartViewModel(IEquipmentService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), () => SelectedItem is not null);
        DeleteCommand = new DelegateCommand(async () => await DeleteAsync(), () => SelectedItem is not null);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var data = await _service.GetSparePartsAsync();
            Items = new ObservableCollection<SparePartItem>(data);
            FilteredView.Refresh();
            UpdateStats();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync()
    {
        if (SelectedItem is null) return;
        try { await _service.SaveSparePartAsync(SelectedItem); await LoadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; }
    }

    private async Task DeleteAsync()
    {
        if (SelectedItem is null) return;
        try { await _service.DeleteSparePartAsync(SelectedItem.Id); Items.Remove(SelectedItem); SelectedItem = null; }
        catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; }
    }

    private void ApplyFilter() => FilteredView.Refresh();
    private bool FilterPredicate(object obj) => obj is not SparePartItem item || string.IsNullOrWhiteSpace(SearchText) ||
        item.PartName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || item.PartNo.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

    private void UpdateStats() { LowStockCount = Items.Count(x => x.StockQty <= x.MinQty); }
}

public class EquipmentPerformanceViewModel : BindableBase
{
    private readonly IEquipmentService _service;
    private ObservableCollection<EquipmentPerformanceItem> _items = [];
    private EquipmentPerformanceItem? _selectedItem;
    private string _searchText = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private double _avgOee;
    private double _avgAvailability;

    public ObservableCollection<EquipmentPerformanceItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public EquipmentPerformanceItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public double AvgOee { get => _avgOee; set => SetProperty(ref _avgOee, value); }
    public double AvgAvailability { get => _avgAvailability; set => SetProperty(ref _avgAvailability, value); }

    public DelegateCommand RefreshCommand { get; }
    public ICollectionView FilteredView { get; }

    public EquipmentPerformanceViewModel(IEquipmentService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var data = await _service.GetPerformanceAsync();
            Items = new ObservableCollection<EquipmentPerformanceItem>(data);
            FilteredView.Refresh();
            UpdateStats();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void ApplyFilter() => FilteredView.Refresh();
    private bool FilterPredicate(object obj) => obj is not EquipmentPerformanceItem item || string.IsNullOrWhiteSpace(SearchText) ||
        item.EquipmentId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || item.EquipmentName.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

    private void UpdateStats()
    {
        AvgOee = Items.Any() ? Items.Average(x => x.OEE) : 0;
        AvgAvailability = Items.Any() ? Items.Average(x => x.Availability) : 0;
    }
}

public class EquipmentHistoryViewModel : BindableBase
{
    private readonly IEquipmentService _service;
    private ObservableCollection<EquipmentHistoryRecord> _items = [];
    private string _selectedEquipmentId = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<EquipmentHistoryRecord> Items { get => _items; set => SetProperty(ref _items, value); }
    public string SelectedEquipmentId { get => _selectedEquipmentId; set { SetProperty(ref _selectedEquipmentId, value); _ = LoadHistoryAsync(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public DelegateCommand RefreshCommand { get; }

    public EquipmentHistoryViewModel(IEquipmentService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadHistoryAsync());
    }

    private async Task LoadHistoryAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedEquipmentId)) return;
        try
        {
            IsLoading = true;
            var data = await _service.GetEquipmentHistoryAsync(SelectedEquipmentId);
            Items = new ObservableCollection<EquipmentHistoryRecord>(data);
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}

public class FixtureViewModel : BindableBase
{
    private readonly IEquipmentService _service;
    private ObservableCollection<FixtureItem> _items = [];
    private FixtureItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _nearExpiryCount;

    public ObservableCollection<FixtureItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public FixtureItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int NearExpiryCount { get => _nearExpiryCount; set => SetProperty(ref _nearExpiryCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }

    public FixtureViewModel(IEquipmentService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), () => SelectedItem is not null);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var data = await _service.GetFixturesAsync();
            Items = new ObservableCollection<FixtureItem>(data);
            UpdateStats();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync()
    {
        if (SelectedItem is null) return;
        try { await _service.SaveFixtureAsync(SelectedItem); await LoadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; }
    }

    private void UpdateStats() { NearExpiryCount = Items.Count(x => x.Status == "NearExpiry"); }
}
