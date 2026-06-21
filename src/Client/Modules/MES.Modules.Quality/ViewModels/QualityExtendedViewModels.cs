using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Navigation.Regions;
using MES.Modules.Quality.Models;
using MES.Modules.Quality.Services;

namespace MES.Modules.Quality.ViewModels;

/// <summary>
/// 首件检验 ViewModel - 管理首件检验记录
/// </summary>
public class FirstArticleInspectionViewModel : BindableBase
{
    private readonly IQualityService _service;
    private ObservableCollection<InspectionItem> _items = [];
    private InspectionItem? _selectedItem;
    private InspectionItem? _editingItem;
    private string _searchText = string.Empty;
    private string _resultFilter = "All";
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private bool _isEditing;

    public ObservableCollection<InspectionItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public InspectionItem? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) RaiseCanExecuteChanged(); } }
    public InspectionItem? EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string ResultFilter { get => _resultFilter; set { if (SetProperty(ref _resultFilter, value)) ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

    // 统计指标
    public int TotalCount => _items.Count;
    public int PassCount => _items.Count(x => x.Result == "Pass");
    public int FailCount => _items.Count(x => x.Result == "Fail");
    public int PendingCount => _items.Count(x => string.IsNullOrEmpty(x.Result));
    public double PassRate => _items.Any() ? (double)PassCount / _items.Count * 100 : 0;

    public ICollectionView FilteredView { get; }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand NewCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand SubmitCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }

    public FirstArticleInspectionViewModel(IQualityService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;

        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        NewCommand = new DelegateCommand(OnNew);
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), CanSave);
        CancelCommand = new DelegateCommand(OnCancel, () => IsEditing);
        DeleteCommand = new DelegateCommand(async () => await DeleteAsync(), () => SelectedItem is not null);
        SubmitCommand = new DelegateCommand(async () => await SubmitAsync(), () => SelectedItem is not null && string.IsNullOrEmpty(SelectedItem.Result));
        ClearFilterCommand = new DelegateCommand(OnClearFilter);

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            var all = await _service.GetInspectionsAsync();
            Items = new ObservableCollection<InspectionItem>(all.Where(x => x.InspectionType == "首检").OrderByDescending(x => x.InspectTime));
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

    private void OnNew()
    {
        var newItem = new InspectionItem
        {
            Id = "INS-FA-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            InspectionType = "首检",
            InspectTime = DateTime.Now,
            Result = "",
            DefectCount = 0
        };
        EditingItem = newItem;
        IsEditing = true;
        CancelCommand.RaiseCanExecuteChanged();
    }

    private async Task SaveAsync()
    {
        if (EditingItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            EditingItem.InspectionType = "首检";
            await _service.SaveInspectionAsync(EditingItem);
            IsEditing = false;
            EditingItem = null;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
        }
    }

    private bool CanSave() => EditingItem is not null && !string.IsNullOrEmpty(EditingItem.LotId) && !string.IsNullOrEmpty(EditingItem.Product);

    private void OnCancel()
    {
        IsEditing = false;
        EditingItem = null;
        CancelCommand.RaiseCanExecuteChanged();
    }

    private async Task DeleteAsync()
    {
        if (SelectedItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            await _service.DeleteInspectionAsync(SelectedItem.Id);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            UpdateStats();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
        }
    }

    private async Task SubmitAsync()
    {
        if (SelectedItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            SelectedItem.Result = "Pass";
            SelectedItem.InspectTime = DateTime.Now;
            await _service.SaveInspectionAsync(SelectedItem);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"提交失败: {ex.Message}";
        }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        ResultFilter = "All";
        _ = LoadDataAsync();
    }

    private void ApplyFilter() => FilteredView.Refresh();

    private bool FilterPredicate(object obj)
    {
        if (obj is not InspectionItem item) return false;
        var matchSearch = string.IsNullOrWhiteSpace(SearchText) ||
            item.LotId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Product.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Inspector.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var matchResult = ResultFilter == "All" || item.Result == ResultFilter;
        return matchSearch && matchResult;
    }

    private void UpdateStats()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(PassCount));
        RaisePropertyChanged(nameof(FailCount));
        RaisePropertyChanged(nameof(PendingCount));
        RaisePropertyChanged(nameof(PassRate));
    }

    private void RaiseCanExecuteChanged()
    {
        SaveCommand.RaiseCanExecuteChanged();
        DeleteCommand.RaiseCanExecuteChanged();
        SubmitCommand.RaiseCanExecuteChanged();
    }
}

/// <summary>
/// 巡检 ViewModel - 管理生产线巡检记录
/// </summary>
public class PatrolInspectionViewModel : BindableBase
{
    private readonly IQualityService _service;
    private ObservableCollection<InspectionItem> _items = [];
    private InspectionItem? _selectedItem;
    private InspectionItem? _editingItem;
    private string _searchText = string.Empty;
    private string _resultFilter = "All";
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private bool _isEditing;

    public ObservableCollection<InspectionItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public InspectionItem? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) RaiseCanExecuteChanged(); } }
    public InspectionItem? EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string ResultFilter { get => _resultFilter; set { if (SetProperty(ref _resultFilter, value)) ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

    // 统计指标
    public int TotalCount => _items.Count;
    public int PassCount => _items.Count(x => x.Result == "Pass");
    public int FailCount => _items.Count(x => x.Result == "Fail");
    public int TotalDefects => _items.Sum(x => x.DefectCount);
    public double PassRate => _items.Any() ? (double)PassCount / _items.Count * 100 : 0;
    public double AvgDefectsPerInspection => _items.Any() ? (double)TotalDefects / _items.Count : 0;

    public ICollectionView FilteredView { get; }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand NewCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand RecordDefectCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }

    public PatrolInspectionViewModel(IQualityService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;

        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        NewCommand = new DelegateCommand(OnNew);
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), CanSave);
        CancelCommand = new DelegateCommand(OnCancel, () => IsEditing);
        DeleteCommand = new DelegateCommand(async () => await DeleteAsync(), () => SelectedItem is not null);
        RecordDefectCommand = new DelegateCommand(async () => await RecordDefectAsync(), () => SelectedItem is not null);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            var all = await _service.GetInspectionsAsync();
            Items = new ObservableCollection<InspectionItem>(all.Where(x => x.InspectionType == "巡检").OrderByDescending(x => x.InspectTime));
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

    private void OnNew()
    {
        var newItem = new InspectionItem
        {
            Id = "INS-PA-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            InspectionType = "巡检",
            InspectTime = DateTime.Now,
            Result = "",
            DefectCount = 0
        };
        EditingItem = newItem;
        IsEditing = true;
        CancelCommand.RaiseCanExecuteChanged();
    }

    private async Task SaveAsync()
    {
        if (EditingItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            EditingItem.InspectionType = "巡检";
            EditingItem.Result = EditingItem.DefectCount > 0 ? "Fail" : "Pass";
            await _service.SaveInspectionAsync(EditingItem);
            IsEditing = false;
            EditingItem = null;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
        }
    }

    private bool CanSave() => EditingItem is not null && !string.IsNullOrEmpty(EditingItem.LotId) && !string.IsNullOrEmpty(EditingItem.Product);

    private void OnCancel()
    {
        IsEditing = false;
        EditingItem = null;
        CancelCommand.RaiseCanExecuteChanged();
    }

    private async Task DeleteAsync()
    {
        if (SelectedItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            await _service.DeleteInspectionAsync(SelectedItem.Id);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            UpdateStats();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
        }
    }

    private async Task RecordDefectAsync()
    {
        if (SelectedItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            SelectedItem.DefectCount++;
            SelectedItem.Result = "Fail";
            await _service.SaveInspectionAsync(SelectedItem);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"记录缺陷失败: {ex.Message}";
        }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        ResultFilter = "All";
        _ = LoadDataAsync();
    }

    private void ApplyFilter() => FilteredView.Refresh();

    private bool FilterPredicate(object obj)
    {
        if (obj is not InspectionItem item) return false;
        var matchSearch = string.IsNullOrWhiteSpace(SearchText) ||
            item.LotId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Product.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Inspector.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var matchResult = ResultFilter == "All" || item.Result == ResultFilter;
        return matchSearch && matchResult;
    }

    private void UpdateStats()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(PassCount));
        RaisePropertyChanged(nameof(FailCount));
        RaisePropertyChanged(nameof(TotalDefects));
        RaisePropertyChanged(nameof(PassRate));
        RaisePropertyChanged(nameof(AvgDefectsPerInspection));
    }

    private void RaiseCanExecuteChanged()
    {
        SaveCommand.RaiseCanExecuteChanged();
        DeleteCommand.RaiseCanExecuteChanged();
        RecordDefectCommand.RaiseCanExecuteChanged();
    }
}

/// <summary>
/// 不合格品管理 ViewModel - 管理不合格品处理流程
/// </summary>
public class NonconformingViewModel : BindableBase
{
    private readonly IQualityService _service;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<InspectionItem> _items = [];
    private InspectionItem? _selectedItem;
    private string _searchText = string.Empty;
    private string _statusFilter = "All";
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public ObservableCollection<InspectionItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public InspectionItem? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) RaiseCanExecuteChanged(); } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string StatusFilter { get => _statusFilter; set { if (SetProperty(ref _statusFilter, value)) ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    // 统计指标
    public int TotalCount => _items.Count;
    public int CriticalCount => _items.Count(x => x.DefectCount >= 5);
    public int MajorCount => _items.Count(x => x.DefectCount >= 2 && x.DefectCount < 5);
    public int MinorCount => _items.Count(x => x.DefectCount == 1);
    public int TotalDefects => _items.Sum(x => x.DefectCount);

    public ICollectionView FilteredView { get; }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ViewDetailCommand { get; }
    public DelegateCommand EscalateCommand { get; }
    public DelegateCommand ResolveCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }

    public NonconformingViewModel(IQualityService service, IRegionManager regionManager)
    {
        _service = service;
        _regionManager = regionManager;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;

        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        ViewDetailCommand = new DelegateCommand(OnViewDetail, () => SelectedItem is not null);
        EscalateCommand = new DelegateCommand(async () => await EscalateAsync(), () => SelectedItem is not null);
        ResolveCommand = new DelegateCommand(async () => await ResolveAsync(), () => SelectedItem is not null);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            var all = await _service.GetInspectionsAsync();
            Items = new ObservableCollection<InspectionItem>(all.Where(x => x.Result == "Fail" || x.DefectCount > 0).OrderByDescending(x => x.DefectCount).ThenByDescending(x => x.InspectTime));
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

    private void OnViewDetail()
    {
        if (SelectedItem is null) return;
        var parameters = new NavigationParameters
        {
            { "LotId", SelectedItem.LotId },
            { "Id", SelectedItem.Id }
        };
        _regionManager.RequestNavigate("MainContentRegion", "NonconformingDetailView", parameters);
    }

    private async Task EscalateAsync()
    {
        if (SelectedItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            SelectedItem.Comments = "[已升级] " + SelectedItem.Comments;
            await _service.SaveInspectionAsync(SelectedItem);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"升级失败: {ex.Message}";
        }
    }

    private async Task ResolveAsync()
    {
        if (SelectedItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            SelectedItem.Comments = "[已解决] " + SelectedItem.Comments;
            SelectedItem.DefectCount = 0;
            await _service.SaveInspectionAsync(SelectedItem);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            UpdateStats();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"解决失败: {ex.Message}";
        }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        StatusFilter = "All";
        _ = LoadDataAsync();
    }

    private void ApplyFilter() => FilteredView.Refresh();

    private bool FilterPredicate(object obj)
    {
        if (obj is not InspectionItem item) return false;
        var matchSearch = string.IsNullOrWhiteSpace(SearchText) ||
            item.LotId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Product.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var matchStatus = StatusFilter == "All" ||
            (StatusFilter == "Critical" && item.DefectCount >= 5) ||
            (StatusFilter == "Major" && item.DefectCount >= 2 && item.DefectCount < 5) ||
            (StatusFilter == "Minor" && item.DefectCount == 1);
        return matchSearch && matchStatus;
    }

    private void UpdateStats()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(CriticalCount));
        RaisePropertyChanged(nameof(MajorCount));
        RaisePropertyChanged(nameof(MinorCount));
        RaisePropertyChanged(nameof(TotalDefects));
    }

    private void RaiseCanExecuteChanged()
    {
        ViewDetailCommand.RaiseCanExecuteChanged();
        EscalateCommand.RaiseCanExecuteChanged();
        ResolveCommand.RaiseCanExecuteChanged();
    }
}

/// <summary>
/// FMEA ViewModel - 失效模式与影响分析
/// </summary>
public class FmeaViewModel : BindableBase
{
    private readonly IQualityService _service;
    private ObservableCollection<FmeaItem> _items = [];
    private FmeaItem? _selectedItem;
    private FmeaItem? _editingItem;
    private string _searchText = string.Empty;
    private string _riskFilter = "All";
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private bool _isEditing;

    public ObservableCollection<FmeaItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public FmeaItem? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) RaiseCanExecuteChanged(); } }
    public FmeaItem? EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string RiskFilter { get => _riskFilter; set { if (SetProperty(ref _riskFilter, value)) ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

    // 统计指标
    public int TotalCount => _items.Count;
    public int HighRiskCount => _items.Count(x => x.RPN >= 100);
    public int MediumRiskCount => _items.Count(x => x.RPN >= 50 && x.RPN < 100);
    public int LowRiskCount => _items.Count(x => x.RPN < 50);
    public double AvgRPN => _items.Any() ? _items.Average(x => x.RPN) : 0;
    public int MaxRPN => _items.Any() ? _items.Max(x => x.RPN) : 0;
    public string TopRiskItem => _items.OrderByDescending(x => x.RPN).FirstOrDefault()?.FailureMode ?? "无";

    public ICollectionView FilteredView { get; }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand NewCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand SortByRPNCommand { get; }

    public FmeaViewModel(IQualityService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;

        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        NewCommand = new DelegateCommand(OnNew);
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), CanSave);
        CancelCommand = new DelegateCommand(OnCancel, () => IsEditing);
        DeleteCommand = new DelegateCommand(async () => await DeleteAsync(), () => SelectedItem is not null);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        SortByRPNCommand = new DelegateCommand(OnSortByRPN);

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            var data = await _service.GetFmeaItemsAsync();
            Items = new ObservableCollection<FmeaItem>(data.OrderByDescending(x => x.RPN));
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

    private void OnNew()
    {
        var newItem = new FmeaItem
        {
            Id = "FMEA-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            Severity = 1,
            Occurrence = 1,
            Detection = 1
        };
        EditingItem = newItem;
        IsEditing = true;
        CancelCommand.RaiseCanExecuteChanged();
    }

    private async Task SaveAsync()
    {
        if (EditingItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            await _service.SaveFmeaAsync(EditingItem);
            IsEditing = false;
            EditingItem = null;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
        }
    }

    private bool CanSave() => EditingItem is not null && !string.IsNullOrEmpty(EditingItem.ProcessStep) && !string.IsNullOrEmpty(EditingItem.FailureMode);

    private void OnCancel()
    {
        IsEditing = false;
        EditingItem = null;
        CancelCommand.RaiseCanExecuteChanged();
    }

    private async Task DeleteAsync()
    {
        if (SelectedItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            await _service.DeleteFmeaAsync(SelectedItem.Id);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            UpdateStats();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
        }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        RiskFilter = "All";
        _ = LoadDataAsync();
    }

    private void OnSortByRPN()
    {
        Items = new ObservableCollection<FmeaItem>(Items.OrderByDescending(x => x.RPN));
        FilteredView.Refresh();
    }

    private void ApplyFilter() => FilteredView.Refresh();

    private bool FilterPredicate(object obj)
    {
        if (obj is not FmeaItem item) return false;
        var matchSearch = string.IsNullOrWhiteSpace(SearchText) ||
            item.ProcessStep.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.FailureMode.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.FailureEffect.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var matchRisk = RiskFilter == "All" ||
            (RiskFilter == "High" && item.RPN >= 100) ||
            (RiskFilter == "Medium" && item.RPN >= 50 && item.RPN < 100) ||
            (RiskFilter == "Low" && item.RPN < 50);
        return matchSearch && matchRisk;
    }

    private void UpdateStats()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(HighRiskCount));
        RaisePropertyChanged(nameof(MediumRiskCount));
        RaisePropertyChanged(nameof(LowRiskCount));
        RaisePropertyChanged(nameof(AvgRPN));
        RaisePropertyChanged(nameof(MaxRPN));
        RaisePropertyChanged(nameof(TopRiskItem));
    }

    private void RaiseCanExecuteChanged()
    {
        SaveCommand.RaiseCanExecuteChanged();
        DeleteCommand.RaiseCanExecuteChanged();
    }
}

/// <summary>
/// 质量目标 ViewModel - 管理和跟踪质量目标达成情况
/// </summary>
public class QualityTargetViewModel : BindableBase
{
    private readonly IQualityService _service;
    private ObservableCollection<QualityTargetItem> _items = [];
    private QualityTargetItem? _selectedItem;
    private QualityTargetItem? _editingItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private bool _isEditing;

    public ObservableCollection<QualityTargetItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public QualityTargetItem? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) RaiseCanExecuteChanged(); } }
    public QualityTargetItem? EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

    // 统计指标
    public int TotalCount => _items.Count;
    public int OnTargetCount => _items.Count(x => x.Status == "OnTarget");
    public int AtRiskCount => _items.Count(x => x.Status == "AtRisk");
    public int MissedCount => _items.Count(x => x.Status == "Missed");
    public double OverallAchievementRate => _items.Any() ? _items.Average(x => x.AchievementRate) : 0;

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand NewCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand UpdateCurrentValueCommand { get; }

    public QualityTargetViewModel(IQualityService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        NewCommand = new DelegateCommand(OnNew);
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), CanSave);
        CancelCommand = new DelegateCommand(OnCancel, () => IsEditing);
        DeleteCommand = new DelegateCommand(async () => await DeleteAsync(), () => SelectedItem is not null);
        UpdateCurrentValueCommand = new DelegateCommand(async () => await UpdateCurrentValueAsync(), () => SelectedItem is not null);

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            var data = await _service.GetQualityTargetsAsync();
            Items = new ObservableCollection<QualityTargetItem>(data.OrderByDescending(x => x.AchievementRate));
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

    private void OnNew()
    {
        var newItem = new QualityTargetItem
        {
            Id = "QT-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            TargetValue = 100,
            CurrentValue = 0,
            Unit = "%",
            Period = "月"
        };
        EditingItem = newItem;
        IsEditing = true;
        CancelCommand.RaiseCanExecuteChanged();
    }

    private async Task SaveAsync()
    {
        if (EditingItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            // 自动评估状态
            if (EditingItem.CurrentValue >= EditingItem.TargetValue)
                EditingItem.Status = "OnTarget";
            else if (EditingItem.CurrentValue >= EditingItem.TargetValue * 0.9)
                EditingItem.Status = "AtRisk";
            else
                EditingItem.Status = "Missed";

            await _service.SaveQualityTargetAsync(EditingItem);
            IsEditing = false;
            EditingItem = null;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
        }
    }

    private bool CanSave() => EditingItem is not null && !string.IsNullOrEmpty(EditingItem.TargetName);

    private void OnCancel()
    {
        IsEditing = false;
        EditingItem = null;
        CancelCommand.RaiseCanExecuteChanged();
    }

    private async Task DeleteAsync()
    {
        if (SelectedItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            Items.Remove(SelectedItem);
            SelectedItem = null;
            UpdateStats();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
        }
    }

    private async Task UpdateCurrentValueAsync()
    {
        if (SelectedItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            // 自动评估状态
            if (SelectedItem.CurrentValue >= SelectedItem.TargetValue)
                SelectedItem.Status = "OnTarget";
            else if (SelectedItem.CurrentValue >= SelectedItem.TargetValue * 0.9)
                SelectedItem.Status = "AtRisk";
            else
                SelectedItem.Status = "Missed";

            await _service.SaveQualityTargetAsync(SelectedItem);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"更新失败: {ex.Message}";
        }
    }

    private void UpdateStats()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(OnTargetCount));
        RaisePropertyChanged(nameof(AtRiskCount));
        RaisePropertyChanged(nameof(MissedCount));
        RaisePropertyChanged(nameof(OverallAchievementRate));
    }

    private void RaiseCanExecuteChanged()
    {
        SaveCommand.RaiseCanExecuteChanged();
        DeleteCommand.RaiseCanExecuteChanged();
        UpdateCurrentValueCommand.RaiseCanExecuteChanged();
    }
}

/// <summary>
/// MSA ViewModel - 测量系统分析
/// </summary>
public class MsaViewModel : BindableBase
{
    private readonly IQualityService _service;
    private ObservableCollection<MsaItem> _items = [];
    private MsaItem? _selectedItem;
    private MsaItem? _editingItem;
    private string _searchText = string.Empty;
    private string _statusFilter = "All";
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private bool _isEditing;

    public ObservableCollection<MsaItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public MsaItem? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) RaiseCanExecuteChanged(); } }
    public MsaItem? EditingItem { get => _editingItem; set => SetProperty(ref _editingItem, value); }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string StatusFilter { get => _statusFilter; set { if (SetProperty(ref _statusFilter, value)) ApplyFilter(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

    // 统计指标
    public int TotalCount => _items.Count;
    public int AcceptableCount => _items.Count(x => x.Status == "Acceptable");
    public int ConditionalCount => _items.Count(x => x.Status == "Conditional");
    public int UnacceptableCount => _items.Count(x => x.Status == "Unacceptable");
    public double AvgGRR => _items.Any() ? _items.Average(x => x.GRR) : 0;
    public int OverdueCalibrationCount => _items.Count(x => (DateTime.Now - x.LastCalibration).Days > 90);

    public ICollectionView FilteredView { get; }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand NewCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand CancelCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand CalibrateCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }

    public MsaViewModel(IQualityService service)
    {
        _service = service;
        FilteredView = CollectionViewSource.GetDefaultView(Items);
        FilteredView.Filter = FilterPredicate;

        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        NewCommand = new DelegateCommand(OnNew);
        SaveCommand = new DelegateCommand(async () => await SaveAsync(), CanSave);
        CancelCommand = new DelegateCommand(OnCancel, () => IsEditing);
        DeleteCommand = new DelegateCommand(async () => await DeleteAsync(), () => SelectedItem is not null);
        CalibrateCommand = new DelegateCommand(async () => await CalibrateAsync(), () => SelectedItem is not null);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            var data = await _service.GetMsaItemsAsync();
            Items = new ObservableCollection<MsaItem>(data.OrderBy(x => x.GaugeName));
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

    private void OnNew()
    {
        var newItem = new MsaItem
        {
            Id = "MSA-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            LastCalibration = DateTime.Now,
            OperatorCount = 3,
            PartCount = 10,
            TrialCount = 3
        };
        EditingItem = newItem;
        IsEditing = true;
        CancelCommand.RaiseCanExecuteChanged();
    }

    private async Task SaveAsync()
    {
        if (EditingItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            // 自动评估GRR状态
            if (EditingItem.GRR < 10)
                EditingItem.Status = "Acceptable";
            else if (EditingItem.GRR < 30)
                EditingItem.Status = "Conditional";
            else
                EditingItem.Status = "Unacceptable";

            await _service.SaveMsaAsync(EditingItem);
            IsEditing = false;
            EditingItem = null;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
        }
    }

    private bool CanSave() => EditingItem is not null && !string.IsNullOrEmpty(EditingItem.GaugeName);

    private void OnCancel()
    {
        IsEditing = false;
        EditingItem = null;
        CancelCommand.RaiseCanExecuteChanged();
    }

    private async Task DeleteAsync()
    {
        if (SelectedItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            Items.Remove(SelectedItem);
            SelectedItem = null;
            UpdateStats();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
        }
    }

    private async Task CalibrateAsync()
    {
        if (SelectedItem is null) return;
        try
        {
            ErrorMessage = string.Empty;
            SelectedItem.LastCalibration = DateTime.Now;
            await _service.SaveMsaAsync(SelectedItem);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"校准失败: {ex.Message}";
        }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        StatusFilter = "All";
        _ = LoadDataAsync();
    }

    private void ApplyFilter() => FilteredView.Refresh();

    private bool FilterPredicate(object obj)
    {
        if (obj is not MsaItem item) return false;
        var matchSearch = string.IsNullOrWhiteSpace(SearchText) ||
            item.GaugeName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Parameter.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var matchStatus = StatusFilter == "All" || item.Status == StatusFilter;
        return matchSearch && matchStatus;
    }

    private void UpdateStats()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(AcceptableCount));
        RaisePropertyChanged(nameof(ConditionalCount));
        RaisePropertyChanged(nameof(UnacceptableCount));
        RaisePropertyChanged(nameof(AvgGRR));
        RaisePropertyChanged(nameof(OverdueCalibrationCount));
    }

    private void RaiseCanExecuteChanged()
    {
        SaveCommand.RaiseCanExecuteChanged();
        DeleteCommand.RaiseCanExecuteChanged();
        CalibrateCommand.RaiseCanExecuteChanged();
    }
}
