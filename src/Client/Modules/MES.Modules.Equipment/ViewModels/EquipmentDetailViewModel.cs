using Prism.Mvvm;

namespace MES.Modules.Equipment.ViewModels;

public class EquipmentDetailViewModel : BindableBase
{
    private string _equipmentId = "EQ-001";
    private string _equipmentName = "焊线机-1";
    private string _status = "Idle";

    public string EquipmentId { get => _equipmentId; set => SetProperty(ref _equipmentId, value); }
    public string EquipmentName { get => _equipmentName; set => SetProperty(ref _equipmentName, value); }
    public string Status { get => _status; set => SetProperty(ref _status, value); }
}
