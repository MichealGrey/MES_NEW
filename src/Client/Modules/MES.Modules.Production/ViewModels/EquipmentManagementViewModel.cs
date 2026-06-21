using System.Collections.ObjectModel;
using System.Windows;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class EquipmentManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<EquipmentInfo> _equipments = [];
    private EquipmentInfo? _selectedEquipment;
    private string? _errorMessage;

    public ObservableCollection<EquipmentInfo> Equipments
    {
        get => _equipments;
        set => SetProperty(ref _equipments, value);
    }

    public EquipmentInfo? SelectedEquipment
    {
        get => _selectedEquipment;
        set
        {
            SetProperty(ref _selectedEquipment, value);
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

    public EquipmentManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedEquipment != null);
        ToggleStatusCommand = new DelegateCommand(OnToggleStatus, () => SelectedEquipment != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            ErrorMessage = null;
            var equipments = await _masterDataService.GetAllEquipmentsAsync();
            Equipments = new ObservableCollection<EquipmentInfo>(equipments);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private void OnAdd()
    {
        MessageBox.Show("设备新增对话框尚未实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OnEdit()
    {
        if (SelectedEquipment == null) return;
        var clone = Clone(SelectedEquipment);
        MessageBox.Show("设备编辑对话框尚未实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async void OnToggleStatus()
    {
        if (SelectedEquipment == null) return;

        var newStatus = SelectedEquipment.Status == "Available" ? "Offline" : "Available";
        var result = MessageBox.Show(
            $"确认将设备 {SelectedEquipment.EquipmentName} ({SelectedEquipment.EquipmentId}) 的状态从 '{SelectedEquipment.Status}' 切换为 '{newStatus}'?",
            "确认切换", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _masterDataService.UpdateEquipmentStatusAsync(SelectedEquipment.EquipmentId, newStatus);
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

    private static EquipmentInfo Clone(EquipmentInfo e) => new()
    {
        EquipmentId = e.EquipmentId,
        EquipmentName = e.EquipmentName,
        EquipmentGroup = e.EquipmentGroup,
        ProcessStage = e.ProcessStage,
        EquipmentType = e.EquipmentType,
        Vendor = e.Vendor,
        Model = e.Model,
        SerialNumber = e.SerialNumber,
        Capability = e.Capability,
        Status = e.Status,
        CurrentLotId = e.CurrentLotId,
        CurrentRecipe = e.CurrentRecipe,
        SupportedRoutes = new List<string>(e.SupportedRoutes),
        SupportedSteps = new List<string>(e.SupportedSteps),
        LastMaintenanceDate = e.LastMaintenanceDate,
        MaintenanceIntervalHours = e.MaintenanceIntervalHours,
        RunningHours = e.RunningHours,
        Location = e.Location,
        ResponsiblePerson = e.ResponsiblePerson,
        CreatedAt = e.CreatedAt,
    };
}
