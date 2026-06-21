using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class GradeSortViewModel : BindableBase
{
    private readonly IProductionDataService _dataService;
    private ObservableCollection<LotInfo> _pendingLots = [];
    private LotInfo? _selectedLot;
    private string _binResults = "未选择批次";
    private string _gradeABin = "Bin1";
    private string _gradeBBin = "Bin2,Bin3";
    private string _gradeCBin = "Bin4,Bin5,Bin6";
    private string _message = string.Empty;
    private bool _isProcessing;

    public ObservableCollection<LotInfo> PendingLots
    {
        get => _pendingLots;
        set => SetProperty(ref _pendingLots, value);
    }

    public LotInfo? SelectedLot
    {
        get => _selectedLot;
        set
        {
            if (SetProperty(ref _selectedLot, value))
            {
                if (value != null)
                {
                    DisplayBinResults(value);
                }
                else
                {
                    BinResults = "未选择批次";
                }
            }
        }
    }

    public string BinResults
    {
        get => _binResults;
        set => SetProperty(ref _binResults, value);
    }

    public string GradeABin
    {
        get => _gradeABin;
        set => SetProperty(ref _gradeABin, value);
    }

    public string GradeBBin
    {
        get => _gradeBBin;
        set => SetProperty(ref _gradeBBin, value);
    }

    public string GradeCBin
    {
        get => _gradeCBin;
        set => SetProperty(ref _gradeCBin, value);
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        set => SetProperty(ref _isProcessing, value);
    }

    // 命令
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ExecuteSortCommand { get; }

    public GradeSortViewModel(IProductionDataService dataService)
    {
        _dataService = dataService;

        RefreshCommand = new DelegateCommand(async () => await LoadPendingLotsAsync());
        ExecuteSortCommand = new DelegateCommand(async () => await ExecuteSortAsync(), CanExecuteSort);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await _dataService.EnsureSeededAsync();
            await LoadPendingLotsAsync();
        }
        catch (Exception ex)
        {
            Message = $"数据加载失败: {ex.Message}";
        }
    }

    public async Task LoadPendingLotsAsync()
    {
        try
        {
            var allLots = await _dataService.GetAllLotsAsync();
            // 筛选：已完成测试、有 Bin 结果、未分选的批次
            var pending = allLots
                .Where(l => !l.IsArchived
                            && l.Status == "Completed"
                            && !string.IsNullOrEmpty(l.BinResult))
                .ToList();

            PendingLots = new ObservableCollection<LotInfo>(pending);
            Message = $"找到 {pending.Count} 个待分选批次";
        }
        catch (Exception ex)
        {
            Message = $"加载失败: {ex.Message}";
        }
    }

    private void DisplayBinResults(LotInfo lot)
    {
        if (string.IsNullOrEmpty(lot.BinResult))
        {
            BinResults = "该批次无 Bin 结果";
            return;
        }

        try
        {
            // 尝试解析 JSON 格式的 Bin 结果
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var formatted = JsonSerializer.Serialize(
                JsonSerializer.Deserialize<object>(lot.BinResult) ?? lot.BinResult, options);
            BinResults = formatted;
        }
        catch
        {
            // 如果不是 JSON，直接显示原文
            BinResults = lot.BinResult;
        }
    }

    private bool CanExecuteSort()
    {
        return SelectedLot != null
               && !IsProcessing
               && !string.IsNullOrWhiteSpace(GradeABin);
    }

    public async Task ExecuteSortAsync()
    {
        if (SelectedLot == null) return;

        try
        {
            IsProcessing = true;
            ExecuteSortCommand.RaiseCanExecuteChanged();

            var gradeBBins = GradeBBin.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var gradeCBins = GradeCBin.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // 解析当前批次的 Bin 结果
            var binResult = SelectedLot.BinResult ?? "{}";
            var binData = JsonSerializer.Deserialize<Dictionary<string, int>>(binResult) ?? new Dictionary<string, int>();

            // 根据规则统计各等级数量
            int gradeAQTY = 0;
            int gradeBQTY = 0;
            int gradeCQTY = 0;

            foreach (var kvp in binData)
            {
                var binName = kvp.Key;
                var qty = kvp.Value;

                if (binName.Equals(GradeABin, StringComparison.OrdinalIgnoreCase))
                    gradeAQTY += qty;
                else if (gradeBBins.Any(b => b.Equals(binName, StringComparison.OrdinalIgnoreCase)))
                    gradeBQTY += qty;
                else if (gradeCBins.Any(b => b.Equals(binName, StringComparison.OrdinalIgnoreCase)))
                    gradeCQTY += qty;
            }

            // TODO: 调用 ILotSplitMergeService 创建等级子批次
            // 以下为模拟逻辑
            Message = $"分选完成: " +
                      $"Grade A ({GradeABin}) = {gradeAQTY} | " +
                      $"Grade B ({GradeBBin}) = {gradeBQTY} | " +
                      $"Grade C ({GradeCBin}) = {gradeCQTY}";

            // 刷新列表
            await LoadPendingLotsAsync();
        }
        catch (Exception ex)
        {
            Message = $"分选失败: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
            ExecuteSortCommand.RaiseCanExecuteChanged();
        }
    }
}
