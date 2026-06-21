using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Domain.Production;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MES.Modules.Production.ViewModels;

public class LotListViewModel : BindableBase
{
    private readonly IProductionDataService _dataService;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<LotInfo> _lots = [];
    private ObservableCollection<LotInfo> _filteredLots = [];
    private LotInfo? _selectedLot;
    private string _searchText = string.Empty;
    private string? _filterStatus;
    private ProcessStage? _selectedStage = null;
    private string? _errorMessage;

    // Assemble 阶段工序关键字
    private static readonly HashSet<string> AssembleSteps = new(StringComparer.OrdinalIgnoreCase)
    {
        "DieAttach", "WireBond", "Mold", "TrimForm", "Marking", "Cure",
        "LaserMark", "SolderPlate", "VisualInspect", "Packing"
    };

    // Test 阶段工序关键字
    private static readonly HashSet<string> TestSteps = new(StringComparer.OrdinalIgnoreCase)
    {
        "CPTest", "FTTest", "BurnIn", "FinalTest", "FinalInspection",
        "TapeAndReel", "AOI", "XRay", "ScanTest"
    };

    public ObservableCollection<LotInfo> Lots
    {
        get => _lots;
        set => SetProperty(ref _lots, value);
    }

    public ObservableCollection<LotInfo> FilteredLots
    {
        get => _filteredLots;
        set => SetProperty(ref _filteredLots, value);
    }

    public LotInfo? SelectedLot
    {
        get => _selectedLot;
        set => SetProperty(ref _selectedLot, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                ApplyFilter();
        }
    }

    public string? FilterStatus
    {
        get => _filterStatus;
        set
        {
            if (SetProperty(ref _filterStatus, value))
                ApplyFilter();
        }
    }

    public ProcessStage? SelectedStage
    {
        get => _selectedStage;
        set
        {
            if (SetProperty(ref _selectedStage, value))
                ApplyFilter();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    // 统计
    public int TotalCount => Lots.Count;
    public int AssembleCount => Lots.Count(l => GetStage(l) == ProcessStage.Assemble);
    public int TestCount => Lots.Count(l => GetStage(l) == ProcessStage.Test);
    public int HoldCount => Lots.Count(l => l.Status == "Hold");
    public int WaitingCount => Lots.Count(l => l.Status == "Waiting");
    public int ProcessingCount => Lots.Count(l => l.Status == "Processing");

    // 命令
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<LotInfo?> ViewDetailCommand { get; }

    public LotListViewModel(IProductionDataService dataService, IRegionManager regionManager)
    {
        _dataService = dataService;
        _regionManager = regionManager;

        RefreshCommand = new DelegateCommand(async () => await ReloadDataAsync());
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        ViewDetailCommand = new DelegateCommand<LotInfo?>(OnViewDetail, lot => lot != null);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            ErrorMessage = null;
            await _dataService.EnsureSeededAsync();
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private async Task ReloadDataAsync()
    {
        var lots = await _dataService.GetAllLotsAsync();
        Lots = new ObservableCollection<LotInfo>(lots);
        ApplyFilter();
        UpdateStatistics();
    }

    private void ApplyFilter()
    {
        var query = Lots.AsEnumerable();

        // 工艺阶段过滤
        if (SelectedStage.HasValue)
            query = query.Where(l => GetStage(l) == SelectedStage);

        // 状态过滤
        if (!string.IsNullOrWhiteSpace(FilterStatus))
            query = query.Where(l => l.Status == FilterStatus);

        // 搜索
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var key = SearchText.Trim().ToLower();
            query = query.Where(l =>
                l.LotId.ToLower().Contains(key)
                || l.OrderId.ToLower().Contains(key)
                || l.ProductName.ToLower().Contains(key)
                || l.DieName.ToLower().Contains(key)
                || l.CurrentStep.ToLower().Contains(key));
        }

        // Urgent 优先排序
        var sorted = query
            .OrderByDescending(l => l.Priority == "Urgent")
            .ThenByDescending(l => l.Status == "Hold")
            .ThenBy(l => l.CurrentStepSeq)
            .ToList();

        FilteredLots = new ObservableCollection<LotInfo>(sorted);
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(AssembleCount));
        RaisePropertyChanged(nameof(TestCount));
        RaisePropertyChanged(nameof(HoldCount));
        RaisePropertyChanged(nameof(WaitingCount));
        RaisePropertyChanged(nameof(ProcessingCount));
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        FilterStatus = null;
        SelectedStage = null;
    }

    private void OnViewDetail(LotInfo? lot)
    {
        if (lot == null) return;
        var parameters = new NavigationParameters { { "LotId", lot.LotId } };
        _regionManager.RequestNavigate("MainContentRegion", "LotDetailViewV2", parameters);
    }

    private void RaiseCanExecuteChanged()
    {
        ViewDetailCommand.RaiseCanExecuteChanged();
    }

    /// <summary>
    /// 根据 ProcessStage 字段判定工艺阶段（优先使用数据库字段）
    /// </summary>
    public static ProcessStage GetStage(LotInfo lot)
    {
        // 优先使用数据库中的 process_stage 字段映射
        return lot.ProcessStage switch
        {
            Models.ProcessStage.Assemble => ProcessStage.Assemble,
            Models.ProcessStage.Test => ProcessStage.Test,
            Models.ProcessStage.Finished => ProcessStage.Test,
            _ => ProcessStage.Assemble
        };
    }
}
