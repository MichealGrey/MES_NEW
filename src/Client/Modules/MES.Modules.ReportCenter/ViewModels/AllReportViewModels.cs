using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Modules.ReportCenter.Models;
using MES.Modules.ReportCenter.Services;

namespace MES.Modules.ReportCenter.ViewModels;

public class DashboardViewModel : BindableBase
{
    private readonly IReportService _service;
    private DashboardSummary? _summary;
    private string? _errorMessage;
    private bool _isLoading;

    public DashboardSummary? Summary { get => _summary; set => SetProperty(ref _summary, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public DelegateCommand RefreshCommand { get; }

    public DashboardViewModel(IReportService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await LoadDashboardAsync(); }
        catch (Exception ex) { ErrorMessage = $"仪表板数据加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task LoadDashboardAsync()
    {
        Summary = await _service.GetDashboardSummaryAsync();
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await LoadDashboardAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }
}

public class YieldReportViewModel : BindableBase
{
    private readonly IReportService _service;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<YieldReportItem> _yieldReports = [];
    private ObservableCollection<YieldTrendPoint> _yieldTrend = [];
    private ObservableCollection<DefectAnalysisItem> _defectAnalysis = [];
    private YieldReportItem? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private DateTime? _startDate;
    private DateTime? _endDate;
    private string? _filterProduct;
    private string? _filterProcessStep;
    private bool _isLoading;

    public ObservableCollection<YieldReportItem> YieldReports { get => _yieldReports; set => SetProperty(ref _yieldReports, value); }
    public ObservableCollection<YieldTrendPoint> YieldTrend { get => _yieldTrend; set => SetProperty(ref _yieldTrend, value); }
    public ObservableCollection<DefectAnalysisItem> DefectAnalysis { get => _defectAnalysis; set => SetProperty(ref _defectAnalysis, value); }
    public YieldReportItem? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) RaiseCanExecuteChanged(); } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public DateTime? StartDate { get => _startDate; set { if (SetProperty(ref _startDate, value)) ApplyFilter(); } }
    public DateTime? EndDate { get => _endDate; set { if (SetProperty(ref _endDate, value)) ApplyFilter(); } }
    public string? FilterProduct { get => _filterProduct; set { if (SetProperty(ref _filterProduct, value)) ApplyFilter(); } }
    public string? FilterProcessStep { get => _filterProcessStep; set { if (SetProperty(ref _filterProcessStep, value)) ApplyFilter(); } }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public double AvgYieldRate => YieldReports.Any() ? Math.Round(YieldReports.Average(y => y.YieldRate), 2) : 0;
    public double MinYieldRate => YieldReports.Any() ? YieldReports.Min(y => y.YieldRate) : 0;
    public double MaxYieldRate => YieldReports.Any() ? YieldReports.Max(y => y.YieldRate) : 0;
    public int BelowTargetCount => YieldReports.Count(y => y.IsBelowTarget);
    public int TotalScrapQty => YieldReports.Sum(y => y.ScrapQty);

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<YieldReportItem?> ViewDetailCommand { get; }
    public DelegateCommand LoadTrendCommand { get; }

    public YieldReportViewModel(IReportService service, IRegionManager regionManager)
    {
        _service = service;
        _regionManager = regionManager;
        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        ViewDetailCommand = new DelegateCommand<YieldReportItem?>(OnViewDetail, y => y != null);
        LoadTrendCommand = new DelegateCommand(OnLoadTrend);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"良率报表加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task ReloadDataAsync()
    {
        var filter = BuildFilter();
        var reports = await _service.GetYieldReportsAsync(filter);
        YieldReports = new ObservableCollection<YieldReportItem>(reports);
        UpdateStatistics();

        var trend = await _service.GetYieldTrendAsync(filter?.ProductId);
        YieldTrend = new ObservableCollection<YieldTrendPoint>(trend);

        var defects = await _service.GetDefectAnalysisAsync(filter);
        DefectAnalysis = new ObservableCollection<DefectAnalysisItem>(defects);
    }

    private ReportFilter? BuildFilter()
    {
        if (StartDate == null && EndDate == null && string.IsNullOrWhiteSpace(FilterProduct) &&
            string.IsNullOrWhiteSpace(FilterProcessStep))
            return null;

        return new ReportFilter
        {
            StartDate = StartDate,
            EndDate = EndDate,
            ProductId = FilterProduct,
            ProcessStep = FilterProcessStep
        };
    }

    private void ApplyFilter()
    {
        var filter = BuildFilter();
        var query = YieldReports.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var k = SearchText.Trim().ToLower();
            query = query.Where(y => y.ProductName.ToLower().Contains(k) || y.LotNo.ToLower().Contains(k) || y.TopDefectType.ToLower().Contains(k));
        }

        YieldReports = new ObservableCollection<YieldReportItem>(query.OrderByDescending(y => y.ReportDate));
        UpdateStatistics();
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void OnLoadTrend()
    {
        try
        {
            var trend = await _service.GetYieldTrendAsync(FilterProduct);
            YieldTrend = new ObservableCollection<YieldTrendPoint>(trend);
        }
        catch (Exception ex) { ErrorMessage = $"趋势数据加载失败: {ex.Message}"; }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        StartDate = null;
        EndDate = null;
        FilterProduct = null;
        FilterProcessStep = null;
        _ = ReloadDataAsync();
    }

    private void OnViewDetail(YieldReportItem? y)
    {
        if (y == null) return;
        var parameters = new NavigationParameters
        {
            { "LotNo", y.LotNo }
        };
        _regionManager.RequestNavigate("MainContentRegion", "YieldReportDetailView", parameters);
    }

    private void RaiseCanExecuteChanged() { ViewDetailCommand.RaiseCanExecuteChanged(); }
    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(AvgYieldRate));
        RaisePropertyChanged(nameof(MinYieldRate));
        RaisePropertyChanged(nameof(MaxYieldRate));
        RaisePropertyChanged(nameof(BelowTargetCount));
        RaisePropertyChanged(nameof(TotalScrapQty));
    }
}

public class QualityReportViewModel : BindableBase
{
    private readonly IReportService _service;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<QualityReportItem> _qualityReports = [];
    private QualityStatistics? _statistics;
    private QualityReportItem? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private DateTime? _startDate;
    private DateTime? _endDate;
    private string? _filterSeverity;
    private string? _filterProcessStep;
    private bool _isLoading;

    public ObservableCollection<QualityReportItem> QualityReports { get => _qualityReports; set => SetProperty(ref _qualityReports, value); }
    public QualityStatistics? Statistics { get => _statistics; set => SetProperty(ref _statistics, value); }
    public QualityReportItem? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) RaiseCanExecuteChanged(); } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public DateTime? StartDate { get => _startDate; set { if (SetProperty(ref _startDate, value)) ApplyFilter(); } }
    public DateTime? EndDate { get => _endDate; set { if (SetProperty(ref _endDate, value)) ApplyFilter(); } }
    public string? FilterSeverity { get => _filterSeverity; set { if (SetProperty(ref _filterSeverity, value)) ApplyFilter(); } }
    public string? FilterProcessStep { get => _filterProcessStep; set { if (SetProperty(ref _filterProcessStep, value)) ApplyFilter(); } }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int TotalCount => QualityReports.Count;
    public int CriticalCount => QualityReports.Count(q => q.Severity == "Critical");
    public int MajorCount => QualityReports.Count(q => q.Severity == "Major");
    public int MinorCount => QualityReports.Count(q => q.Severity == "Minor");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<QualityReportItem?> ViewDetailCommand { get; }
    public DelegateCommand LoadStatisticsCommand { get; }

    public QualityReportViewModel(IReportService service, IRegionManager regionManager)
    {
        _service = service;
        _regionManager = regionManager;
        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        ViewDetailCommand = new DelegateCommand<QualityReportItem?>(OnViewDetail, q => q != null);
        LoadStatisticsCommand = new DelegateCommand(OnLoadStatistics);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"质量报表加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task ReloadDataAsync()
    {
        var filter = BuildFilter();
        var reports = await _service.GetQualityReportsAsync(filter);
        QualityReports = new ObservableCollection<QualityReportItem>(reports);
        UpdateStatistics();

        Statistics = await _service.GetQualityStatisticsAsync(filter);
    }

    private ReportFilter? BuildFilter()
    {
        if (StartDate == null && EndDate == null && string.IsNullOrWhiteSpace(FilterSeverity) &&
            string.IsNullOrWhiteSpace(FilterProcessStep))
            return null;

        return new ReportFilter
        {
            StartDate = StartDate,
            EndDate = EndDate,
            Status = FilterSeverity,
            ProcessStep = FilterProcessStep
        };
    }

    private void ApplyFilter()
    {
        var filter = BuildFilter();
        var query = QualityReports.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var k = SearchText.Trim().ToLower();
            query = query.Where(q => q.ProductName.ToLower().Contains(k) || q.LotNo.ToLower().Contains(k) ||
                                     q.DefectDescription.ToLower().Contains(k) || q.Inspector.ToLower().Contains(k));
        }

        QualityReports = new ObservableCollection<QualityReportItem>(query.OrderByDescending(q => q.ReportDate));
        UpdateStatistics();
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void OnLoadStatistics()
    {
        try { Statistics = await _service.GetQualityStatisticsAsync(BuildFilter()); }
        catch (Exception ex) { ErrorMessage = $"统计数据加载失败: {ex.Message}"; }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        StartDate = null;
        EndDate = null;
        FilterSeverity = null;
        FilterProcessStep = null;
        _ = ReloadDataAsync();
    }

    private void OnViewDetail(QualityReportItem? q)
    {
        if (q == null) return;
        var parameters = new NavigationParameters
        {
            { "InspectionNo", q.InspectionNo }
        };
        _regionManager.RequestNavigate("MainContentRegion", "QualityReportDetailView", parameters);
    }

    private void RaiseCanExecuteChanged() { ViewDetailCommand.RaiseCanExecuteChanged(); }
    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(CriticalCount));
        RaisePropertyChanged(nameof(MajorCount));
        RaisePropertyChanged(nameof(MinorCount));
    }
}

public class ProductionReportViewModel : BindableBase
{
    private readonly IReportService _service;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<ProductionDailyReport> _productionReports = [];
    private ProductionDailyReport? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private DateTime? _startDate;
    private DateTime? _endDate;
    private string? _filterProduct;
    private string? _filterShift;
    private string? _filterProcessStep;
    private bool _isLoading;

    public ObservableCollection<ProductionDailyReport> ProductionReports { get => _productionReports; set => SetProperty(ref _productionReports, value); }
    public ProductionDailyReport? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) RaiseCanExecuteChanged(); } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public DateTime? StartDate { get => _startDate; set { if (SetProperty(ref _startDate, value)) ApplyFilter(); } }
    public DateTime? EndDate { get => _endDate; set { if (SetProperty(ref _endDate, value)) ApplyFilter(); } }
    public string? FilterProduct { get => _filterProduct; set { if (SetProperty(ref _filterProduct, value)) ApplyFilter(); } }
    public string? FilterShift { get => _filterShift; set { if (SetProperty(ref _filterShift, value)) ApplyFilter(); } }
    public string? FilterProcessStep { get => _filterProcessStep; set { if (SetProperty(ref _filterProcessStep, value)) ApplyFilter(); } }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int TotalReports => ProductionReports.Count;
    public double AvgYieldRate => ProductionReports.Any() ? Math.Round(ProductionReports.Average(p => p.YieldRate), 2) : 0;
    public double AvgEfficiency => ProductionReports.Any() ? Math.Round(ProductionReports.Average(p => p.Efficiency), 2) : 0;
    public int TotalInputQty => ProductionReports.Sum(p => p.InputQty);
    public int TotalGoodQty => ProductionReports.Sum(p => p.GoodQty);
    public int TotalScrapQty => ProductionReports.Sum(p => p.ScrapQty);

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<ProductionDailyReport?> ViewDetailCommand { get; }

    public ProductionReportViewModel(IReportService service, IRegionManager regionManager)
    {
        _service = service;
        _regionManager = regionManager;
        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        ViewDetailCommand = new DelegateCommand<ProductionDailyReport?>(OnViewDetail, p => p != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"生产报表加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task ReloadDataAsync()
    {
        var filter = BuildFilter();
        var reports = await _service.GetProductionReportsAsync(filter);
        ProductionReports = new ObservableCollection<ProductionDailyReport>(reports);
        UpdateStatistics();
    }

    private ReportFilter? BuildFilter()
    {
        if (StartDate == null && EndDate == null && string.IsNullOrWhiteSpace(FilterProduct) &&
            string.IsNullOrWhiteSpace(FilterShift) && string.IsNullOrWhiteSpace(FilterProcessStep))
            return null;

        return new ReportFilter
        {
            StartDate = StartDate,
            EndDate = EndDate,
            ProductId = FilterProduct,
            Shift = FilterShift,
            ProcessStep = FilterProcessStep
        };
    }

    private void ApplyFilter()
    {
        var filter = BuildFilter();
        var query = ProductionReports.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var k = SearchText.Trim().ToLower();
            query = query.Where(p => p.ProductName.ToLower().Contains(k) || p.LotNo.ToLower().Contains(k) ||
                                     p.WorkOrderNo.ToLower().Contains(k) || p.Operator.ToLower().Contains(k));
        }

        ProductionReports = new ObservableCollection<ProductionDailyReport>(query.OrderByDescending(p => p.ReportDate));
        UpdateStatistics();
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        StartDate = null;
        EndDate = null;
        FilterProduct = null;
        FilterShift = null;
        FilterProcessStep = null;
        _ = ReloadDataAsync();
    }

    private void OnViewDetail(ProductionDailyReport? p)
    {
        if (p == null) return;
        var parameters = new NavigationParameters
        {
            { "WorkOrderNo", p.WorkOrderNo }
        };
        _regionManager.RequestNavigate("MainContentRegion", "ProductionReportDetailView", parameters);
    }

    private void RaiseCanExecuteChanged() { ViewDetailCommand.RaiseCanExecuteChanged(); }
    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalReports));
        RaisePropertyChanged(nameof(AvgYieldRate));
        RaisePropertyChanged(nameof(AvgEfficiency));
        RaisePropertyChanged(nameof(TotalInputQty));
        RaisePropertyChanged(nameof(TotalGoodQty));
        RaisePropertyChanged(nameof(TotalScrapQty));
    }
}

public class LotGenealogyReportViewModel : BindableBase
{
    private readonly IReportService _service;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<LotGenealogyReportItem> _genealogyReports = [];
    private LotGenealogyReportItem? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isLoading;

    public ObservableCollection<LotGenealogyReportItem> GenealogyReports { get => _genealogyReports; set => SetProperty(ref _genealogyReports, value); }
    public LotGenealogyReportItem? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) RaiseCanExecuteChanged(); } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int TotalLots => GenealogyReports.GroupBy(g => g.LotNo).Count();
    public int CompletedCount => GenealogyReports.Count(g => g.Status == "Completed");
    public int InProgressCount => GenealogyReports.Count(g => g.Status == "InProgress");
    public int SplitCount => GenealogyReports.Count(g => g.SplitMergeType == "分批");
    public int MergeCount => GenealogyReports.Count(g => g.SplitMergeType == "合批");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<LotGenealogyReportItem?> ViewDetailCommand { get; }

    public LotGenealogyReportViewModel(IReportService service, IRegionManager regionManager)
    {
        _service = service;
        _regionManager = regionManager;
        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        ViewDetailCommand = new DelegateCommand<LotGenealogyReportItem?>(OnViewDetail, g => g != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"批次追溯报表加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task ReloadDataAsync()
    {
        var lotNo = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText.Trim();
        var reports = await _service.GetLotGenealogyAsync(lotNo);
        GenealogyReports = new ObservableCollection<LotGenealogyReportItem>(reports);
        UpdateStatistics();
    }

    private void ApplyFilter()
    {
        _ = ReloadDataAsync();
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        _ = ReloadDataAsync();
    }

    private void OnViewDetail(LotGenealogyReportItem? g)
    {
        if (g == null) return;
        var parameters = new NavigationParameters
        {
            { "LotNo", g.LotNo }
        };
        _regionManager.RequestNavigate("MainContentRegion", "LotGenealogyReportDetailView", parameters);
    }

    private void RaiseCanExecuteChanged() { ViewDetailCommand.RaiseCanExecuteChanged(); }
    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalLots));
        RaisePropertyChanged(nameof(CompletedCount));
        RaisePropertyChanged(nameof(InProgressCount));
        RaisePropertyChanged(nameof(SplitCount));
        RaisePropertyChanged(nameof(MergeCount));
    }
}

public class EquipmentReportViewModel : BindableBase
{
    private readonly IReportService _service;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<EquipmentReportItem> _equipmentReports = [];
    private ObservableCollection<EquipmentTrendPoint> _equipmentTrend = [];
    private EquipmentReportItem? _selectedItem;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private DateTime? _startDate;
    private DateTime? _endDate;
    private string? _filterEquipmentNo;
    private string? _filterStatus;
    private bool _isLoading;

    public ObservableCollection<EquipmentReportItem> EquipmentReports { get => _equipmentReports; set => SetProperty(ref _equipmentReports, value); }
    public ObservableCollection<EquipmentTrendPoint> EquipmentTrend { get => _equipmentTrend; set => SetProperty(ref _equipmentTrend, value); }
    public EquipmentReportItem? SelectedItem { get => _selectedItem; set { if (SetProperty(ref _selectedItem, value)) RaiseCanExecuteChanged(); } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public DateTime? StartDate { get => _startDate; set { if (SetProperty(ref _startDate, value)) ApplyFilter(); } }
    public DateTime? EndDate { get => _endDate; set { if (SetProperty(ref _endDate, value)) ApplyFilter(); } }
    public string? FilterEquipmentNo { get => _filterEquipmentNo; set { if (SetProperty(ref _filterEquipmentNo, value)) ApplyFilter(); } }
    public string? FilterStatus { get => _filterStatus; set { if (SetProperty(ref _filterStatus, value)) ApplyFilter(); } }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public int TotalEquipment => EquipmentReports.Count;
    public int RunningCount => EquipmentReports.Count(e => e.Status == "Running");
    public int IdleCount => EquipmentReports.Count(e => e.Status == "Idle");
    public double AvgOEE => EquipmentReports.Any() ? Math.Round(EquipmentReports.Average(e => e.OEE), 2) : 0;
    public double AvgAvailability => EquipmentReports.Any() ? Math.Round(EquipmentReports.Average(e => e.Availability), 2) : 0;
    public double AvgPerformance => EquipmentReports.Any() ? Math.Round(EquipmentReports.Average(e => e.Performance), 2) : 0;
    public int TotalAlarmCount => EquipmentReports.Sum(e => e.AlarmCount);

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<EquipmentReportItem?> ViewDetailCommand { get; }
    public DelegateCommand LoadTrendCommand { get; }

    public EquipmentReportViewModel(IReportService service, IRegionManager regionManager)
    {
        _service = service;
        _regionManager = regionManager;
        RefreshCommand = new DelegateCommand(OnRefresh);
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        ViewDetailCommand = new DelegateCommand<EquipmentReportItem?>(OnViewDetail, e => e != null);
        LoadTrendCommand = new DelegateCommand(OnLoadTrend);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"设备报表加载失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async Task ReloadDataAsync()
    {
        var filter = BuildFilter();
        var reports = await _service.GetEquipmentReportsAsync(filter);
        EquipmentReports = new ObservableCollection<EquipmentReportItem>(reports);
        UpdateStatistics();
    }

    private ReportFilter? BuildFilter()
    {
        if (StartDate == null && EndDate == null && string.IsNullOrWhiteSpace(FilterEquipmentNo) &&
            string.IsNullOrWhiteSpace(FilterStatus))
            return null;

        return new ReportFilter
        {
            StartDate = StartDate,
            EndDate = EndDate,
            EquipmentNo = FilterEquipmentNo,
            Status = FilterStatus
        };
    }

    private void ApplyFilter()
    {
        var filter = BuildFilter();
        var query = EquipmentReports.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var k = SearchText.Trim().ToLower();
            query = query.Where(e => e.EquipmentName.ToLower().Contains(k) || e.EquipmentNo.ToLower().Contains(k));
        }

        EquipmentReports = new ObservableCollection<EquipmentReportItem>(query.OrderByDescending(e => e.ReportDate));
        UpdateStatistics();
    }

    private async void OnRefresh()
    {
        try { IsLoading = true; ErrorMessage = null; await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    private async void OnLoadTrend()
    {
        try
        {
            var trend = await _service.GetEquipmentTrendAsync(FilterEquipmentNo);
            EquipmentTrend = new ObservableCollection<EquipmentTrendPoint>(trend);
        }
        catch (Exception ex) { ErrorMessage = $"趋势数据加载失败: {ex.Message}"; }
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        StartDate = null;
        EndDate = null;
        FilterEquipmentNo = null;
        FilterStatus = null;
        _ = ReloadDataAsync();
    }

    private void OnViewDetail(EquipmentReportItem? e)
    {
        if (e == null) return;
        var parameters = new NavigationParameters
        {
            { "EquipmentNo", e.EquipmentNo }
        };
        _regionManager.RequestNavigate("MainContentRegion", "EquipmentReportDetailView", parameters);
    }

    private void RaiseCanExecuteChanged() { ViewDetailCommand.RaiseCanExecuteChanged(); }
    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalEquipment));
        RaisePropertyChanged(nameof(RunningCount));
        RaisePropertyChanged(nameof(IdleCount));
        RaisePropertyChanged(nameof(AvgOEE));
        RaisePropertyChanged(nameof(AvgAvailability));
        RaisePropertyChanged(nameof(AvgPerformance));
        RaisePropertyChanged(nameof(TotalAlarmCount));
    }
}
