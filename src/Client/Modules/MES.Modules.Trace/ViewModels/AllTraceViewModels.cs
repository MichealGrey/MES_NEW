using Prism.Mvvm;
using Prism.Commands;
using MES.Modules.Trace.Models;
using MES.Modules.Trace.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace MES.Modules.Trace.ViewModels;

public class LotTraceViewModel : BindableBase
{
    private readonly ITraceService _service;
    private ObservableCollection<LotTraceItem> _items = [];
    private LotTraceItem? _selectedItem;
    private string _searchText = string.Empty;
    private string _statusFilter = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;

    public ObservableCollection<LotTraceItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public LotTraceItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilter(); } }
    public string StatusFilter { get => _statusFilter; set { SetProperty(ref _statusFilter, value); ApplyFilter(); } }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int TotalCount => _collectionView?.Cast<object>().Count() ?? 0;
    public int InProcessCount => Items.Count(x => x.Status == "InProcess");
    public int CompletedCount => Items.Count(x => x.Status == "Completed");
    public int HoldCount => Items.Count(x => x.Status == "Hold");

    public DelegateCommand SearchCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }

    public LotTraceViewModel(ITraceService service)
    {
        _service = service;
        SearchCommand = new DelegateCommand(ApplyFilter);
        ClearFilterCommand = new DelegateCommand(ClearFilter);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<LotTraceItem>(await _service.GetLotTracesAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Filter = FilterLotTrace;
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private bool FilterLotTrace(object obj)
    {
        if (obj is not LotTraceItem item) return false;
        var matchSearch = string.IsNullOrEmpty(SearchText) ||
            item.LotId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Product.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            item.Customer.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        var matchStatus = string.IsNullOrEmpty(StatusFilter) || item.Status == StatusFilter;
        return matchSearch && matchStatus;
    }

    private void ApplyFilter() => CollectionView?.Refresh();
    private void ClearFilter() { SearchText = string.Empty; StatusFilter = string.Empty; }
}

public class ForwardTraceViewModel : BindableBase
{
    private readonly ITraceService _service;
    private ObservableCollection<ForwardTraceItem> _items = [];
    private ForwardTraceItem? _selectedItem;
    private string _searchLotId = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;

    public ObservableCollection<ForwardTraceItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public ForwardTraceItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string SearchLotId { get => _searchLotId; set => SetProperty(ref _searchLotId, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int TotalSteps => Items.Count;
    public int PassCount => Items.Count(x => x.Result == "Pass");
    public int FailCount => Items.Count(x => x.Result == "Fail");
    public double PassRate => Items.Any() ? (double)PassCount / Items.Count * 100 : 0;

    public DelegateCommand TraceCommand { get; }
    public DelegateCommand<string> TraceByLotCommand { get; }

    public ForwardTraceViewModel(ITraceService service)
    {
        _service = service;
        TraceCommand = new DelegateCommand(ExecuteTrace, CanExecuteTrace);
        TraceByLotCommand = new DelegateCommand<string>(ExecuteTraceByLot);
        _ = LoadDataAsync();
    }

    private bool CanExecuteTrace() => !string.IsNullOrEmpty(SearchLotId);

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<ForwardTraceItem>(await _service.GetForwardTraceAsync(string.Empty));
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void ExecuteTrace()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<ForwardTraceItem>(await _service.GetForwardTraceAsync(SearchLotId));
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"追踪失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void ExecuteTraceByLot(string lotId)
    {
        if (string.IsNullOrEmpty(lotId)) return;
        SearchLotId = lotId;
        ExecuteTrace();
    }
}

public class BackwardTraceViewModel : BindableBase
{
    private readonly ITraceService _service;
    private ObservableCollection<BackwardTraceItem> _items = [];
    private string _searchLotId = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;

    public ObservableCollection<BackwardTraceItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string SearchLotId { get => _searchLotId; set => SetProperty(ref _searchLotId, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int MaterialCount => Items.Count;
    public int PassMaterialCount => Items.Count(x => x.InspectionResult == "Pass");

    public DelegateCommand SearchCommand { get; }

    public BackwardTraceViewModel(ITraceService service)
    {
        _service = service;
        SearchCommand = new DelegateCommand(ExecuteSearch, CanExecuteSearch);
        _ = LoadDataAsync();
    }

    private bool CanExecuteSearch() => !string.IsNullOrEmpty(SearchLotId);

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<BackwardTraceItem>(await _service.GetBackwardTraceAsync("LOT-2024-001"));
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void ExecuteSearch()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<BackwardTraceItem>(await _service.GetBackwardTraceAsync(SearchLotId));
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"搜索失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}

public class GenealogyViewModel : BindableBase
{
    private readonly ITraceService _service;
    private ObservableCollection<GenealogyItem> _items = [];
    private string _searchLotId = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;

    public ObservableCollection<GenealogyItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string SearchLotId { get => _searchLotId; set => SetProperty(ref _searchLotId, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int ParentLotCount => Items.Count(x => !string.IsNullOrEmpty(x.ParentLotId));
    public int ChildLotCount => Items.Count(x => !string.IsNullOrEmpty(x.ChildLotId));

    public DelegateCommand SearchCommand { get; }

    public GenealogyViewModel(ITraceService service)
    {
        _service = service;
        SearchCommand = new DelegateCommand(ExecuteSearch, CanExecuteSearch);
        _ = LoadDataAsync();
    }

    private bool CanExecuteSearch() => !string.IsNullOrEmpty(SearchLotId);

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<GenealogyItem>(await _service.GetGenealogyAsync("LOT-2024-001"));
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void ExecuteSearch()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<GenealogyItem>(await _service.GetGenealogyAsync(SearchLotId));
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"搜索失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}

public class ImpactAnalysisViewModel : BindableBase
{
    private readonly ITraceService _service;
    private ObservableCollection<ImpactAnalysisItem> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;
    private string _impactTypeFilter = string.Empty;

    public ObservableCollection<ImpactAnalysisItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string ImpactTypeFilter { get => _impactTypeFilter; set { SetProperty(ref _impactTypeFilter, value); CollectionView?.Refresh(); } }

    public int TotalAffected => Items.Sum(x => x.AffectedQty);
    public int AnalysisCount => Items.Count;
    public int MaterialImpactCount => Items.Count(x => x.ImpactType == "Material");
    public int EquipmentImpactCount => Items.Count(x => x.ImpactType == "Equipment");

    public DelegateCommand AnalyzeCommand { get; }

    public ImpactAnalysisViewModel(ITraceService service)
    {
        _service = service;
        AnalyzeCommand = new DelegateCommand(ExecuteAnalyze);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<ImpactAnalysisItem>(await _service.GetImpactAnalysisAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void ExecuteAnalyze() => CollectionView?.Refresh();
}

public class CustomerTraceReportViewModel : BindableBase
{
    private readonly ITraceService _service;
    private ObservableCollection<CustomerTraceReport> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;
    private string _customerFilter = string.Empty;

    public ObservableCollection<CustomerTraceReport> Items { get => _items; set => SetProperty(ref _items, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string CustomerFilter { get => _customerFilter; set { SetProperty(ref _customerFilter, value); ApplyFilter(); } }

    public int TotalReports => Items.Count;
    public int CompleteCount => Items.Count(x => x.TraceResult == "Complete");
    public int TotalQtyReported => Items.Sum(x => x.TotalQty);

    public DelegateCommand GenerateCommand { get; }

    public CustomerTraceReportViewModel(ITraceService service)
    {
        _service = service;
        GenerateCommand = new DelegateCommand(ExecuteGenerate);
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<CustomerTraceReport>(await _service.GetCustomerReportsAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Filter = FilterReports;
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private bool FilterReports(object obj)
    {
        if (obj is not CustomerTraceReport item) return false;
        return string.IsNullOrEmpty(CustomerFilter) || item.CustomerName.Contains(CustomerFilter, StringComparison.OrdinalIgnoreCase);
    }

    private void ApplyFilter() => CollectionView?.Refresh();
    private void ExecuteGenerate() => CollectionView?.Refresh();
}
