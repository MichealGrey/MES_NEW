using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Domain.Production;
using MES.Infrastructure.Persistence.Entities;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class LotArchiveViewModel : BindableBase
{
    private readonly IProductionDataService _dataService;
    private ObservableCollection<ArchiveLotInfo> _archivedLots = [];
    private ObservableCollection<ArchiveLotInfo> _filteredLots = [];
    private DateTime? _fromDate;
    private DateTime? _toDate;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private bool _isLoading;

    public ObservableCollection<ArchiveLotInfo> ArchivedLots
    {
        get => _archivedLots;
        set => SetProperty(ref _archivedLots, value);
    }

    public ObservableCollection<ArchiveLotInfo> FilteredLots
    {
        get => _filteredLots;
        set => SetProperty(ref _filteredLots, value);
    }

    public DateTime? FromDate
    {
        get => _fromDate;
        set
        {
            if (SetProperty(ref _fromDate, value))
                ApplyFilter();
        }
    }

    public DateTime? ToDate
    {
        get => _toDate;
        set
        {
            if (SetProperty(ref _toDate, value))
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
    public int TotalArchived => FilteredLots.Count;
    public decimal TotalYield => FilteredLots.Count > 0
        ? (decimal)FilteredLots.Average(l => l.YieldPercent)
        : 0m;

    // 命令
    public DelegateCommand SearchCommand { get; }
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ExportCommand { get; }

    public LotArchiveViewModel(IProductionDataService dataService)
    {
        _dataService = dataService;

        SearchCommand = new DelegateCommand(() => ApplyFilter());
        RefreshCommand = new DelegateCommand(async () => await LoadArchivedLotsAsync());
        ExportCommand = new DelegateCommand(OnExport);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            ErrorMessage = null;
            await _dataService.EnsureSeededAsync();
            await LoadArchivedLotsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    public async Task LoadArchivedLotsAsync()
    {
        IsLoading = true;
        try
        {
            var archivedEntities = await _dataService.GetArchivedLotsAsync();
            var lots = archivedEntities.Select(MapToArchiveLotInfo).ToList();
            ArchivedLots = new ObservableCollection<ArchiveLotInfo>(lots);
            ApplyFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyFilter()
    {
        var query = ArchivedLots.AsEnumerable();

        // 日期范围过滤
        if (FromDate.HasValue)
            query = query.Where(l => l.ArchivedAt >= FromDate.Value);

        if (ToDate.HasValue)
            query = query.Where(l => l.ArchivedAt <= ToDate.Value.AddDays(1));

        // 搜索
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var key = SearchText.Trim().ToLower();
            query = query.Where(l =>
                l.LotId.ToLower().Contains(key)
                || l.WorkOrderId.ToLower().Contains(key)
                || l.ProductName.ToLower().Contains(key));
        }

        // 按归档时间倒序
        var sorted = query
            .OrderByDescending(l => l.ArchivedAt)
            .ToList();

        FilteredLots = new ObservableCollection<ArchiveLotInfo>(sorted);

        // 更新统计
        RaisePropertyChanged(nameof(TotalArchived));
        RaisePropertyChanged(nameof(TotalYield));
    }

    private void OnExport()
    {
        if (FilteredLots.Count == 0)
        {
            System.Windows.MessageBox.Show("没有可导出的数据。", "导出",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        // TODO: 实现实际导出逻辑（CSV/Excel）
        System.Windows.MessageBox.Show(
            $"已准备导出 {FilteredLots.Count} 条归档记录。\n\n" +
            $"此为演示功能，实际导出需集成报表服务。",
            "导出报表",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information);
    }

    private static ArchiveLotInfo MapToArchiveLotInfo(ProdLotArchive entity)
    {
        return new ArchiveLotInfo
        {
            LotId = entity.LotId,
            WorkOrderId = entity.OrderId,
            ProductName = entity.ProductName ?? "—",
            LotLevel = LotLevel.MotherLot,
            ProcessStage = MapProcessStage(entity.ProcessStage),
            CurrentStep = "—",
            Status = entity.Status ?? "Completed",
            Qty = entity.TotalPassQty,
            OriginalQty = entity.OriginalQty,
            YieldPercent = (double)entity.FinalYield,
            Priority = "Normal",
            ArchivedAt = entity.ArchivedAt,
            CreatedAt = entity.CreatedAt,
        };
    }

    private static LotLevel MapLotLevel(string? dbValue) => dbValue switch
    {
        "Wafer" => LotLevel.WaferLot,
        "Mother" => LotLevel.MotherLot,
        "Sub" => LotLevel.SubLot,
        "Grade" => LotLevel.GradeLot,
        _ => LotLevel.MotherLot
    };

    private static ProcessStage MapProcessStage(string? dbValue) => dbValue switch
    {
        "Frontend" or "Assemble" => ProcessStage.Assemble,
        "Backend" or "Test" => ProcessStage.Test,
        "Finished" => ProcessStage.Finished,
        _ => ProcessStage.Assemble
    };
}

/// <summary>
/// 归档批次展示用信息类
/// </summary>
public class ArchiveLotInfo
{
    public string LotId { get; set; } = string.Empty;
    public string WorkOrderId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public LotLevel LotLevel { get; set; }
    public ProcessStage ProcessStage { get; set; }
    public string CurrentStep { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Qty { get; set; }
    public int OriginalQty { get; set; }
    public double YieldPercent { get; set; }
    public string Priority { get; set; } = string.Empty;
    public DateTime ArchivedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public string LotLevelDisplay => LotLevel switch
    {
        LotLevel.WaferLot => "Wafer Lot",
        LotLevel.MotherLot => "Mother Lot",
        LotLevel.SubLot => "Sub Lot",
        LotLevel.GradeLot => "Grade Lot",
        _ => LotLevel.ToString()
    };

    public string ProcessStageDisplay => ProcessStage switch
    {
        ProcessStage.Assemble => "Assemble",
        ProcessStage.Test => "Test",
        ProcessStage.Finished => "Finished",
        _ => ProcessStage.ToString()
    };
}
