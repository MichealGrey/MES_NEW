using System.Collections.ObjectModel;
using System.Windows;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class CarrierManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<CarrierInfo> _carriers = [];
    private CarrierInfo? _selectedCarrier;
    private string? _errorMessage;

    public ObservableCollection<CarrierInfo> Carriers
    {
        get => _carriers;
        set => SetProperty(ref _carriers, value);
    }

    public CarrierInfo? SelectedCarrier
    {
        get => _selectedCarrier;
        set
        {
            SetProperty(ref _selectedCarrier, value);
            RaiseCanExecuteChanged();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AddCommand { get; }
    public DelegateCommand EditCommand { get; }
    public DelegateCommand ToggleStatusCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public CarrierManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedCarrier != null);
        ToggleStatusCommand = new DelegateCommand(OnToggleStatus, () => SelectedCarrier != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            ErrorMessage = null;
            var carriers = await _masterDataService.GetAllCarriersAsync();
            Carriers = new ObservableCollection<CarrierInfo>(carriers);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private void OnAdd()
    {
        MessageBox.Show("载具新增对话框尚未实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OnEdit()
    {
        if (SelectedCarrier == null) return;
        var clone = Clone(SelectedCarrier);
        MessageBox.Show("载具编辑对话框尚未实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async void OnToggleStatus()
    {
        if (SelectedCarrier == null) return;

        var newStatus = SelectedCarrier.Status == "Available" ? "InUse" : "Available";
        var result = MessageBox.Show(
            $"确认将载具 {SelectedCarrier.CarrierId} 的状态从 '{SelectedCarrier.Status}' 切换为 '{newStatus}'?",
            "确认切换", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _masterDataService.UpdateCarrierStatusAsync(SelectedCarrier.CarrierId, newStatus);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"状态切换失败: {ex.Message}";
            MessageBox.Show($"状态切换失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RaiseCanExecuteChanged()
    {
        EditCommand.RaiseCanExecuteChanged();
        ToggleStatusCommand.RaiseCanExecuteChanged();
    }

    private static CarrierInfo Clone(CarrierInfo c) => new()
    {
        CarrierId = c.CarrierId,
        CarrierType = c.CarrierType,
        Status = c.Status,
        CurrentLotId = c.CurrentLotId,
        Capacity = c.Capacity,
        UseCount = c.UseCount,
        MaxUseCount = c.MaxUseCount,
        LastCleanDate = c.LastCleanDate,
        CleanIntervalUses = c.CleanIntervalUses,
        Location = c.Location,
        ApplicableProcess = c.ApplicableProcess,
        ApplicablePackage = c.ApplicablePackage,
        CreatedAt = c.CreatedAt,
    };
}
