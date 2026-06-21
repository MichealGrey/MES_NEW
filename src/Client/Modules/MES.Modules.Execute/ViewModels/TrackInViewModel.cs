using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using MES.Modules.Execute.Services;

namespace MES.Modules.Execute.ViewModels;

public class TrackInViewModel : BindableBase
{
    private readonly IExecuteService _executeService;
    private ObservableCollection<TrackInRecord> _records = [];
    private TrackInRecord? _selectedRecord;
    private string _lotId = string.Empty;
    private string _equipmentId = string.Empty;
    private string _operatorName = string.Empty;
    private string? _errorMessage;

    public ObservableCollection<TrackInRecord> Records
    {
        get => _records;
        set => SetProperty(ref _records, value);
    }

    public TrackInRecord? SelectedRecord
    {
        get => _selectedRecord;
        set => SetProperty(ref _selectedRecord, value);
    }

    public string LotId
    {
        get => _lotId;
        set => SetProperty(ref _lotId, value);
    }

    public string EquipmentId
    {
        get => _equipmentId;
        set => SetProperty(ref _equipmentId, value);
    }

    public string OperatorName
    {
        get => _operatorName;
        set => SetProperty(ref _operatorName, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public int InProgressCount => Records.Count(r => r.Status == "InProgress");
    public int CompletedCount => Records.Count(r => r.Status == "Completed");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand TrackInCommand { get; }
    public DelegateCommand TrackOutCommand { get; }

    public TrackInViewModel(IExecuteService executeService)
    {
        _executeService = executeService;

        RefreshCommand = new DelegateCommand(OnRefresh);
        TrackInCommand = new DelegateCommand(OnTrackIn, CanTrackIn);
        TrackOutCommand = new DelegateCommand(OnTrackOut, () => SelectedRecord != null && SelectedRecord.Status == "InProgress");

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
        var records = await _executeService.GetAllTrackInRecordsAsync();
        Records = new ObservableCollection<TrackInRecord>(records);
        UpdateStatistics();
        TrackOutCommand.RaiseCanExecuteChanged();
    }

    private bool CanTrackIn() => !string.IsNullOrWhiteSpace(LotId) && !string.IsNullOrWhiteSpace(EquipmentId);

    private async void OnTrackIn()
    {
        try
        {
            var record = new TrackInRecord
            {
                RecordId = $"{LotId}_{DateTime.Now:HHmmss}",
                LotId = LotId,
                EquipmentId = EquipmentId,
                Operator = OperatorName,
                TrackInTime = DateTime.UtcNow,
                Status = "InProgress",
            };

            await _executeService.SaveTrackInRecordAsync(record);
            await ReloadDataAsync();

            LotId = string.Empty;
            EquipmentId = string.Empty;

            System.Windows.MessageBox.Show("Track-In成功!", "成功", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Track-In失败: {ex.Message}";
        }
    }

    private async void OnTrackOut()
    {
        if (SelectedRecord == null) return;
        try
        {
            SelectedRecord.Status = "Completed";
            SelectedRecord.TrackOutTime = DateTime.UtcNow;
            await _executeService.SaveTrackInRecordAsync(SelectedRecord);
            await ReloadDataAsync();

            System.Windows.MessageBox.Show("Track-Out成功!", "成功", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Track-Out失败: {ex.Message}";
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

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(InProgressCount));
        RaisePropertyChanged(nameof(CompletedCount));
    }
}
