using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Modules.EngineeringChange.Models;
using MES.Modules.EngineeringChange.Services;

namespace MES.Modules.EngineeringChange.ViewModels;

public class ECNListViewModel : BindableBase
{
    private readonly IECNService _service;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<ECNInfo> _ecns = [];
    private ECNInfo? _selectedECN;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private string? _filterStatus;
    private string? _filterType;

    public ObservableCollection<ECNInfo> ECNs { get => _ecns; set => SetProperty(ref _ecns, value); }
    public ECNInfo? SelectedECN { get => _selectedECN; set { if (SetProperty(ref _selectedECN, value)) RaiseCanExecuteChanged(); } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public string? FilterStatus { get => _filterStatus; set { if (SetProperty(ref _filterStatus, value)) ApplyFilter(); } }
    public string? FilterType { get => _filterType; set { if (SetProperty(ref _filterType, value)) ApplyFilter(); } }

    public int TotalCount => ECNs.Count;
    public int DraftCount => ECNs.Count(e => e.Status == "Draft");
    public int InReviewCount => ECNs.Count(e => e.Status == "Review");
    public int ApprovedCount => ECNs.Count(e => e.Status == "Approval");
    public int RejectedCount => ECNs.Count(e => e.Status == "Rejected");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand<ECNInfo?> ViewDetailCommand { get; }
    public DelegateCommand<ECNInfo?> SubmitCommand { get; }
    public DelegateCommand<ECNInfo?> ApproveCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }

    public ECNListViewModel(IECNService service, IRegionManager regionManager)
    {
        _service = service;
        _regionManager = regionManager;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AddCommand = new DelegateCommand(OnAdd);
        ViewDetailCommand = new DelegateCommand<ECNInfo?>(OnViewDetail, e => e != null);
        SubmitCommand = new DelegateCommand<ECNInfo?>(OnSubmit, e => e?.Status == "Draft");
        ApproveCommand = new DelegateCommand<ECNInfo?>(OnApprove, e => e?.Status == "Approval");
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllECNsAsync(); ECNs = new ObservableCollection<ECNInfo>(items.OrderByDescending(e => e.RequestedAt)); UpdateStatistics(); }

    private void ApplyFilter()
    {
        var query = ECNs.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText)) { var k = SearchText.Trim().ToLower(); query = query.Where(e => e.EcnNo.ToLower().Contains(k) || e.EcnTitle.ToLower().Contains(k) || e.RequestedBy.ToLower().Contains(k) || e.AffectedProducts.ToLower().Contains(k)); }
        if (!string.IsNullOrWhiteSpace(FilterStatus)) query = query.Where(e => e.Status == FilterStatus);
        if (!string.IsNullOrWhiteSpace(FilterType)) query = query.Where(e => e.EcnType == FilterType);
        ECNs = new ObservableCollection<ECNInfo>(query.OrderByDescending(e => e.RequestedAt));
        UpdateStatistics();
    }

    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; FilterStatus = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private async void OnClearFilter() { SearchText = string.Empty; FilterStatus = null; FilterType = null; _ = ReloadDataAsync(); }
    private void OnAdd() { System.Windows.MessageBox.Show("新建ECN功能", "新建", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information); }

    private void OnViewDetail(ECNInfo? e)
    {
        if (e == null) return;
        var parameters = new NavigationParameters { { "ECNId", e.EcnId } };
        _regionManager.RequestNavigate("MainContentRegion", "ECNDetailView", parameters);
    }

    private async void OnSubmit(ECNInfo? e) { if (e == null) return; try { await _service.SubmitForApprovalAsync(e.EcnId); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"提交失败: {ex.Message}"; } }
    private async void OnApprove(ECNInfo? e) { if (e == null) return; try { await _service.ApproveECNAsync(e.EcnId, "当前用户"); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"审批失败: {ex.Message}"; } }

    private void RaiseCanExecuteChanged() { ViewDetailCommand.RaiseCanExecuteChanged(); SubmitCommand.RaiseCanExecuteChanged(); ApproveCommand.RaiseCanExecuteChanged(); }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(DraftCount)); RaisePropertyChanged(nameof(InReviewCount)); RaisePropertyChanged(nameof(ApprovedCount)); RaisePropertyChanged(nameof(RejectedCount)); }
}

public class ECNDetailViewModel : BindableBase
{
    private readonly IECNService _service;
    private ECNInfo? _ecn;
    private ObservableCollection<ECNHistory> _history = [];
    private string? _errorMessage;

    public ECNInfo? ECN { get => _ecn; set => SetProperty(ref _ecn, value); }
    public ObservableCollection<ECNHistory> History { get => _history; set => SetProperty(ref _history, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public DelegateCommand RefreshCommand { get; }

    public ECNDetailViewModel(IECNService service) { _service = service; RefreshCommand = new DelegateCommand(OnRefresh); }

    public async Task LoadECNAsync(string ecnId)
    {
        try { ErrorMessage = null; ECN = await _service.GetECNAsync(ecnId); var hist = await _service.GetECNHistoryAsync(ecnId); History = new ObservableCollection<ECNHistory>(hist.OrderByDescending(h => h.ActionDate)); }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
    }

    private async void OnRefresh() { if (ECN != null) await LoadECNAsync(ECN.EcnId); }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        if (navigationContext.Parameters.TryGetValue("ECNId", out string? ecnId) && !string.IsNullOrEmpty(ecnId))
        {
            _ = LoadECNAsync(ecnId);
        }
    }
}

public class ECNApprovalViewModel : BindableBase
{
    private readonly IECNService _service;
    private ObservableCollection<ECNInfo> _pendingECNs = [];
    private string? _errorMessage;

    public ObservableCollection<ECNInfo> PendingECNs { get => _pendingECNs; set => SetProperty(ref _pendingECNs, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand<ECNInfo?> ApproveCommand { get; }
    public DelegateCommand<ECNInfo?> RejectCommand { get; }

    public ECNApprovalViewModel(IECNService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        ApproveCommand = new DelegateCommand<ECNInfo?>(OnApprove, e => e != null);
        RejectCommand = new DelegateCommand<ECNInfo?>(OnReject, e => e != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllECNsAsync(); PendingECNs = new ObservableCollection<ECNInfo>(items.Where(e => e.Status == "Review" || e.Status == "Approval")); }
    private async void OnRefresh() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private async void OnApprove(ECNInfo? e) { if (e == null) return; try { await _service.ApproveECNAsync(e.EcnId, "当前用户"); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"审批失败: {ex.Message}"; } }
    private async void OnReject(ECNInfo? e) { if (e == null) return; try { await _service.RejectECNAsync(e.EcnId, "不符合要求"); await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"拒绝失败: {ex.Message}"; } }
}

public class ECNHistoryViewModel : BindableBase
{
    private readonly IECNService _service;
    private ObservableCollection<ECNHistory> _allHistory = [];
    private string? _errorMessage;
    private string _searchECNId = string.Empty;

    public ObservableCollection<ECNHistory> AllHistory { get => _allHistory; set => SetProperty(ref _allHistory, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public string SearchECNId { get => _searchECNId; set => SetProperty(ref _searchECNId, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand SearchCommand { get; }

    public ECNHistoryViewModel(IECNService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        SearchCommand = new DelegateCommand(OnSearch);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync()
    {
        var ecns = await _service.GetAllECNsAsync();
        var allHist = new List<ECNHistory>();
        foreach (var ecn in ecns) { var hist = await _service.GetECNHistoryAsync(ecn.EcnId); allHist.AddRange(hist); }
        AllHistory = new ObservableCollection<ECNHistory>(allHist.OrderByDescending(h => h.ActionDate));
    }
    private async void OnRefresh() { try { ErrorMessage = null; SearchECNId = string.Empty; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }

    private async void OnSearch()
    {
        if (string.IsNullOrWhiteSpace(SearchECNId)) { await ReloadDataAsync(); return; }
        var hist = await _service.GetECNHistoryAsync(SearchECNId.Trim());
        AllHistory = new ObservableCollection<ECNHistory>(hist.OrderByDescending(h => h.ActionDate));
    }
}

public class ECNImpactViewModel : BindableBase
{
    private readonly IECNService _service;
    private ObservableCollection<ECNInfo> _approvedECNs = [];
    private string? _errorMessage;

    public ObservableCollection<ECNInfo> ApprovedECNs { get => _approvedECNs; set => SetProperty(ref _approvedECNs, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public int TotalApproved => ApprovedECNs.Count;
    public int AffectedProducts => ApprovedECNs.GroupBy(e => e.AffectedProducts).Count();

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand<ECNInfo?> AnalyzeCommand { get; }

    public ECNImpactViewModel(IECNService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AnalyzeCommand = new DelegateCommand<ECNInfo?>(OnAnalyze, e => e != null);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllECNsAsync(); ApprovedECNs = new ObservableCollection<ECNInfo>(items.Where(e => e.Status == "Close")); UpdateStatistics(); }
    private async void OnRefresh() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }

    private void OnAnalyze(ECNInfo? e)
    {
        if (e == null) return;
        System.Windows.MessageBox.Show($"ECN影响分析\n\n单号: {e.EcnNo}\n标题: {e.EcnTitle}\n影响产品: {e.AffectedProducts}\n影响工艺路线: {e.AffectedRoutes}\n变更原因: {e.Reason}\n影响分析: {e.ImpactAssessment}", "影响分析", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalApproved)); RaisePropertyChanged(nameof(AffectedProducts)); }
}
