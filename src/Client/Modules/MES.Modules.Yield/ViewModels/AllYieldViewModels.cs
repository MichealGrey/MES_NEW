using Prism.Mvvm;
using Prism.Commands;
using MES.Modules.Yield.Models;
using MES.Modules.Yield.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace MES.Modules.Yield.ViewModels;

public class YieldTrendViewModel : BindableBase
{
    private readonly IYieldService _service;
    private ObservableCollection<YieldTrendItem> _items = [];
    private YieldTrendItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private double _avgYield;
    private ICollectionView _collectionView = null!;
    private string _productFilter = string.Empty;

    public ObservableCollection<YieldTrendItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public YieldTrendItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public double AvgYield { get => _avgYield; set => SetProperty(ref _avgYield, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string ProductFilter { get => _productFilter; set { SetProperty(ref _productFilter, value); ApplyFilter(); } }

    public int TotalRecords => Items.Count;
    public int AboveTargetCount => Items.Count(x => x.Yield >= x.TargetYield);
    public int BelowTargetCount => Items.Count(x => x.Yield < x.TargetYield);

    public DelegateCommand RefreshCommand { get; }

    public YieldTrendViewModel(IYieldService service)
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
            Items = new ObservableCollection<YieldTrendItem>(await _service.GetYieldTrendAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Filter = FilterTrend;
            CollectionView.Refresh();
            AvgYield = Items.Any() ? Items.Average(x => x.Yield) : 0;
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private bool FilterTrend(object obj)
    {
        if (obj is not YieldTrendItem item) return false;
        return string.IsNullOrEmpty(ProductFilter) || item.Product.Contains(ProductFilter, StringComparison.OrdinalIgnoreCase);
    }

    private void ApplyFilter()
    {
        CollectionView?.Refresh();
        AvgYield = Items.Any() ? Items.Where(x => string.IsNullOrEmpty(ProductFilter) || x.Product.Contains(ProductFilter, StringComparison.OrdinalIgnoreCase)).Average(x => x.Yield) : 0;
    }
}

public class YieldDashboardViewModel : BindableBase
{
    private readonly IYieldService _service;
    private ObservableCollection<YieldKpiItem> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;

    public ObservableCollection<YieldKpiItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }

    public double OverallAvg => Items.Any() ? Items.Average(x => x.CurrentValue) : 0;
    public int AboveTargetCount => Items.Count(x => x.CurrentValue >= x.TargetValue);
    public int BelowTargetCount => Items.Count(x => x.CurrentValue < x.TargetValue);
    public int UpTrendCount => Items.Count(x => x.Trend == "Up");
    public int DownTrendCount => Items.Count(x => x.Trend == "Down");

    public DelegateCommand RefreshCommand { get; }

    public YieldDashboardViewModel(IYieldService service)
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
            Items = new ObservableCollection<YieldKpiItem>(await _service.GetDashboardKpisAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}

public class YieldAnalysisViewModel : BindableBase
{
    private readonly IYieldService _service;
    private ObservableCollection<YieldKpiItem> _items = [];
    private string _selectedProduct = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;

    public ObservableCollection<YieldKpiItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string SelectedProduct { get => _selectedProduct; set => SetProperty(ref _selectedProduct, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }

    public double AvgYield => Items.Any() ? Items.Average(x => x.CurrentValue) : 0;
    public double MinYield => Items.Any() ? Items.Min(x => x.CurrentValue) : 0;
    public double MaxYield => Items.Any() ? Items.Max(x => x.CurrentValue) : 0;

    public DelegateCommand AnalyzeCommand { get; }

    public YieldAnalysisViewModel(IYieldService service)
    {
        _service = service;
        AnalyzeCommand = new DelegateCommand(ExecuteAnalyze, CanExecuteAnalyze);
        _ = LoadDataAsync();
    }

    private bool CanExecuteAnalyze() => !string.IsNullOrEmpty(SelectedProduct);

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<YieldKpiItem>(await _service.GetYieldAnalysisAsync(string.Empty));
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void ExecuteAnalyze()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<YieldKpiItem>(await _service.GetYieldAnalysisAsync(SelectedProduct));
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"分析失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}

public class WaferMapViewModel : BindableBase
{
    private readonly IYieldService _service;
    private ObservableCollection<WaferDieItem> _items = [];
    private string _selectedWaferId = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;

    public ObservableCollection<WaferDieItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string SelectedWaferId { get => _selectedWaferId; set => SetProperty(ref _selectedWaferId, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }

    public int TotalDie => Items.Count;
    public int GoodDie => Items.Count(x => x.Result == "Good");
    public int FailDie => Items.Count(x => x.Result == "Fail");
    public int MarginalDie => Items.Count(x => x.Result == "Marginal");
    public double WaferYield => Items.Any() ? (double)GoodDie / Items.Count * 100 : 0;

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand LoadWaferCommand { get; }

    public WaferMapViewModel(IYieldService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        LoadWaferCommand = new DelegateCommand(ExecuteLoadWafer, CanExecuteLoadWafer);
        _ = LoadDataAsync();
    }

    private bool CanExecuteLoadWafer() => !string.IsNullOrEmpty(SelectedWaferId);

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<WaferDieItem>(await _service.GetWaferMapAsync("WF-001"));
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void ExecuteLoadWafer()
    {
        try
        {
            IsLoading = true;
            Items = new ObservableCollection<WaferDieItem>(await _service.GetWaferMapAsync(SelectedWaferId));
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}

public class BinAnalysisViewModel : BindableBase
{
    private readonly IYieldService _service;
    private ObservableCollection<WaferDieItem> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;
    private int _goodCount;
    private int _failCount;
    private double _yieldPercent;

    public ObservableCollection<WaferDieItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public int GoodCount { get => _goodCount; set => SetProperty(ref _goodCount, value); }
    public int FailCount { get => _failCount; set => SetProperty(ref _failCount, value); }
    public double YieldPercent { get => _yieldPercent; set => SetProperty(ref _yieldPercent, value); }

    public int MarginalCount => Items.Count(x => x.Result == "Marginal");
    public int TotalDie => Items.Count;
    public Dictionary<string, int> BinDistribution => Items
        .GroupBy(x => x.BinCode)
        .ToDictionary(g => g.Key, g => g.Count());

    public DelegateCommand RefreshCommand { get; }

    public BinAnalysisViewModel(IYieldService service)
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
            Items = new ObservableCollection<WaferDieItem>(await _service.GetBinAnalysisAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Refresh();
            GoodCount = Items.Count(x => x.Result == "Good");
            FailCount = Items.Count(x => x.Result == "Fail");
            YieldPercent = Items.Any() ? (double)GoodCount / Items.Count * 100 : 0;
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}

public class DefectAnalysisViewModel : BindableBase
{
    private readonly IYieldService _service;
    private ObservableCollection<WaferDieItem> _items = [];
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private ICollectionView _collectionView = null!;
    private string _defectTypeFilter = string.Empty;

    public ObservableCollection<WaferDieItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public ICollectionView CollectionView { get => _collectionView; set => SetProperty(ref _collectionView, value); }
    public string DefectTypeFilter { get => _defectTypeFilter; set { SetProperty(ref _defectTypeFilter, value); ApplyFilter(); } }

    public int TotalDefects => Items.Count;
    public Dictionary<string, int> DefectDistribution => Items
        .GroupBy(x => x.DefectType)
        .ToDictionary(g => g.Key, g => g.Count());
    public string TopDefectType => DefectDistribution.Any()
        ? DefectDistribution.OrderByDescending(x => x.Value).First().Key
        : string.Empty;

    public DelegateCommand RefreshCommand { get; }

    public DefectAnalysisViewModel(IYieldService service)
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
            Items = new ObservableCollection<WaferDieItem>(await _service.GetDefectAnalysisAsync());
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.Filter = FilterDefects;
            CollectionView.Refresh();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private bool FilterDefects(object obj)
    {
        if (obj is not WaferDieItem item) return false;
        return string.IsNullOrEmpty(DefectTypeFilter) || item.DefectType.Contains(DefectTypeFilter, StringComparison.OrdinalIgnoreCase);
    }

    private void ApplyFilter() => CollectionView?.Refresh();
}
