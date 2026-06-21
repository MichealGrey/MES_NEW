using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using MES.Modules.Lot.Services;

namespace MES.Modules.Lot.ViewModels;

public class LotHoldViewModel : BindableBase
{
    private readonly ILotService _lotService;
    private ObservableCollection<LotInfo> _holdLots = [];
    private LotInfo? _selectedLot;
    private string _holdReason = string.Empty;
    private string _holdCategory = "Engineering";
    private string _remark = string.Empty;
    private string? _errorMessage;

    public ObservableCollection<LotInfo> HoldLots
    {
        get => _holdLots;
        set => SetProperty(ref _holdLots, value);
    }

    public LotInfo? SelectedLot
    {
        get => _selectedLot;
        set
        {
            if (SetProperty(ref _selectedLot, value))
            {
                if (_selectedLot != null)
                {
                    HoldReason = _selectedLot.HoldReason ?? string.Empty;
                    HoldCategory = _selectedLot.HoldCategory ?? "Engineering";
                    Remark = _selectedLot.Remark ?? string.Empty;
                }
            }
        }
    }

    public string HoldReason
    {
        get => _holdReason;
        set => SetProperty(ref _holdReason, value);
    }

    public string HoldCategory
    {
        get => _holdCategory;
        set => SetProperty(ref _holdCategory, value);
    }

    public string Remark
    {
        get => _remark;
        set => SetProperty(ref _remark, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public int HoldCount => HoldLots.Count;

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ReleaseCommand { get; }
    public DelegateCommand ReleaseAllCommand { get; }

    public LotHoldViewModel(ILotService lotService)
    {
        _lotService = lotService;

        RefreshCommand = new DelegateCommand(OnRefresh);
        ReleaseCommand = new DelegateCommand(OnRelease, () => SelectedLot != null);
        ReleaseAllCommand = new DelegateCommand(OnReleaseAll, () => HoldLots.Count > 0);

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
        var lots = await _lotService.GetHoldLotsAsync();
        HoldLots = new ObservableCollection<LotInfo>(lots);
        RaisePropertyChanged(nameof(HoldCount));
        ReleaseAllCommand.RaiseCanExecuteChanged();
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

    private async void OnRelease()
    {
        if (SelectedLot == null) return;
        try
        {
            if (System.Windows.MessageBox.Show($"确定释放批次 {SelectedLot.LotId}?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
                return;

            await _lotService.ReleaseLotAsync(SelectedLot.LotId);
            await ReloadDataAsync();
            SelectedLot = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"释放失败: {ex.Message}";
        }
    }

    private async void OnReleaseAll()
    {
        if (HoldLots.Count == 0) return;
        try
        {
            if (System.Windows.MessageBox.Show($"确定释放所有 {HoldLots.Count} 个Hold批次?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
                return;

            foreach (var lot in HoldLots.ToList())
            {
                await _lotService.ReleaseLotAsync(lot.LotId);
            }
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"批量释放失败: {ex.Message}";
        }
    }
}
