using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MES.Modules.Equipment.ViewModels;

public class EquipmentAlarmItem
{
    public string AlarmId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string AlarmCode { get; set; } = string.Empty;
    public string Severity { get; set; } = "Minor";
    public string Text { get; set; } = string.Empty;
    public DateTime Time { get; set; }
}

public class EquipmentAlarmViewModel : BindableBase
{
    private ObservableCollection<EquipmentAlarmItem> _alarms = [];
    public ObservableCollection<EquipmentAlarmItem> Alarms { get => _alarms; set => SetProperty(ref _alarms, value); }

    public EquipmentAlarmViewModel()
    {
        Alarms = new ObservableCollection<EquipmentAlarmItem>
        {
            new() { AlarmId = "ALM-001", EquipmentId = "MP-01", AlarmCode = "E301", Severity = "Critical", Text = "塑封温度超限", Time = DateTime.Now.AddMinutes(-5) },
            new() { AlarmId = "ALM-002", EquipmentId = "WB-01", AlarmCode = "E102", Severity = "Major", Text = "焊线功率异常", Time = DateTime.Now.AddMinutes(-15) },
            new() { AlarmId = "ALM-003", EquipmentId = "TH-01", AlarmCode = "E205", Severity = "Minor", Text = "测试座接触不良", Time = DateTime.Now.AddHours(-1) },
        };
    }
}
