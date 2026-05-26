using Prism.Commands;
using Prism.Mvvm;
using MES.Domain.Production;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace MES.Modules.Production.ViewModels;

/// <summary>
/// WIP 工序级统计信息
/// </summary>
public class WipStepInfo
{
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public int WipCount { get; set; }              // 在该工序的批次总数
    public int UrgentCount { get; set; }           // 紧急批次
    public int HoldCount { get; set; }             // Hold批次
    public int WaitingCount { get; set; }          // 等待中批次
    public int ProcessingCount { get; set; }       // 生产中批次
    public double TotalQty { get; set; }           // 总数量
    public double AvgYield { get; set; }           // 平均良率
    public bool IsAssemble { get; set; } = true;   // 工艺阶段
}

/// <summary>
/// WIP 汇总统计
/// </summary>
public class WipSummary
{
    public int TotalLots { get; set; }
    public int TotalQty { get; set; }
    public int UrgentLots { get; set; }
    public int HoldLots { get; set; }
    public int WaitingLots { get; set; }
    public int ProcessingLots { get; set; }
    public int CompletedLots { get; set; }
    public double AvgYield { get; set; }
    public int AssembleLots { get; set; }
    public int TestLots { get; set; }
}

public class WipOverviewViewModel : BindableBase
{
    private readonly IProductionDataService _dataService;
    private ObservableCollection<WipStepInfo> _wipByStep = [];
    private WipSummary _summary = new();
    private string? _errorMessage;
    private ProcessStage _selectedStage = ProcessStage.All;

    // 工序名称映射
    private static readonly Dictionary<string, string> StepNameMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "DieAttach", "贴片(DieAttach)" },
        { "WireBond", "焊线(WireBond)" },
        { "Mold", "模压(Mold)" },
        { "TrimForm", "切筋(TrimForm)" },
        { "Marking", "打标(Marking)" },
        { "Cure", "固化(Cure)" },
        { "LaserMark", "激光打标" },
        { "VisualInspect", "外观检查" },
        { "Packing", "包装(Packing)" },
        { "CPTest", "CP测试" },
        { "FTTest", "FT测试" },
        { "BurnIn", "老化(BurnIn)" },
        { "FinalTest", "终测(FinalTest)" },
        { "FinalInspection", "终检" },
        { "TapeAndReel", "编带(TapeAndReel)" },
        { "AOI", "AOI检测" },
        { "XRay", "X-Ray检测" },
        { "ScanTest", "扫描测试" },
    };

    public ObservableCollection<WipStepInfo> WipByStep
    {
        get => _wipByStep;
        set => SetProperty(ref _wipByStep, value);
    }

    public WipSummary Summary
    {
        get => _summary;
        set => SetProperty(ref _summary, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ProcessStage SelectedStage
    {
        get => _selectedStage;
        set
        {
            if (SetProperty(ref _selectedStage, value))
                RefreshData();
        }
    }

    public DelegateCommand RefreshCommand { get; }

    public WipOverviewViewModel(IProductionDataService dataService)
    {
        _dataService = dataService;
        RefreshCommand = new DelegateCommand(async () => await ReloadDataAsync());
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
        BuildWipData(lots);
    }

    private void RefreshData()
    {
        if (WipByStep.Count == 0) return;
        // 从当前数据重新过滤（不重新请求API）
        var allLots = WipByStep.SelectMany(s => Enumerable.Repeat(0, s.WipCount)).ToList();
        // 简化：直接重新加载
        _ = ReloadDataAsync();
    }

    private void BuildWipData(List<LotInfo> lots)
    {
        // 按工艺阶段过滤
        var filtered = SelectedStage switch
        {
            ProcessStage.Assemble => lots.Where(l => LotListViewModel.GetStage(l) == ProcessStage.Assemble).ToList(),
            ProcessStage.Test => lots.Where(l => LotListViewModel.GetStage(l) == ProcessStage.Test).ToList(),
            _ => lots
        };

        // 按工序分组统计
        var stepGroups = filtered
            .GroupBy(l => l.CurrentStep)
            .Select(g => new WipStepInfo
            {
                StepCode = g.Key,
                StepName = StepNameMap.TryGetValue(g.Key, out var name) ? name : g.Key,
                WipCount = g.Count(),
                UrgentCount = g.Count(l => l.Priority == "Urgent"),
                HoldCount = g.Count(l => l.Status == "Hold"),
                WaitingCount = g.Count(l => l.Status == "Waiting"),
                ProcessingCount = g.Count(l => l.Status == "Processing"),
                TotalQty = g.Sum(l => l.UnitCount),
                AvgYield = g.Average(l => l.CurrentYield),
                IsAssemble = LotListViewModel.GetStage(g.First()) == ProcessStage.Assemble,
            })
            .OrderBy(s => s.IsAssemble ? 0 : 1)
            .ThenBy(s => s.StepCode)
            .ToList();

        WipByStep = new ObservableCollection<WipStepInfo>(stepGroups);

        // 汇总统计
        Summary = new WipSummary
        {
            TotalLots = filtered.Count,
            TotalQty = filtered.Sum(l => l.UnitCount),
            UrgentLots = filtered.Count(l => l.Priority == "Urgent"),
            HoldLots = filtered.Count(l => l.Status == "Hold"),
            WaitingLots = filtered.Count(l => l.Status == "Waiting"),
            ProcessingLots = filtered.Count(l => l.Status == "Processing"),
            CompletedLots = filtered.Count(l => l.Status == "Completed"),
            AvgYield = filtered.Count > 0 ? filtered.Average(l => l.CurrentYield) : 0,
            AssembleLots = filtered.Count(l => LotListViewModel.GetStage(l) == ProcessStage.Assemble),
            TestLots = filtered.Count(l => LotListViewModel.GetStage(l) == ProcessStage.Test),
        };
    }
}
