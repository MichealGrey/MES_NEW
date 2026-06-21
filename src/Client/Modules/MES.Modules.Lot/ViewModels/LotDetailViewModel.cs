using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Modules.Lot.Services;

namespace MES.Modules.Lot.ViewModels;

public class LotDetailViewModel : BindableBase
{
    private readonly ILotService _lotService;
    private LotInfo? _lot;
    private string _lotId = string.Empty;
    private string? _errorMessage;

    public LotInfo? Lot
    {
        get => _lot;
        set => SetProperty(ref _lot, value);
    }

    public string LotId
    {
        get => _lotId;
        set => SetProperty(ref _lotId, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand SearchCommand { get; }
    public DelegateCommand HoldCommand { get; }
    public DelegateCommand ReleaseCommand { get; }
    public DelegateCommand ArchiveCommand { get; }

    public LotDetailViewModel(ILotService lotService)
    {
        _lotService = lotService;

        SearchCommand = new DelegateCommand(OnSearch, () => !string.IsNullOrWhiteSpace(LotId));
        HoldCommand = new DelegateCommand(OnHold, () => Lot != null && (Lot.Status == "InProgress" || Lot.Status == "Created"));
        ReleaseCommand = new DelegateCommand(OnRelease, () => Lot != null && Lot.Status == "Hold");
        ArchiveCommand = new DelegateCommand(OnArchive, () => Lot != null && (Lot.Status == "Completed" || Lot.Status == "Closed"));

        _ = InitializeAsync();
    }

    private Task InitializeAsync() => Task.CompletedTask;

    private async void OnSearch()
    {
        if (string.IsNullOrWhiteSpace(LotId)) return;
        try
        {
            ErrorMessage = null;
            Lot = await _lotService.GetLotAsync(LotId.Trim());
            if (Lot == null)
                ErrorMessage = $"未找到批次 {LotId}";
            
            RaiseCanExecuteChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"查询失败: {ex.Message}";
        }
    }

    private async void OnHold()
    {
        if (Lot == null) return;
        try
        {
            var reason = Microsoft.VisualBasic.Interaction.InputBox("请输入Hold原因:", "批次Hold", "");
            if (string.IsNullOrWhiteSpace(reason)) return;

            await _lotService.HoldLotAsync(Lot.LotId, reason);
            Lot = await _lotService.GetLotAsync(Lot.LotId);
            RaiseCanExecuteChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hold失败: {ex.Message}";
        }
    }

    private async void OnRelease()
    {
        if (Lot == null) return;
        try
        {
            if (System.Windows.MessageBox.Show($"确定释放批次 {Lot.LotId}?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
                return;

            await _lotService.ReleaseLotAsync(Lot.LotId);
            Lot = await _lotService.GetLotAsync(Lot.LotId);
            RaiseCanExecuteChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"释放失败: {ex.Message}";
        }
    }

    private async void OnArchive()
    {
        if (Lot == null) return;
        try
        {
            if (System.Windows.MessageBox.Show($"确定归档批次 {Lot.LotId}?", "确认", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
                return;

            await _lotService.ArchiveLotAsync(Lot.LotId);
            Lot = await _lotService.GetLotAsync(Lot.LotId);
            RaiseCanExecuteChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"归档失败: {ex.Message}";
        }
    }

    private void RaiseCanExecuteChanged()
    {
        SearchCommand.RaiseCanExecuteChanged();
        HoldCommand.RaiseCanExecuteChanged();
        ReleaseCommand.RaiseCanExecuteChanged();
        ArchiveCommand.RaiseCanExecuteChanged();
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        if (navigationContext.Parameters.TryGetValue("LotId", out string? lotId) && !string.IsNullOrEmpty(lotId))
        {
            LotId = lotId;
            await LoadLotAsync(lotId);
        }
    }

    private async Task LoadLotAsync(string lotId)
    {
        try
        {
            ErrorMessage = null;
            Lot = await _lotService.GetLotAsync(lotId);
            if (Lot == null)
                ErrorMessage = $"未找到批次 {lotId}";
            RaiseCanExecuteChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
    }
}
