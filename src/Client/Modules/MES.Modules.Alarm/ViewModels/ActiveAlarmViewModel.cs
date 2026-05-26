using Prism.Mvvm;
using MES.Modules.Alarm.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Alarm.ViewModels;

public class ActiveAlarmViewModel : BindableBase
{
    private ObservableCollection<ActiveAlarmItem> _items = [];
    private ActiveAlarmItem? _selectedItem;

    public ObservableCollection<ActiveAlarmItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public ActiveAlarmItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public ActiveAlarmViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<ActiveAlarmItem>
        {
            new() { AlarmId = "ALM-001", AlarmCode = "E301", EquipmentId = "EQ-003", Severity = "Critical", Text = "腔体温度超上限 (>250C)", AlarmTime = DateTime.Now.AddMinutes(-5), Duration = "00:05:12" },
            new() { AlarmId = "ALM-002", AlarmCode = "E102", EquipmentId = "EQ-008", Severity = "Major", Text = "RF功率偏差超过10%", AlarmTime = DateTime.Now.AddMinutes(-15), Duration = "00:15:30" },
            new() { AlarmId = "ALM-003", AlarmCode = "E205", EquipmentId = "EQ-012", Severity = "Minor", Text = "冷却水流量偏低", AlarmTime = DateTime.Now.AddHours(-1), Duration = "01:02:45" },
            new() { AlarmId = "ALM-004", AlarmCode = "E401", EquipmentId = "EQ-001", Severity = "Critical", Text = "对准标记偏差超限", AlarmTime = DateTime.Now.AddMinutes(-2), Duration = "00:02:18" },
            new() { AlarmId = "ALM-005", AlarmCode = "E110", EquipmentId = "EQ-015", Severity = "Major", Text = "真空度超出规格", AlarmTime = DateTime.Now.AddMinutes(-30), Duration = "00:30:55" },
            new() { AlarmId = "ALM-006", AlarmCode = "E308", EquipmentId = "EQ-005", Severity = "Minor", Text = "气体流量波动", AlarmTime = DateTime.Now.AddHours(-2), Duration = "02:15:20" },
            new() { AlarmId = "ALM-007", AlarmCode = "E501", EquipmentId = "EQ-010", Severity = "Critical", Text = "束流异常中断", AlarmTime = DateTime.Now.AddMinutes(-8), Duration = "00:08:33" },
            new() { AlarmId = "ALM-008", AlarmCode = "E220", EquipmentId = "EQ-018", Severity = "Minor", Text = "排风系统压力低", AlarmTime = DateTime.Now.AddHours(-3), Duration = "03:10:05" },
        };
    }
}
