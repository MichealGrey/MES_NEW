using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class LotOverviewViewModel : BindableBase
{
    private readonly IProductionDataService _dataService;
    private ObservableCollection<LotInfo> _lots = [];
    private ObservableCollection<LotInfo> _filteredLots = [];
    private string _searchText = string.Empty;
    private string? _statusFilter;
    private string? _stageFilter;
    private string? _errorMessage;
    private bool _isLoading;

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

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                ApplyFilter();
        }
    }

    public string? StatusFilter
    {
        get => _statusFilter;
        set
        {
            if (SetProperty(ref _statusFilter, value))
                ApplyFilter();
        }
    }

    public string? StageFilter
    {
        get => _stageFilter;
        set
        {
            if (SetProperty(ref _stageFilter, value))
                ApplyFilter();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    // 统计
    public int TotalCount => FilteredLots.Count;
    public int ActiveCount => FilteredLots.Count(l => l.Status is "Waiting" or "Processing");
    public int HoldCount => FilteredLots.Count(l => l.Status == "Hold");
    public int CompletedCount => FilteredLots.Count(l => l.Status == "Completed");

    // 命令
    public DelegateCommand SearchCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public LotOverviewViewModel(IProductionDataService dataService)
    {
        _dataService = dataService;

        SearchCommand = new DelegateCommand(() => ApplyFilter());
        RefreshCommand = new DelegateCommand(async () => await LoadLotsAsync());

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            ErrorMessage = null;
            await _dataService.EnsureSeededAsync();
            await LoadLotsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    public async Task LoadLotsAsync()
    {
        IsLoading = true;
        try
        {
            var allLots = await _dataService.GetAllLotsAsync();
            // 只显示未归档的活跃批次
            var activeLots = allLots.Where(l => !l.IsArchived).ToList();
            Lots = new ObservableCollection<LotInfo>(activeLots);
            ApplyFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyFilter()
    {
        var query = Lots.AsEnumerable();

        // 工艺阶段过滤
        if (!string.IsNullOrWhiteSpace(StatusFilter))
            query = query.Where(l => l.Status == StatusFilter);

        // Stage 过滤
        if (!string.IsNullOrWhiteSpace(StageFilter) && StageFilter != "All")
        {
            query = query.Where(l => GetStage(l) == StageFilter);
        }

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

        // 排序：Urgent 优先, Hold 其次
        var sorted = query
            .OrderByDescending(l => l.Priority == "Urgent")
            .ThenByDescending(l => l.Status == "Hold")
            .ThenBy(l => l.LotId)
            .ToList();

        FilteredLots = new ObservableCollection<LotInfo>(sorted);

        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(ActiveCount));
        RaisePropertyChanged(nameof(HoldCount));
        RaisePropertyChanged(nameof(CompletedCount));
    }

    /// <summary>
    /// 根据 ProcessStage 字段判定工艺阶段
    /// </summary>
    private static string GetStage(LotInfo lot)
    {
        return lot.ProcessStage switch
        {
            Models.ProcessStage.Assemble => "Assemble",
            Models.ProcessStage.Test => "Test",
            Models.ProcessStage.Finished => "Finished",
            _ => "Assemble"
        };
    }
}
