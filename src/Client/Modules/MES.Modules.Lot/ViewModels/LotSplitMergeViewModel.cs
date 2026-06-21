using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using MES.Modules.Lot.Services;

namespace MES.Modules.Lot.ViewModels;

public class LotSplitMergeViewModel : BindableBase
{
    private readonly ILotService _lotService;
    private ObservableCollection<LotInfo> _allLots = [];
    private LotInfo? _sourceLot;
    private LotInfo? _targetLot;
    private int _splitQty;
    private string _splitReason = string.Empty;
    private string _operationType = "Split";
    private string? _errorMessage;

    public ObservableCollection<LotInfo> AllLots
    {
        get => _allLots;
        set => SetProperty(ref _allLots, value);
    }

    public LotInfo? SourceLot
    {
        get => _sourceLot;
        set => SetProperty(ref _sourceLot, value);
    }

    public LotInfo? TargetLot
    {
        get => _targetLot;
        set => SetProperty(ref _targetLot, value);
    }

    public int SplitQty
    {
        get => _splitQty;
        set => SetProperty(ref _splitQty, value);
    }

    public string SplitReason
    {
        get => _splitReason;
        set => SetProperty(ref _splitReason, value);
    }

    public string OperationType
    {
        get => _operationType;
        set => SetProperty(ref _operationType, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ExecuteSplitCommand { get; }
    public DelegateCommand ExecuteMergeCommand { get; }

    public LotSplitMergeViewModel(ILotService lotService)
    {
        _lotService = lotService;

        RefreshCommand = new DelegateCommand(OnRefresh);
        ExecuteSplitCommand = new DelegateCommand(OnExecuteSplit, CanExecuteSplit);
        ExecuteMergeCommand = new DelegateCommand(OnExecuteMerge, CanExecuteMerge);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            ErrorMessage = null;
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private async Task ReloadDataAsync()
    {
        var lots = await _lotService.GetAllLotsAsync();
        AllLots = new ObservableCollection<LotInfo>(lots.Where(l => l.Status == "InProgress" || l.Status == "Created"));
        ExecuteSplitCommand.RaiseCanExecuteChanged();
        ExecuteMergeCommand.RaiseCanExecuteChanged();
    }

    private bool CanExecuteSplit() => SourceLot != null && SplitQty > 0 && SplitQty < SourceLot.UnitCount && !string.IsNullOrWhiteSpace(SplitReason);
    private bool CanExecuteMerge() => SourceLot != null && TargetLot != null && SourceLot.LotId != TargetLot.LotId;

    private async void OnExecuteSplit()
    {
        if (SourceLot == null || SplitQty <= 0) return;
        try
        {
            if (System.Windows.MessageBox.Show(
                $"确定将批次 {SourceLot.LotId} 拆分 {SplitQty} 个单元?",
                "确认拆分",
                System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
                return;

            var newLot = new LotInfo
            {
                LotId = $"{SourceLot.LotId}-S{DateTime.Now:HHmmss}",
                OrderId = SourceLot.OrderId,
                ProductId = SourceLot.ProductId,
                ProductName = SourceLot.ProductName,
                DieName = SourceLot.DieName,
                CurrentStep = SourceLot.CurrentStep,
                Status = "Created",
                UnitCount = SplitQty,
                RouteId = SourceLot.RouteId,
                RouteVersion = SourceLot.RouteVersion,
                CurrentStepSeq = SourceLot.CurrentStepSeq,
                IsPartialLot = true,
                MotherLotId = SourceLot.LotId,
                SplitReason = SplitReason,
                SplitTime = DateTime.UtcNow,
                SplitQty = SplitQty,
                ProcessStage = SourceLot.ProcessStage,
                CreatedAt = DateTime.UtcNow,
            };

            await _lotService.SaveLotAsync(newLot);

            SourceLot.UnitCount -= SplitQty;
            await _lotService.SaveLotAsync(SourceLot);

            await ReloadDataAsync();
            SourceLot = null;
            SplitQty = 0;
            SplitReason = string.Empty;

            System.Windows.MessageBox.Show("拆分成功!", "成功", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"拆分失败: {ex.Message}";
        }
    }

    private async void OnExecuteMerge()
    {
        if (SourceLot == null || TargetLot == null) return;
        try
        {
            if (System.Windows.MessageBox.Show(
                $"确定将批次 {SourceLot.LotId} 合并到 {TargetLot.LotId}?",
                "确认合并",
                System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
                return;

            TargetLot.UnitCount += SourceLot.UnitCount;
            await _lotService.SaveLotAsync(TargetLot);

            await _lotService.DeleteLotAsync(SourceLot.LotId);

            await ReloadDataAsync();
            SourceLot = null;
            TargetLot = null;

            System.Windows.MessageBox.Show("合并成功!", "成功", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"合并失败: {ex.Message}";
        }
    }

    private async void OnRefresh()
    {
        try
        {
            ErrorMessage = null;
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"刷新失败: {ex.Message}";
        }
    }
}
