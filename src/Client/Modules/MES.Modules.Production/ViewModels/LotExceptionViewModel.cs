using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class LotExceptionViewModel : BindableBase
{
    private ObservableCollection<LotExceptionInfo> _allExceptions = [];
    private ObservableCollection<LotExceptionInfo> _filteredExceptions = [];
    private string _filterStatus = "全部";
    private string _searchText = string.Empty;
    private int _totalExceptions;
    private int _pendingCount;
    private int _resolvedCount;

    public ObservableCollection<LotExceptionInfo> AllExceptions
    {
        get => _allExceptions;
        set => SetProperty(ref _allExceptions, value);
    }

    public ObservableCollection<LotExceptionInfo> FilteredExceptions
    {
        get => _filteredExceptions;
        set => SetProperty(ref _filteredExceptions, value);
    }

    public string FilterStatus
    {
        get => _filterStatus;
        set
        {
            if (SetProperty(ref _filterStatus, value))
                ApplyFilter();
        }
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

    public int TotalExceptions
    {
        get => _totalExceptions;
        set => SetProperty(ref _totalExceptions, value);
    }

    public int PendingCount
    {
        get => _pendingCount;
        set => SetProperty(ref _pendingCount, value);
    }

    public int ResolvedCount
    {
        get => _resolvedCount;
        set => SetProperty(ref _resolvedCount, value);
    }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ExportCommand { get; }

    public LotExceptionViewModel()
    {
        RefreshCommand = new DelegateCommand(OnRefresh);
        ExportCommand = new DelegateCommand(OnExport);

        InitializeDemoData();
    }

    private void InitializeDemoData()
    {
        AllExceptions = new ObservableCollection<LotExceptionInfo>
        {
            new() { ExceptionId = "EXC-2026-001", LotId = "LOT-2026-0001", ExceptionType = "良率异常", Description = "FT工序良率低于阈值(92.3% < 95%)", Status = "待处理", DetectedAt = DateTime.Now.AddHours(-2), Handler = "" },
            new() { ExceptionId = "EXC-2026-002", LotId = "LOT-2026-0003", ExceptionType = "设备异常", Description = "WB设备#1参数异常，需停机检查", Status = "处理中", DetectedAt = DateTime.Now.AddHours(-5), Handler = "陈工程师" },
            new() { ExceptionId = "EXC-2026-003", LotId = "LOT-2026-0005", ExceptionType = "物料异常", Description = "使用错误批号金线(WIRE-AU-08 vs WIRE-AU-10)", Status = "已关闭", DetectedAt = DateTime.Now.AddDays(-1), Handler = "赵组长" },
            new() { ExceptionId = "EXC-2026-004", LotId = "LOT-2026-0007", ExceptionType = "滞留超时", Description = "PMC工序滞留超过24小时", Status = "待处理", DetectedAt = DateTime.Now.AddHours(-1), Handler = "" },
        };

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered = AllExceptions.AsEnumerable();

        if (!string.IsNullOrEmpty(_filterStatus) && _filterStatus != "全部")
            filtered = filtered.Where(e => e.Status == _filterStatus);

        if (!string.IsNullOrWhiteSpace(_searchText))
        {
            var keyword = _searchText.ToLowerInvariant();
            filtered = filtered.Where(e =>
                e.LotId.Contains(keyword, System.StringComparison.OrdinalIgnoreCase) ||
                e.ExceptionId.Contains(keyword, System.StringComparison.OrdinalIgnoreCase) ||
                e.Description.Contains(keyword, System.StringComparison.OrdinalIgnoreCase));
        }

        FilteredExceptions = new ObservableCollection<LotExceptionInfo>(filtered);
        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        TotalExceptions = AllExceptions.Count;
        PendingCount = AllExceptions.Count(e => e.Status == "待处理");
        ResolvedCount = AllExceptions.Count(e => e.Status == "已关闭");
    }

    private void OnRefresh() => InitializeDemoData();
    private void OnExport() { /* TODO: 导出异常报表 */ }
}

public class LotExceptionInfo
{
    public string ExceptionId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public string Handler { get; set; } = string.Empty;
}
