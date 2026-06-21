using Prism.Mvvm;
using Prism.Commands;
using MES.Modules.Warehouse.Models;
using MES.Modules.Warehouse.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace MES.Modules.Warehouse.ViewModels;

public class FoupManagementViewModel : BindableBase
{
    private readonly IWarehouseService _service;
    private ObservableCollection<FoupItem> _items = [];
    private FoupItem? _selectedItem;
    private string _statusFilter = "All";
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<FoupItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public FoupItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string StatusFilter { get => _statusFilter; set { SetProperty(ref _statusFilter, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public ICollectionView FilteredView { get; }

    public FoupManagementViewModel(IWarehouseService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = obj => obj is FoupItem item && (StatusFilter == "All" || item.Status == StatusFilter);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), () => SelectedItem is not null);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try { IsLoading = true; Items = new ObservableCollection<FoupItem>(await _service.GetFoupsAsync()); FilteredView.Refresh(); }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync() { if (SelectedItem is null) return; try { await _service.SaveFoupAsync(SelectedItem); await LoadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void ApplyFilter() => FilteredView.Refresh();
}

public class MaterialListViewModel : BindableBase
{
    private readonly IWarehouseService _service;
    private ObservableCollection<MaterialItem> _items = [];
    private MaterialItem? _selectedItem;
    private string _searchText = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _lowStockCount;

    public ObservableCollection<MaterialItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public MaterialItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int LowStockCount { get => _lowStockCount; set => SetProperty(ref _lowStockCount, value); }
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public ICollectionView FilteredView { get; }

    public MaterialListViewModel(IWarehouseService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = obj => obj is not MaterialItem item || string.IsNullOrWhiteSpace(SearchText) ||
            item.MaterialName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || item.MaterialNo.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), () => SelectedItem is not null);
        DeleteCommand = new DelegateCommand(async () => await DeleteAsync(), () => SelectedItem is not null);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try { IsLoading = true; Items = new ObservableCollection<MaterialItem>(await _service.GetMaterialsAsync()); FilteredView.Refresh(); UpdateStats(); }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync() { if (SelectedItem is null) return; try { await _service.SaveMaterialAsync(SelectedItem); await LoadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private async Task DeleteAsync() { if (SelectedItem is null) return; try { await _service.DeleteMaterialAsync(SelectedItem.Id); Items.Remove(SelectedItem); SelectedItem = null; } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private void ApplyFilter() => FilteredView.Refresh();
    private void UpdateStats() { LowStockCount = Items.Count(x => x.StockQty <= x.MinQty); }
}

public class ReticleManagementViewModel : BindableBase
{
    private readonly IWarehouseService _service;
    private ObservableCollection<ReticleItem> _items = [];
    private ReticleItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _nearExpiryCount;

    public ObservableCollection<ReticleItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public ReticleItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int NearExpiryCount { get => _nearExpiryCount; set => SetProperty(ref _nearExpiryCount, value); }
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }

    public ReticleManagementViewModel(IWarehouseService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), () => SelectedItem is not null);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try { IsLoading = true; Items = new ObservableCollection<ReticleItem>(await _service.GetReticlesAsync()); UpdateStats(); }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync() { if (SelectedItem is null) return; try { await _service.SaveReticleAsync(SelectedItem); await LoadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void UpdateStats() { NearExpiryCount = Items.Count(x => x.UseCount >= x.MaxUseCount * 0.8); }
}

public class StockerViewModel : BindableBase
{
    private readonly IWarehouseService _service;
    private ObservableCollection<StockerItem> _items = [];
    private StockerItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<StockerItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public StockerItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }

    public StockerViewModel(IWarehouseService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), () => SelectedItem is not null);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try { IsLoading = true; Items = new ObservableCollection<StockerItem>(await _service.GetStockersAsync()); }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync() { if (SelectedItem is null) return; try { await _service.SaveStockerAsync(SelectedItem); await LoadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
}

public class InboundManageViewModel : BindableBase
{
    private readonly IWarehouseService _service;
    private ObservableCollection<InboundRecord> _items = [];
    private InboundRecord? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<InboundRecord> Items { get => _items; set => SetProperty(ref _items, value); }
    public InboundRecord? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }

    public InboundManageViewModel(IWarehouseService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), () => SelectedItem is not null);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try { IsLoading = true; Items = new ObservableCollection<InboundRecord>(await _service.GetInboundRecordsAsync()); }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync() { if (SelectedItem is null) return; try { await _service.SaveInboundRecordAsync(SelectedItem); await LoadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
}

public class OutboundManageViewModel : BindableBase
{
    private readonly IWarehouseService _service;
    private ObservableCollection<OutboundRecord> _items = [];
    private OutboundRecord? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<OutboundRecord> Items { get => _items; set => SetProperty(ref _items, value); }
    public OutboundRecord? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }

    public OutboundManageViewModel(IWarehouseService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), () => SelectedItem is not null);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try { IsLoading = true; Items = new ObservableCollection<OutboundRecord>(await _service.GetOutboundRecordsAsync()); }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task SaveAsync() { if (SelectedItem is null) return; try { await _service.SaveOutboundRecordAsync(SelectedItem); await LoadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
}

public class InventoryManageViewModel : BindableBase
{
    private readonly IWarehouseService _service;
    private ObservableCollection<InventoryRecord> _items = [];
    private InventoryRecord? _selectedItem;
    private string _searchText = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private double _totalValue;

    public ObservableCollection<InventoryRecord> Items { get => _items; set => SetProperty(ref _items, value); }
    public InventoryRecord? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public double TotalValue { get => _totalValue; set => SetProperty(ref _totalValue, value); }
    public DelegateCommand RefreshCommand { get; }
    public ICollectionView FilteredView { get; }

    public InventoryManageViewModel(IWarehouseService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = obj => obj is not InventoryRecord item || string.IsNullOrWhiteSpace(SearchText) ||
            item.MaterialName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || item.MaterialNo.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try { IsLoading = true; Items = new ObservableCollection<InventoryRecord>(await _service.GetInventoryRecordsAsync()); FilteredView.Refresh(); TotalValue = Items.Sum(x => x.TotalValue); }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void ApplyFilter() => FilteredView.Refresh();
}

public class ExpiryAlertViewModel : BindableBase
{
    private readonly IWarehouseService _service;
    private ObservableCollection<MaterialItem> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<MaterialItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public DelegateCommand RefreshCommand { get; }

    public ExpiryAlertViewModel(IWarehouseService service)
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
            var all = await _service.GetMaterialsAsync();
            Items = new ObservableCollection<MaterialItem>(all.Where(m => m.ExpiryDate.HasValue && m.ExpiryDate.Value <= DateTime.Now.AddDays(30)).OrderBy(m => m.ExpiryDate));
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}
