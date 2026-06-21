using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using MES.Modules.Execute.Services;

namespace MES.Modules.Execute.ViewModels;

public class DispatchViewModel : BindableBase
{
    private readonly IExecuteService _executeService;
    private ObservableCollection<DispatchInfo> _dispatches = [];
    private DispatchInfo? _selectedDispatch;
    private string? _filterStatus;
    private string? _errorMessage;

    public ObservableCollection<DispatchInfo> Dispatches
    {
        get => _dispatches;
        set => SetProperty(ref _dispatches, value);
    }

    public DispatchInfo? SelectedDispatch
    {
        get => _selectedDispatch;
        set => SetProperty(ref _selectedDispatch, value);
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

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public int PendingCount => Dispatches.Count(d => d.Status == "Pending");
    public int InProgressCount => Dispatches.Count(d => d.Status == "InProgress");
    public int CompletedCount => Dispatches.Count(d => d.Status == "Completed");

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand StartCommand { get; }
    public DelegateCommand CompleteCommand { get; }

    public DispatchViewModel(IExecuteService executeService)
    {
        _executeService = executeService;

        RefreshCommand = new DelegateCommand(OnRefresh);
        StartCommand = new DelegateCommand(OnStart, () => SelectedDispatch != null && SelectedDispatch.Status == "Pending");
        CompleteCommand = new DelegateCommand(OnComplete, () => SelectedDispatch != null && SelectedDispatch.Status == "InProgress");

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
        var dispatches = await _executeService.GetAllDispatchesAsync();
        Dispatches = new ObservableCollection<DispatchInfo>(dispatches);
        UpdateStatistics();
        RaiseCanExecuteChanged();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterStatus))
        {
            _ = ReloadDataAsync();
            return;
        }

        var filtered = Dispatches.Where(d => d.Status == FilterStatus).ToList();
        Dispatches = new ObservableCollection<DispatchInfo>(filtered);
    }

    private async void OnStart()
    {
        if (SelectedDispatch == null) return;
        try
        {
            await _executeService.UpdateDispatchStatusAsync(SelectedDispatch.DispatchId, "InProgress");
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"启动失败: {ex.Message}";
        }
    }

    private async void OnComplete()
    {
        if (SelectedDispatch == null) return;
        try
        {
            await _executeService.UpdateDispatchStatusAsync(SelectedDispatch.DispatchId, "Completed");
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"完成失败: {ex.Message}";
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

    private void RaiseCanExecuteChanged()
    {
        StartCommand.RaiseCanExecuteChanged();
        CompleteCommand.RaiseCanExecuteChanged();
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(PendingCount));
        RaisePropertyChanged(nameof(InProgressCount));
        RaisePropertyChanged(nameof(CompletedCount));
    }
}
