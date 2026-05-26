using Prism.Mvvm;
using MES.Modules.Alarm.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Alarm.ViewModels;

public class AlarmRuleConfigViewModel : BindableBase
{
    private ObservableCollection<AlarmRuleItem> _items = [];
    private AlarmRuleItem? _selectedItem;

    public ObservableCollection<AlarmRuleItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public AlarmRuleItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public AlarmRuleConfigViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<AlarmRuleItem>
        {
            new() { AlarmCode = "E301", AlarmName = "温度超限", EquipmentType = "MoldingPress", Severity = "Critical", AutoHold = true, Description = "模压温度超过185C时触发" },
            new() { AlarmCode = "E102", AlarmName = "超声波功率异常", EquipmentType = "WireBonder", Severity = "Major", AutoHold = true, Description = "超声波功率偏差超过10%" },
            new() { AlarmCode = "E205", AlarmName = "冷却水流量低", EquipmentType = "DicingSaw", Severity = "Minor", AutoHold = false, Description = "冷却水流量低于5L/min" },
            new() { AlarmCode = "E401", AlarmName = "贴装偏差超限", EquipmentType = "DieBonder", Severity = "Critical", AutoHold = true, Description = "芯片贴装偏移超过50μm" },
            new() { AlarmCode = "E110", AlarmName = "真空度异常", EquipmentType = "MoldingPress", Severity = "Major", AutoHold = true, Description = "模压腔体真空度超出设定值" },
            new() { AlarmCode = "E308", AlarmName = "EMC流量波动", EquipmentType = "MoldingPress", Severity = "Minor", AutoHold = false, Description = "EMC注塑流量波动超过5%" },
            new() { AlarmCode = "E501", AlarmName = "测试分选异常", EquipmentType = "TestHandler", Severity = "Critical", AutoHold = true, Description = "测试分选良率低于设定下限" },
            new() { AlarmCode = "E220", AlarmName = "排风压力低", EquipmentType = "All", Severity = "Minor", AutoHold = false, Description = "排风系统压力低于设定值" },
        };
    }
}
