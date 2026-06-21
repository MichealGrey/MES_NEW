using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using MES.Modules.MasterData.Services;

namespace MES.Modules.MasterData.ViewModels;

public class RouteManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<RouteInfo> _items = [];
    private RouteInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private RouteInfo _editingItem = new();

    public ObservableCollection<RouteInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public RouteInfo? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) { if (_selectedItem != null) EditingItem = new RouteInfo { RouteId = _selectedItem.RouteId, RouteName = _selectedItem.RouteName, ProductId = _selectedItem.ProductId, Version = _selectedItem.Version, Status = _selectedItem.Status, Description = _selectedItem.Description }; } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public RouteInfo EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public int TotalCount => Items.Count;
    public int ActiveCount => Items.Count(i => i.Status == "Active");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<RouteInfo?> DeleteCommand { get; }

    public RouteManagementViewModel(IMasterDataService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingItem.RouteId));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<RouteInfo?>(OnDelete, i => i != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllRoutesAsync(); Items = new ObservableCollection<RouteInfo>(items); UpdateStatistics(); }
    private void ApplyFilter() { if (string.IsNullOrWhiteSpace(SearchText)) { _ = ReloadDataAsync(); return; } var key = SearchText.Trim().ToLower(); Items = new ObservableCollection<RouteInfo>(Items.Where(i => i.RouteId.ToLower().Contains(key) || i.RouteName.ToLower().Contains(key)).ToList()); }
    private void OnAdd() { EditingItem = new RouteInfo { Status = "Active" }; IsEditing = true; SaveCommand.RaiseCanExecuteChanged(); }
    private async void OnSave() { try { await _service.SaveRouteAsync(EditingItem); IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void OnCancel() { IsEditing = false; EditingItem = new RouteInfo(); }
    private async void OnDelete(RouteInfo? item) { if (item == null) return; try { if (System.Windows.MessageBox.Show($"确定删除?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return; await _service.DeleteRouteAsync(item.RouteId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(ActiveCount)); }
}

public class RecipeManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<RecipeInfo> _items = [];
    private RecipeInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private RecipeInfo _editingItem = new();

    public ObservableCollection<RecipeInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public RecipeInfo? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) { if (_selectedItem != null) EditingItem = new RecipeInfo { RecipeId = _selectedItem.RecipeId, RecipeName = _selectedItem.RecipeName, EquipmentId = _selectedItem.EquipmentId, StepCode = _selectedItem.StepCode, Status = _selectedItem.Status, IsActive = _selectedItem.IsActive, Parameters = _selectedItem.Parameters }; } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public RecipeInfo EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public int TotalCount => Items.Count;
    public int ActiveCount => Items.Count(i => i.Status == "Active");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<RecipeInfo?> DeleteCommand { get; }

    public RecipeManagementViewModel(IMasterDataService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingItem.RecipeId));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<RecipeInfo?>(OnDelete, i => i != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllRecipesAsync(); Items = new ObservableCollection<RecipeInfo>(items); UpdateStatistics(); }
    private void ApplyFilter() { if (string.IsNullOrWhiteSpace(SearchText)) { _ = ReloadDataAsync(); return; } var key = SearchText.Trim().ToLower(); Items = new ObservableCollection<RecipeInfo>(Items.Where(i => i.RecipeId.ToLower().Contains(key) || i.RecipeName.ToLower().Contains(key)).ToList()); }
    private void OnAdd() { EditingItem = new RecipeInfo { Status = "Active", IsActive = true }; IsEditing = true; SaveCommand.RaiseCanExecuteChanged(); }
    private async void OnSave() { try { await _service.SaveRecipeAsync(EditingItem); IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void OnCancel() { IsEditing = false; EditingItem = new RecipeInfo(); }
    private async void OnDelete(RecipeInfo? item) { if (item == null) return; try { if (System.Windows.MessageBox.Show($"确定删除?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return; await _service.DeleteRecipeAsync(item.RecipeId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(ActiveCount)); }
}

public class EquipmentManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<EquipmentInfo> _items = [];
    private EquipmentInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private EquipmentInfo _editingItem = new();

    public ObservableCollection<EquipmentInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public EquipmentInfo? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) { if (_selectedItem != null) EditingItem = new EquipmentInfo { EquipmentId = _selectedItem.EquipmentId, EquipmentName = _selectedItem.EquipmentName, EquipmentType = _selectedItem.EquipmentType, EquipmentGroup = _selectedItem.EquipmentGroup, Status = _selectedItem.Status, Location = _selectedItem.Location, Description = _selectedItem.Description }; } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public EquipmentInfo EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public int TotalCount => Items.Count;
    public int AvailableCount => Items.Count(i => i.Status == "Available");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<EquipmentInfo?> DeleteCommand { get; }

    public EquipmentManagementViewModel(IMasterDataService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingItem.EquipmentId));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<EquipmentInfo?>(OnDelete, i => i != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllEquipmentsAsync(); Items = new ObservableCollection<EquipmentInfo>(items); UpdateStatistics(); }
    private void ApplyFilter() { if (string.IsNullOrWhiteSpace(SearchText)) { _ = ReloadDataAsync(); return; } var key = SearchText.Trim().ToLower(); Items = new ObservableCollection<EquipmentInfo>(Items.Where(i => i.EquipmentId.ToLower().Contains(key) || i.EquipmentName.ToLower().Contains(key)).ToList()); }
    private void OnAdd() { EditingItem = new EquipmentInfo { Status = "Available" }; IsEditing = true; SaveCommand.RaiseCanExecuteChanged(); }
    private async void OnSave() { try { await _service.SaveEquipmentAsync(EditingItem); IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void OnCancel() { IsEditing = false; EditingItem = new EquipmentInfo(); }
    private async void OnDelete(EquipmentInfo? item) { if (item == null) return; try { if (System.Windows.MessageBox.Show($"确定删除?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return; await _service.DeleteEquipmentAsync(item.EquipmentId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(AvailableCount)); }
}

public class MaterialManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<MaterialInfo> _items = [];
    private MaterialInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private MaterialInfo _editingItem = new();

    public ObservableCollection<MaterialInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public MaterialInfo? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) { if (_selectedItem != null) EditingItem = new MaterialInfo { MaterialId = _selectedItem.MaterialId, MaterialName = _selectedItem.MaterialName, MaterialType = _selectedItem.MaterialType, Unit = _selectedItem.Unit, Status = _selectedItem.Status, Supplier = _selectedItem.Supplier, Description = _selectedItem.Description }; } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public MaterialInfo EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public int TotalCount => Items.Count;
    public int ActiveCount => Items.Count(i => i.Status == "Active");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<MaterialInfo?> DeleteCommand { get; }

    public MaterialManagementViewModel(IMasterDataService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingItem.MaterialId));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<MaterialInfo?>(OnDelete, i => i != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllMaterialsAsync(); Items = new ObservableCollection<MaterialInfo>(items); UpdateStatistics(); }
    private void ApplyFilter() { if (string.IsNullOrWhiteSpace(SearchText)) { _ = ReloadDataAsync(); return; } var key = SearchText.Trim().ToLower(); Items = new ObservableCollection<MaterialInfo>(Items.Where(i => i.MaterialId.ToLower().Contains(key) || i.MaterialName.ToLower().Contains(key)).ToList()); }
    private void OnAdd() { EditingItem = new MaterialInfo { Status = "Active" }; IsEditing = true; SaveCommand.RaiseCanExecuteChanged(); }
    private async void OnSave() { try { await _service.SaveMaterialAsync(EditingItem); IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void OnCancel() { IsEditing = false; EditingItem = new MaterialInfo(); }
    private async void OnDelete(MaterialInfo? item) { if (item == null) return; try { if (System.Windows.MessageBox.Show($"确定删除?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return; await _service.DeleteMaterialAsync(item.MaterialId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(ActiveCount)); }
}

public class CustomerManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<CustomerInfo> _items = [];
    private CustomerInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private CustomerInfo _editingItem = new();

    public ObservableCollection<CustomerInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public CustomerInfo? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) { if (_selectedItem != null) EditingItem = new CustomerInfo { CustomerId = _selectedItem.CustomerId, CustomerName = _selectedItem.CustomerName, ContactPerson = _selectedItem.ContactPerson, Phone = _selectedItem.Phone, Email = _selectedItem.Email, Status = _selectedItem.Status, Address = _selectedItem.Address }; } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public CustomerInfo EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public int TotalCount => Items.Count;
    public int ActiveCount => Items.Count(i => i.Status == "Active");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<CustomerInfo?> DeleteCommand { get; }

    public CustomerManagementViewModel(IMasterDataService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingItem.CustomerId));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<CustomerInfo?>(OnDelete, i => i != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllCustomersAsync(); Items = new ObservableCollection<CustomerInfo>(items); UpdateStatistics(); }
    private void ApplyFilter() { if (string.IsNullOrWhiteSpace(SearchText)) { _ = ReloadDataAsync(); return; } var key = SearchText.Trim().ToLower(); Items = new ObservableCollection<CustomerInfo>(Items.Where(i => i.CustomerId.ToLower().Contains(key) || i.CustomerName.ToLower().Contains(key)).ToList()); }
    private void OnAdd() { EditingItem = new CustomerInfo { Status = "Active" }; IsEditing = true; SaveCommand.RaiseCanExecuteChanged(); }
    private async void OnSave() { try { await _service.SaveCustomerAsync(EditingItem); IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void OnCancel() { IsEditing = false; EditingItem = new CustomerInfo(); }
    private async void OnDelete(CustomerInfo? item) { if (item == null) return; try { if (System.Windows.MessageBox.Show($"确定删除?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return; await _service.DeleteCustomerAsync(item.CustomerId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(ActiveCount)); }
}

public class ReasonCodeManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<ReasonCodeInfo> _items = [];
    private ReasonCodeInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private ReasonCodeInfo _editingItem = new();

    public ObservableCollection<ReasonCodeInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public ReasonCodeInfo? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) { if (_selectedItem != null) EditingItem = new ReasonCodeInfo { ReasonCodeId = _selectedItem.ReasonCodeId, ReasonCode = _selectedItem.ReasonCode, ReasonName = _selectedItem.ReasonName, Category = _selectedItem.Category, Status = _selectedItem.Status, Description = _selectedItem.Description }; } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public ReasonCodeInfo EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public int TotalCount => Items.Count;
    public int ActiveCount => Items.Count(i => i.Status == "Active");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<ReasonCodeInfo?> DeleteCommand { get; }

    public ReasonCodeManagementViewModel(IMasterDataService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingItem.ReasonCodeId));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<ReasonCodeInfo?>(OnDelete, i => i != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllReasonCodesAsync(); Items = new ObservableCollection<ReasonCodeInfo>(items); UpdateStatistics(); }
    private void ApplyFilter() { if (string.IsNullOrWhiteSpace(SearchText)) { _ = ReloadDataAsync(); return; } var key = SearchText.Trim().ToLower(); Items = new ObservableCollection<ReasonCodeInfo>(Items.Where(i => i.ReasonCodeId.ToLower().Contains(key) || i.ReasonName.ToLower().Contains(key)).ToList()); }
    private void OnAdd() { EditingItem = new ReasonCodeInfo { Status = "Active" }; IsEditing = true; SaveCommand.RaiseCanExecuteChanged(); }
    private async void OnSave() { try { await _service.SaveReasonCodeAsync(EditingItem); IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void OnCancel() { IsEditing = false; EditingItem = new ReasonCodeInfo(); }
    private async void OnDelete(ReasonCodeInfo? item) { if (item == null) return; try { if (System.Windows.MessageBox.Show($"确定删除?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return; await _service.DeleteReasonCodeAsync(item.ReasonCodeId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(ActiveCount)); }
}

public class DefectCodeManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<DefectCodeInfo> _items = [];
    private DefectCodeInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private DefectCodeInfo _editingItem = new();

    public ObservableCollection<DefectCodeInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public DefectCodeInfo? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) { if (_selectedItem != null) EditingItem = new DefectCodeInfo { DefectCodeId = _selectedItem.DefectCodeId, DefectCode = _selectedItem.DefectCode, DefectName = _selectedItem.DefectName, Category = _selectedItem.Category, Severity = _selectedItem.Severity, Status = _selectedItem.Status, Description = _selectedItem.Description }; } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public DefectCodeInfo EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public int TotalCount => Items.Count;
    public int ActiveCount => Items.Count(i => i.Status == "Active");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<DefectCodeInfo?> DeleteCommand { get; }

    public DefectCodeManagementViewModel(IMasterDataService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingItem.DefectCodeId));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<DefectCodeInfo?>(OnDelete, i => i != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllDefectCodesAsync(); Items = new ObservableCollection<DefectCodeInfo>(items); UpdateStatistics(); }
    private void ApplyFilter() { if (string.IsNullOrWhiteSpace(SearchText)) { _ = ReloadDataAsync(); return; } var key = SearchText.Trim().ToLower(); Items = new ObservableCollection<DefectCodeInfo>(Items.Where(i => i.DefectCodeId.ToLower().Contains(key) || i.DefectName.ToLower().Contains(key)).ToList()); }
    private void OnAdd() { EditingItem = new DefectCodeInfo { Status = "Active" }; IsEditing = true; SaveCommand.RaiseCanExecuteChanged(); }
    private async void OnSave() { try { await _service.SaveDefectCodeAsync(EditingItem); IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void OnCancel() { IsEditing = false; EditingItem = new DefectCodeInfo(); }
    private async void OnDelete(DefectCodeInfo? item) { if (item == null) return; try { if (System.Windows.MessageBox.Show($"确定删除?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return; await _service.DeleteDefectCodeAsync(item.DefectCodeId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(ActiveCount)); }
}

public class CarrierManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<CarrierInfo> _items = [];
    private CarrierInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private CarrierInfo _editingItem = new();

    public ObservableCollection<CarrierInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public CarrierInfo? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) { if (_selectedItem != null) EditingItem = new CarrierInfo { CarrierId = _selectedItem.CarrierId, CarrierName = _selectedItem.CarrierName, CarrierType = _selectedItem.CarrierType, Status = _selectedItem.Status, Capacity = _selectedItem.Capacity, Location = _selectedItem.Location }; } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public CarrierInfo EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public int TotalCount => Items.Count;
    public int AvailableCount => Items.Count(i => i.Status == "Available");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<CarrierInfo?> DeleteCommand { get; }

    public CarrierManagementViewModel(IMasterDataService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingItem.CarrierId));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<CarrierInfo?>(OnDelete, i => i != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllCarriersAsync(); Items = new ObservableCollection<CarrierInfo>(items); UpdateStatistics(); }
    private void ApplyFilter() { if (string.IsNullOrWhiteSpace(SearchText)) { _ = ReloadDataAsync(); return; } var key = SearchText.Trim().ToLower(); Items = new ObservableCollection<CarrierInfo>(Items.Where(i => i.CarrierId.ToLower().Contains(key) || i.CarrierName.ToLower().Contains(key)).ToList()); }
    private void OnAdd() { EditingItem = new CarrierInfo { Status = "Available" }; IsEditing = true; SaveCommand.RaiseCanExecuteChanged(); }
    private async void OnSave() { try { await _service.SaveCarrierAsync(EditingItem); IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void OnCancel() { IsEditing = false; EditingItem = new CarrierInfo(); }
    private async void OnDelete(CarrierInfo? item) { if (item == null) return; try { if (System.Windows.MessageBox.Show($"确定删除?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return; await _service.DeleteCarrierAsync(item.CarrierId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(AvailableCount)); }
}

public class YieldRuleManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<YieldRuleInfo> _items = [];
    private YieldRuleInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private YieldRuleInfo _editingItem = new();

    public ObservableCollection<YieldRuleInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public YieldRuleInfo? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) { if (_selectedItem != null) EditingItem = new YieldRuleInfo { RuleId = _selectedItem.RuleId, RuleName = _selectedItem.RuleName, ProductId = _selectedItem.ProductId, StepCode = _selectedItem.StepCode, TargetYield = _selectedItem.TargetYield, WarningYield = _selectedItem.WarningYield, Status = _selectedItem.Status }; } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public YieldRuleInfo EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public int TotalCount => Items.Count;
    public int ActiveCount => Items.Count(i => i.Status == "Active");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<YieldRuleInfo?> DeleteCommand { get; }

    public YieldRuleManagementViewModel(IMasterDataService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingItem.RuleId));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<YieldRuleInfo?>(OnDelete, i => i != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllYieldRulesAsync(); Items = new ObservableCollection<YieldRuleInfo>(items); UpdateStatistics(); }
    private void ApplyFilter() { if (string.IsNullOrWhiteSpace(SearchText)) { _ = ReloadDataAsync(); return; } var key = SearchText.Trim().ToLower(); Items = new ObservableCollection<YieldRuleInfo>(Items.Where(i => i.RuleId.ToLower().Contains(key) || i.RuleName.ToLower().Contains(key)).ToList()); }
    private void OnAdd() { EditingItem = new YieldRuleInfo { Status = "Active" }; IsEditing = true; SaveCommand.RaiseCanExecuteChanged(); }
    private async void OnSave() { try { await _service.SaveYieldRuleAsync(EditingItem); IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void OnCancel() { IsEditing = false; EditingItem = new YieldRuleInfo(); }
    private async void OnDelete(YieldRuleInfo? item) { if (item == null) return; try { if (System.Windows.MessageBox.Show($"确定删除?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return; await _service.DeleteYieldRuleAsync(item.RuleId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(ActiveCount)); }
}

public class ScrapRuleManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<ScrapRuleInfo> _items = [];
    private ScrapRuleInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private ScrapRuleInfo _editingItem = new();

    public ObservableCollection<ScrapRuleInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public ScrapRuleInfo? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) { if (_selectedItem != null) EditingItem = new ScrapRuleInfo { RuleId = _selectedItem.RuleId, RuleName = _selectedItem.RuleName, ProductId = _selectedItem.ProductId, StepCode = _selectedItem.StepCode, MaxScrapQty = _selectedItem.MaxScrapQty, MaxScrapRate = _selectedItem.MaxScrapRate, Status = _selectedItem.Status }; } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public ScrapRuleInfo EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public int TotalCount => Items.Count;
    public int ActiveCount => Items.Count(i => i.Status == "Active");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<ScrapRuleInfo?> DeleteCommand { get; }

    public ScrapRuleManagementViewModel(IMasterDataService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingItem.RuleId));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<ScrapRuleInfo?>(OnDelete, i => i != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllScrapRulesAsync(); Items = new ObservableCollection<ScrapRuleInfo>(items); UpdateStatistics(); }
    private void ApplyFilter() { if (string.IsNullOrWhiteSpace(SearchText)) { _ = ReloadDataAsync(); return; } var key = SearchText.Trim().ToLower(); Items = new ObservableCollection<ScrapRuleInfo>(Items.Where(i => i.RuleId.ToLower().Contains(key) || i.RuleName.ToLower().Contains(key)).ToList()); }
    private void OnAdd() { EditingItem = new ScrapRuleInfo { Status = "Active" }; IsEditing = true; SaveCommand.RaiseCanExecuteChanged(); }
    private async void OnSave() { try { await _service.SaveScrapRuleAsync(EditingItem); IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void OnCancel() { IsEditing = false; EditingItem = new ScrapRuleInfo(); }
    private async void OnDelete(ScrapRuleInfo? item) { if (item == null) return; try { if (System.Windows.MessageBox.Show($"确定删除?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return; await _service.DeleteScrapRuleAsync(item.RuleId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(ActiveCount)); }
}

public class AlarmRuleManagementViewModel : BindableBase
{
    private readonly IMasterDataService _service;
    private ObservableCollection<AlarmRuleInfo> _items = [];
    private AlarmRuleInfo? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isEditing;
    private AlarmRuleInfo _editingItem = new();

    public ObservableCollection<AlarmRuleInfo> Items { get => _items; set => SetProperty(ref _items, value); }
    public AlarmRuleInfo? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) { if (_selectedItem != null) EditingItem = new AlarmRuleInfo { RuleId = _selectedItem.RuleId, RuleName = _selectedItem.RuleName, EquipmentId = _selectedItem.EquipmentId, AlarmType = _selectedItem.AlarmType, Severity = _selectedItem.Severity, Condition = _selectedItem.Condition, Status = _selectedItem.Status }; } } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public AlarmRuleInfo EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public int TotalCount => Items.Count;
    public int ActiveCount => Items.Count(i => i.Status == "Active");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand<AlarmRuleInfo?> DeleteCommand { get; }

    public AlarmRuleManagementViewModel(IMasterDataService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        SaveCommand = new DelegateCommand(OnSave, () => !string.IsNullOrWhiteSpace(EditingItem.RuleId));
        CancelCommand = new DelegateCommand(OnCancel);
        DeleteCommand = new DelegateCommand<AlarmRuleInfo?>(OnDelete, i => i != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllAlarmRulesAsync(); Items = new ObservableCollection<AlarmRuleInfo>(items); UpdateStatistics(); }
    private void ApplyFilter() { if (string.IsNullOrWhiteSpace(SearchText)) { _ = ReloadDataAsync(); return; } var key = SearchText.Trim().ToLower(); Items = new ObservableCollection<AlarmRuleInfo>(Items.Where(i => i.RuleId.ToLower().Contains(key) || i.RuleName.ToLower().Contains(key)).ToList()); }
    private void OnAdd() { EditingItem = new AlarmRuleInfo { Status = "Active" }; IsEditing = true; SaveCommand.RaiseCanExecuteChanged(); }
    private async void OnSave() { try { await _service.SaveAlarmRuleAsync(EditingItem); IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; } }
    private void OnCancel() { IsEditing = false; EditingItem = new AlarmRuleInfo(); }
    private async void OnDelete(AlarmRuleInfo? item) { if (item == null) return; try { if (System.Windows.MessageBox.Show($"确定删除?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) return; await _service.DeleteAlarmRuleAsync(item.RuleId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"删除失败: {ex.Message}"; } }
    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; IsEditing = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(ActiveCount)); }
}
