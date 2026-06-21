using Prism.Mvvm;
using Prism.Commands;
using MES.Modules.Recipe.Models;
using MES.Modules.Recipe.Services;
using System.Collections.ObjectModel;

namespace MES.Modules.Recipe.ViewModels;

public class RecipeListViewModel : BindableBase
{
    private readonly IRecipeService _recipeService;
    private ObservableCollection<RecipeListModel> _items = [];
    private RecipeListModel? _selectedItem;
    private string _searchText = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _totalCount;
    private int _activeCount;
    private int _draftCount;

    public ObservableCollection<RecipeListModel> Items { get => _items; set => SetProperty(ref _items, value); }
    public RecipeListModel? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); FilterCommand.Execute(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int ActiveCount { get => _activeCount; set => SetProperty(ref _activeCount, value); }
    public int DraftCount { get => _draftCount; set => SetProperty(ref _draftCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand FilterCommand { get; }

    public RecipeListViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        FilterCommand = new DelegateCommand(ExecuteFilter);
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _recipeService.GetAllRecipesAsync();
            Items = new ObservableCollection<RecipeListModel>(data);
            UpdateStatistics();
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

    private void ExecuteFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            LoadMockData();
            return;
        }

        var filtered = Items.Where(r =>
            r.RecipeName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            r.RecipeId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            r.EquipmentType.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

        Items = new ObservableCollection<RecipeListModel>(filtered);
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<RecipeListModel>
        {
            new() { Id = "RCP-001", RecipeId = "RCP-WB-001", RecipeName = "WB Standard Recipe", EquipmentType = "Wire Bonder", Version = "1.0", Status = "Active", CreatedBy = "张工", CreatedDate = DateTime.Now.AddDays(-30) },
            new() { Id = "RCP-002", RecipeId = "RCP-DB-001", RecipeName = "DB Standard Recipe", EquipmentType = "Die Bonder", Version = "2.0", Status = "Active", CreatedBy = "李工", CreatedDate = DateTime.Now.AddDays(-20) },
            new() { Id = "RCP-003", RecipeId = "RCP-MP-001", RecipeName = "MP Standard Recipe", EquipmentType = "Mold Press", Version = "1.5", Status = "Draft", CreatedBy = "王工", CreatedDate = DateTime.Now.AddDays(-10) },
            new() { Id = "RCP-004", RecipeId = "RCP-WS-001", RecipeName = "WS Standard Recipe", EquipmentType = "Wafer Saw", Version = "1.0", Status = "Active", CreatedBy = "赵工", CreatedDate = DateTime.Now.AddDays(-15) },
            new() { Id = "RCP-005", RecipeId = "RCP-TH-001", RecipeName = "TH Standard Recipe", EquipmentType = "Trim & Form", Version = "1.2", Status = "Obsolete", CreatedBy = "张工", CreatedDate = DateTime.Now.AddDays(-60) },
        };
        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        ActiveCount = Items.Count(r => r.Status == "Active");
        DraftCount = Items.Count(r => r.Status == "Draft");
    }
}

public class RecipeParameterViewModel : BindableBase
{
    private readonly IRecipeService _recipeService;
    private ObservableCollection<RecipeParameterItem> _items = [];
    private RecipeParameterItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _totalCount;
    private bool _canSave;

    public ObservableCollection<RecipeParameterItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public RecipeParameterItem? SelectedItem { get => _selectedItem; set { SetProperty(ref _selectedItem, value); _canSave = _selectedItem != null; SaveCommand.RaiseCanExecuteChanged(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand DeleteCommand { get; }

    public RecipeParameterViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        SaveCommand = new DelegateCommand(async () => await SaveSelectedItemAsync(), () => _canSave);
        AddCommand = new DelegateCommand(AddNewParameter);
        DeleteCommand = new DelegateCommand(async () => await DeleteSelectedItemAsync(), () => SelectedItem != null);
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _recipeService.GetRecipeParametersAsync(string.Empty);
            Items = new ObservableCollection<RecipeParameterItem>(data);
            TotalCount = Items.Count;
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

    private async Task SaveSelectedItemAsync()
    {
        if (SelectedItem == null) return;
        IsLoading = true;
        try
        {
            await _recipeService.SaveParameterAsync(SelectedItem);
            await LoadDataAsync();
            ErrorMessage = "保存成功";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteSelectedItemAsync()
    {
        if (SelectedItem == null) return;
        IsLoading = true;
        try
        {
            await _recipeService.DeleteParameterAsync(SelectedItem.Id);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            TotalCount = Items.Count;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddNewParameter()
    {
        var newParam = new RecipeParameterItem
        {
            Id = $"RP-{Items.Count + 1:D3}",
            ParameterName = "NewParameter",
            MinValue = 0,
            MaxValue = 100,
            TargetValue = 50,
            Unit = ""
        };
        Items.Add(newParam);
        SelectedItem = newParam;
        TotalCount = Items.Count;
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<RecipeParameterItem>
        {
            new() { Id = "RP-001", ParameterName = "BondForce", MinValue = 10, MaxValue = 50, TargetValue = 30, Unit = "gf" },
            new() { Id = "RP-002", ParameterName = "BondTime", MinValue = 10, MaxValue = 100, TargetValue = 50, Unit = "ms" },
            new() { Id = "RP-003", ParameterName = "Temperature", MinValue = 150, MaxValue = 250, TargetValue = 200, Unit = "°C" },
            new() { Id = "RP-004", ParameterName = "Power", MinValue = 5, MaxValue = 30, TargetValue = 15, Unit = "W" },
            new() { Id = "RP-005", ParameterName = "Pressure", MinValue = 100, MaxValue = 500, TargetValue = 300, Unit = "psi" },
        };
        TotalCount = Items.Count;
    }
}

public class RecipeEquipmentViewModel : BindableBase
{
    private readonly IRecipeService _recipeService;
    private ObservableCollection<RecipeEquipmentItem> _items = [];
    private RecipeEquipmentItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _totalCount;
    private int _boundCount;

    public ObservableCollection<RecipeEquipmentItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public RecipeEquipmentItem? SelectedItem { get => _selectedItem; set { SetProperty(ref _selectedItem, value); UnbindCommand.RaiseCanExecuteChanged(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int BoundCount { get => _boundCount; set => SetProperty(ref _boundCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand BindCommand { get; }
    public DelegateCommand UnbindCommand { get; }

    public RecipeEquipmentViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        BindCommand = new DelegateCommand(async () => await BindNewEquipmentAsync());
        UnbindCommand = new DelegateCommand(async () => await UnbindSelectedAsync(), () => SelectedItem != null);
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _recipeService.GetRecipeEquipmentBindingsAsync();
            Items = new ObservableCollection<RecipeEquipmentItem>(data);
            UpdateStatistics();
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

    private async Task BindNewEquipmentAsync()
    {
        IsLoading = true;
        try
        {
            await _recipeService.BindRecipeToEquipmentAsync("RCP-WB-001", $"WB-{new Random().Next(10, 99)}");
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"绑定失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task UnbindSelectedAsync()
    {
        if (SelectedItem == null) return;
        IsLoading = true;
        try
        {
            await _recipeService.UnbindRecipeFromEquipmentAsync(SelectedItem.Id);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"解绑失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        BoundCount = Items.Count(e => e.Status == "Bound");
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<RecipeEquipmentItem>
        {
            new() { Id = "RE-001", EquipmentId = "WB-01", RecipeId = "RCP-WB-001", Status = "Bound", LastUsed = DateTime.Now.AddHours(-2) },
            new() { Id = "RE-002", EquipmentId = "WB-02", RecipeId = "RCP-WB-001", Status = "Bound", LastUsed = DateTime.Now.AddHours(-5) },
            new() { Id = "RE-003", EquipmentId = "DB-01", RecipeId = "RCP-DB-001", Status = "Bound", LastUsed = DateTime.Now.AddHours(-1) },
            new() { Id = "RE-004", EquipmentId = "MP-01", RecipeId = "RCP-MP-001", Status = "Bound", LastUsed = DateTime.Now.AddHours(-3) },
        };
        UpdateStatistics();
    }
}

public class RecipeApprovalViewModel : BindableBase
{
    private readonly IRecipeService _recipeService;
    private ObservableCollection<RecipeApprovalItem> _items = [];
    private RecipeApprovalItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private string _reviewComment = string.Empty;
    private int _totalCount;
    private int _pendingCount;
    private int _approvedCount;
    private int _rejectedCount;

    public ObservableCollection<RecipeApprovalItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public RecipeApprovalItem? SelectedItem { get => _selectedItem; set { SetProperty(ref _selectedItem, value); UpdateCommandStates(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string ReviewComment { get => _reviewComment; set => SetProperty(ref _reviewComment, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int PendingCount { get => _pendingCount; set => SetProperty(ref _pendingCount, value); }
    public int ApprovedCount { get => _approvedCount; set => SetProperty(ref _approvedCount, value); }
    public int RejectedCount { get => _rejectedCount; set => SetProperty(ref _rejectedCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ApproveCommand { get; }
    public DelegateCommand RejectCommand { get; }

    public RecipeApprovalViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        ApproveCommand = new DelegateCommand(async () => await ApproveSelectedAsync(), () => SelectedItem?.Status == "Pending");
        RejectCommand = new DelegateCommand(async () => await RejectSelectedAsync(), () => SelectedItem?.Status == "Pending");
        LoadMockData();
    }

    private void UpdateCommandStates()
    {
        ApproveCommand.RaiseCanExecuteChanged();
        RejectCommand.RaiseCanExecuteChanged();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _recipeService.GetPendingApprovalsAsync();
            Items = new ObservableCollection<RecipeApprovalItem>(data);
            UpdateStatistics();
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

    private async Task ApproveSelectedAsync()
    {
        if (SelectedItem == null || SelectedItem.Status != "Pending") return;
        IsLoading = true;
        try
        {
            await _recipeService.ApproveRecipeAsync(SelectedItem.Id, ReviewComment);
            SelectedItem.Status = "Approved";
            SelectedItem.ApprovedDate = DateTime.Now;
            SelectedItem.Comment = ReviewComment;
            ReviewComment = string.Empty;
            UpdateStatistics();
            UpdateCommandStates();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"审批失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RejectSelectedAsync()
    {
        if (SelectedItem == null || SelectedItem.Status != "Pending") return;
        if (string.IsNullOrWhiteSpace(ReviewComment))
        {
            ErrorMessage = "拒绝时必须填写原因";
            return;
        }
        IsLoading = true;
        try
        {
            await _recipeService.RejectRecipeAsync(SelectedItem.Id, ReviewComment);
            SelectedItem.Status = "Rejected";
            SelectedItem.RejectedDate = DateTime.Now;
            SelectedItem.RejectionReason = ReviewComment;
            ReviewComment = string.Empty;
            UpdateStatistics();
            UpdateCommandStates();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"拒绝失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        PendingCount = Items.Count(a => a.Status == "Pending");
        ApprovedCount = Items.Count(a => a.Status == "Approved");
        RejectedCount = Items.Count(a => a.Status == "Rejected");
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<RecipeApprovalItem>
        {
            new() { Id = "RA-001", RecipeId = "RCP-MP-001", RecipeName = "MP Standard Recipe", SubmittedBy = "王工", SubmittedDate = DateTime.Now.AddDays(-2), Status = "Pending", Reviewer = "张经理" },
            new() { Id = "RA-002", RecipeId = "RCP-WB-002", RecipeName = "WB Advanced Recipe", SubmittedBy = "李工", SubmittedDate = DateTime.Now.AddDays(-5), Status = "Approved", Reviewer = "张经理", ApprovedDate = DateTime.Now.AddDays(-3) },
            new() { Id = "RA-003", RecipeId = "RCP-DB-002", RecipeName = "DB High-Speed Recipe", SubmittedBy = "赵工", SubmittedDate = DateTime.Now.AddDays(-1), Status = "Rejected", Reviewer = "张经理", RejectionReason = "参数超出安全范围" },
        };
        UpdateStatistics();
    }
}

public class RecipeDispatchViewModel : BindableBase
{
    private readonly IRecipeService _recipeService;
    private ObservableCollection<RecipeListModel> _items = [];
    private RecipeListModel? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private string _targetEquipment = string.Empty;
    private int _totalCount;
    private int _dispatchedCount;

    public ObservableCollection<RecipeListModel> Items { get => _items; set => SetProperty(ref _items, value); }
    public RecipeListModel? SelectedItem { get => _selectedItem; set { SetProperty(ref _selectedItem, value); DispatchCommand.RaiseCanExecuteChanged(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string TargetEquipment { get => _targetEquipment; set { SetProperty(ref _targetEquipment, value); DispatchCommand.RaiseCanExecuteChanged(); } }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int DispatchedCount { get => _dispatchedCount; set => SetProperty(ref _dispatchedCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand DispatchCommand { get; }

    public RecipeDispatchViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        DispatchCommand = new DelegateCommand(async () => await DispatchSelectedAsync(), () => SelectedItem != null && !string.IsNullOrWhiteSpace(TargetEquipment));
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _recipeService.GetDispatchableRecipesAsync();
            Items = new ObservableCollection<RecipeListModel>(data);
            UpdateStatistics();
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

    private async Task DispatchSelectedAsync()
    {
        if (SelectedItem == null || string.IsNullOrWhiteSpace(TargetEquipment)) return;
        IsLoading = true;
        try
        {
            await _recipeService.DispatchRecipeAsync(SelectedItem.Id, TargetEquipment);
            SelectedItem.CurrentEquipment = TargetEquipment;
            SelectedItem.Status = "Dispatched";
            UpdateStatistics();
            ErrorMessage = $"Recipe {SelectedItem.RecipeName} 已派发到设备 {TargetEquipment}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"派发失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        DispatchedCount = Items.Count(r => r.Status == "Dispatched");
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<RecipeListModel>
        {
            new() { Id = "RCP-001", RecipeId = "RCP-WB-001", RecipeName = "WB Standard Recipe", EquipmentType = "Wire Bonder", Version = "1.0", Status = "Active", CurrentEquipment = "WB-01" },
            new() { Id = "RCP-002", RecipeId = "RCP-DB-001", RecipeName = "DB Standard Recipe", EquipmentType = "Die Bonder", Version = "2.0", Status = "Active", CurrentEquipment = "DB-01" },
            new() { Id = "RCP-004", RecipeId = "RCP-WS-001", RecipeName = "WS Standard Recipe", EquipmentType = "Wafer Saw", Version = "1.0", Status = "Active", CurrentEquipment = "WS-01" },
        };
        UpdateStatistics();
    }
}

public class RecipeVersionViewModel : BindableBase
{
    private readonly IRecipeService _recipeService;
    private ObservableCollection<RecipeListModel> _items = [];
    private RecipeListModel? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private string _newVersion = string.Empty;
    private int _totalCount;
    private int _activeVersionCount;

    public ObservableCollection<RecipeListModel> Items { get => _items; set => SetProperty(ref _items, value); }
    public RecipeListModel? SelectedItem { get => _selectedItem; set { SetProperty(ref _selectedItem, value); CreateVersionCommand.RaiseCanExecuteChanged(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string NewVersion { get => _newVersion; set { SetProperty(ref _newVersion, value); CreateVersionCommand.RaiseCanExecuteChanged(); } }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int ActiveVersionCount { get => _activeVersionCount; set => SetProperty(ref _activeVersionCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand CreateVersionCommand { get; }
    public DelegateCommand SetActiveCommand { get; }

    public RecipeVersionViewModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        CreateVersionCommand = new DelegateCommand(async () => await CreateNewVersionAsync(), () => SelectedItem != null && !string.IsNullOrWhiteSpace(NewVersion));
        SetActiveCommand = new DelegateCommand(async () => await SetSelectedAsActiveAsync(), () => SelectedItem?.Status != "Active");
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _recipeService.GetRecipeVersionsAsync(string.Empty);
            Items = new ObservableCollection<RecipeListModel>(data);
            UpdateStatistics();
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

    private async Task CreateNewVersionAsync()
    {
        if (SelectedItem == null || string.IsNullOrWhiteSpace(NewVersion)) return;
        IsLoading = true;
        try
        {
            await _recipeService.CreateNewVersionAsync(SelectedItem.Id, NewVersion);
            await LoadDataAsync();
            NewVersion = string.Empty;
            ErrorMessage = $"新版本 {NewVersion} 已创建";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"创建版本失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SetSelectedAsActiveAsync()
    {
        if (SelectedItem == null) return;
        IsLoading = true;
        try
        {
            SelectedItem.Status = "Active";
            UpdateStatistics();
            ErrorMessage = $"版本 {SelectedItem.Version} 已激活";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"激活失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        ActiveVersionCount = Items.Count(r => r.Status == "Active");
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<RecipeListModel>
        {
            new() { Id = "RCP-001", RecipeId = "RCP-WB-001", RecipeName = "WB Standard Recipe", EquipmentType = "Wire Bonder", Version = "1.0", Status = "Active", CreatedBy = "张工", CreatedDate = DateTime.Now.AddDays(-30) },
            new() { Id = "RCP-001-v0.9", RecipeId = "RCP-WB-001", RecipeName = "WB Standard Recipe", EquipmentType = "Wire Bonder", Version = "0.9", Status = "Obsolete", CreatedBy = "张工", CreatedDate = DateTime.Now.AddDays(-60) },
            new() { Id = "RCP-002", RecipeId = "RCP-DB-001", RecipeName = "DB Standard Recipe", EquipmentType = "Die Bonder", Version = "2.0", Status = "Active", CreatedBy = "李工", CreatedDate = DateTime.Now.AddDays(-20) },
            new() { Id = "RCP-002-v1.0", RecipeId = "RCP-DB-001", RecipeName = "DB Standard Recipe", EquipmentType = "Die Bonder", Version = "1.0", Status = "Obsolete", CreatedBy = "李工", CreatedDate = DateTime.Now.AddDays(-50) },
        };
        UpdateStatistics();
    }
}
