using Prism.Mvvm;
using Prism.Commands;
using MES.Modules.Quality.Models;
using MES.Modules.Quality.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace MES.Modules.Quality.ViewModels;

public class SpcChartViewModel : BindableBase
{
    private readonly IQualityService _service;
    private ObservableCollection<SpcChartItem> _items = [];
    private SpcChartItem? _selectedItem;
    private string _searchText = string.Empty;
    private string _statusFilter = "All";
    private string _errorMessage = string.Empty;
    private int _totalCount;
    private int _inControlCount;
    private int _warningCount;
    private int _outOfControlCount;
    private bool _isLoading;

    public ObservableCollection<SpcChartItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public SpcChartItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public string StatusFilter { get => _statusFilter; set { SetProperty(ref _statusFilter, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int InControlCount { get => _inControlCount; set => SetProperty(ref _inControlCount, value); }
    public int WarningCount { get => _warningCount; set => SetProperty(ref _warningCount, value); }
    public int OutOfControlCount { get => _outOfControlCount; set => SetProperty(ref _outOfControlCount, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SearchCommand { get; }

    public ICollectionView FilteredView { get; }

    public SpcChartViewModel(IQualityService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SearchCommand = new DelegateCommand(() => ApplyFilter());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var data = await _service.GetSpcChartsAsync();
            Items = new ObservableCollection<SpcChartItem>(data);
            FilteredView.Refresh();
            UpdateStats();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyFilter() => FilteredView.Refresh();

    private bool FilterPredicate(object obj)
    {
        if (obj is not SpcChartItem item) return false;
        var matchSearch = string.IsNullOrWhiteSpace(SearchText) ||
            item.EquipmentId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Parameter.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var matchStatus = StatusFilter == "All" || item.Status == StatusFilter;
        return matchSearch && matchStatus;
    }

    private void UpdateStats()
    {
        TotalCount = Items.Count;
        InControlCount = Items.Count(x => x.Status == "InControl");
        WarningCount = Items.Count(x => x.Status == "Warning");
        OutOfControlCount = Items.Count(x => x.Status == "OutControl");
    }
}

public class SpcRuleConfigViewModel : BindableBase
{
    private readonly IQualityService _service;
    private ObservableCollection<SpcRuleItem> _items = [];
    private SpcRuleItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<SpcRuleItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public SpcRuleItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand ToggleStatusCommand { get; }

    public SpcRuleConfigViewModel(IQualityService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), CanSave);
        DeleteCommand = new DelegateCommand(async () => await DeleteAsync(), CanDelete);
        ToggleStatusCommand = new DelegateCommand(async () => await ToggleStatusAsync(), CanToggleStatus);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var data = await _service.GetSpcRulesAsync();
            Items = new ObservableCollection<SpcRuleItem>(data);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync()
    {
        if (SelectedItem is null) return;
        try { await _service.SaveSpcRuleAsync(SelectedItem); await LoadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; }
    }

    private bool CanSave() => SelectedItem is not null;

    private async Task DeleteAsync()
    {
        if (SelectedItem is null) return;
        try { await _service.DeleteSpcRuleAsync(SelectedItem.Id); Items.Remove(SelectedItem); SelectedItem = null; }
        catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; }
    }

    private bool CanDelete() => SelectedItem is not null;

    private async Task ToggleStatusAsync()
    {
        if (SelectedItem is null) return;
        SelectedItem.IsEnabled = !SelectedItem.IsEnabled;
        await SaveAsync();
    }

    private bool CanToggleStatus() => SelectedItem is not null;
}

public class OocEventViewModel : BindableBase
{
    private readonly IQualityService _service;
    private ObservableCollection<OocEventItem> _items = [];
    private OocEventItem? _selectedItem;
    private string _searchText = string.Empty;
    private string _statusFilter = "All";
    private string _errorMessage = string.Empty;
    private int _pendingCount;
    private int _investigatingCount;
    private int _closedCount;
    private bool _isLoading;

    public ObservableCollection<OocEventItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public OocEventItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public string StatusFilter { get => _statusFilter; set { SetProperty(ref _statusFilter, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public int PendingCount { get => _pendingCount; set => SetProperty(ref _pendingCount, value); }
    public int InvestigatingCount { get => _investigatingCount; set => SetProperty(ref _investigatingCount, value); }
    public int ClosedCount { get => _closedCount; set => SetProperty(ref _closedCount, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand<string> UpdateStatusCommand { get; }

    public ICollectionView FilteredView { get; }

    public OocEventViewModel(IQualityService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        UpdateStatusCommand = new DelegateCommand<string>(async status => await UpdateStatusAsync(status), _ => SelectedItem is not null);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var data = await _service.GetOocEventsAsync();
            Items = new ObservableCollection<OocEventItem>(data);
            FilteredView.Refresh();
            UpdateStats();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task UpdateStatusAsync(string status)
    {
        if (SelectedItem is null) return;
        try
        {
            await _service.UpdateOocEventStatusAsync(SelectedItem.Id, status);
            await LoadDataAsync();
        }
        catch (Exception ex) { ErrorMessage = $"更新失败: {ex.Message}"; }
    }

    private void ApplyFilter() => FilteredView.Refresh();

    private bool FilterPredicate(object obj)
    {
        if (obj is not OocEventItem item) return false;
        var matchSearch = string.IsNullOrWhiteSpace(SearchText) ||
            item.EquipmentId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Parameter.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var matchStatus = StatusFilter == "All" || item.Status == StatusFilter;
        return matchSearch && matchStatus;
    }

    private void UpdateStats()
    {
        PendingCount = Items.Count(x => x.Status == "Pending");
        InvestigatingCount = Items.Count(x => x.Status == "Investigating");
        ClosedCount = Items.Count(x => x.Status == "Closed");
    }
}

public class FdcMonitorViewModel : BindableBase
{
    private readonly IQualityService _service;
    private ObservableCollection<FdcMonitorItem> _items = [];
    private FdcMonitorItem? _selectedItem;
    private string _searchText = string.Empty;
    private string _statusFilter = "All";
    private string _errorMessage = string.Empty;
    private int _normalCount;
    private int _warningCount;
    private int _alarmCount;
    private bool _isLoading;

    public ObservableCollection<FdcMonitorItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public FdcMonitorItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public string StatusFilter { get => _statusFilter; set { SetProperty(ref _statusFilter, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public int NormalCount { get => _normalCount; set => SetProperty(ref _normalCount, value); }
    public int WarningCount { get => _warningCount; set => SetProperty(ref _warningCount, value); }
    public int AlarmCount { get => _alarmCount; set => SetProperty(ref _alarmCount, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public DelegateCommand RefreshCommand { get; }

    public ICollectionView FilteredView { get; }

    public FdcMonitorViewModel(IQualityService service)
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
            var data = await _service.GetFdcMonitorsAsync();
            Items = new ObservableCollection<FdcMonitorItem>(data);
            FilteredView.Refresh();
            UpdateStats();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void ApplyFilter() => FilteredView.Refresh();

    private bool FilterPredicate(object obj)
    {
        if (obj is not FdcMonitorItem item) return false;
        var matchSearch = string.IsNullOrWhiteSpace(SearchText) ||
            item.EquipmentId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Chamber.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var matchStatus = StatusFilter == "All" || item.Status == StatusFilter;
        return matchSearch && matchStatus;
    }

    private void UpdateStats()
    {
        NormalCount = Items.Count(x => x.Status == "Normal");
        WarningCount = Items.Count(x => x.Status == "Warning");
        AlarmCount = Items.Count(x => x.Status == "Alarm");
    }
}

public class InspectionViewModel : BindableBase
{
    private readonly IQualityService _service;
    private ObservableCollection<InspectionItem> _items = [];
    private InspectionItem? _selectedItem;
    private string _searchText = string.Empty;
    private string _typeFilter = "All";
    private string _resultFilter = "All";
    private string _errorMessage = string.Empty;
    private int _passCount;
    private int _failCount;
    private bool _isLoading;

    public ObservableCollection<InspectionItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public InspectionItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public string TypeFilter { get => _typeFilter; set { SetProperty(ref _typeFilter, value); ApplyFilter(); } }
    public string ResultFilter { get => _resultFilter; set { SetProperty(ref _resultFilter, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public int PassCount { get => _passCount; set => SetProperty(ref _passCount, value); }
    public int FailCount { get => _failCount; set => SetProperty(ref _failCount, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand NewCommand { get; }

    public ICollectionView FilteredView { get; }

    public InspectionViewModel(IQualityService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveAsync());
        DeleteCommand = new DelegateCommand(async () => await DeleteAsync(), () => SelectedItem is not null);
        NewCommand = new DelegateCommand(() => SelectedItem = new InspectionItem { InspectTime = DateTime.Now });
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var data = await _service.GetInspectionsAsync();
            Items = new ObservableCollection<InspectionItem>(data);
            FilteredView.Refresh();
            UpdateStats();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync()
    {
        if (SelectedItem is null) return;
        try { await _service.SaveInspectionAsync(SelectedItem); await LoadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; }
    }

    private async Task DeleteAsync()
    {
        if (SelectedItem is null) return;
        try { await _service.DeleteInspectionAsync(SelectedItem.Id); Items.Remove(SelectedItem); SelectedItem = null; }
        catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; }
    }

    private void ApplyFilter() => FilteredView.Refresh();

    private bool FilterPredicate(object obj)
    {
        if (obj is not InspectionItem item) return false;
        var matchSearch = string.IsNullOrWhiteSpace(SearchText) ||
            item.LotId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Product.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var matchType = TypeFilter == "All" || item.InspectionType == TypeFilter;
        var matchResult = ResultFilter == "All" || item.Result == ResultFilter;
        return matchSearch && matchType && matchResult;
    }

    private void UpdateStats()
    {
        PassCount = Items.Count(x => x.Result == "Pass");
        FailCount = Items.Count(x => x.Result == "Fail");
    }
}
