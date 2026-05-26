using Prism.Mvvm;
using MES.Modules.Alarm.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Alarm.ViewModels;

public class AlarmHistoryViewModel : BindableBase
{
    private ObservableCollection<AlarmHistoryItem> _items = [];
    private AlarmHistoryItem? _selectedItem;

    public ObservableCollection<AlarmHistoryItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public AlarmHistoryItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public AlarmHistoryViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        var now = DateTime.Now;
        Items = new ObservableCollection<AlarmHistoryItem>
        {
            new() { AlarmId = "ALM-101", AlarmCode = "E301", EquipmentId = "EQ-003", Severity = "Critical", Text = "腔体温度超上限", AlarmTime = now.AddHours(-5), AckTime = now.AddHours(-5).AddMinutes(3), AckBy = "Zhang Wei", CloseTime = now.AddHours(-4) },
            new() { AlarmId = "ALM-102", AlarmCode = "E102", EquipmentId = "EQ-008", Severity = "Major", Text = "RF功率偏差", AlarmTime = now.AddHours(-8), AckTime = now.AddHours(-8).AddMinutes(10), AckBy = "Li Ming", CloseTime = now.AddHours(-6) },
            new() { AlarmId = "ALM-103", AlarmCode = "E205", EquipmentId = "EQ-012", Severity = "Minor", Text = "冷却水流量偏低", AlarmTime = now.AddHours(-12), AckTime = now.AddHours(-12).AddMinutes(15), AckBy = "Wang Fang", CloseTime = now.AddHours(-10) },
            new() { AlarmId = "ALM-104", AlarmCode = "E401", EquipmentId = "EQ-001", Severity = "Critical", Text = "对准标记偏差超限", AlarmTime = now.AddDays(-1), AckTime = now.AddDays(-1).AddMinutes(5), AckBy = "Liu Jun", CloseTime = now.AddDays(-1).AddHours(2) },
            new() { AlarmId = "ALM-105", AlarmCode = "E110", EquipmentId = "EQ-015", Severity = "Major", Text = "真空度超出规格", AlarmTime = now.AddDays(-1).AddHours(-3), AckTime = now.AddDays(-1).AddHours(-3).AddMinutes(8), AckBy = "Chen Lei", CloseTime = now.AddDays(-1).AddHours(-1) },
            new() { AlarmId = "ALM-106", AlarmCode = "E308", EquipmentId = "EQ-005", Severity = "Minor", Text = "气体流量波动", AlarmTime = now.AddDays(-2), AckTime = now.AddDays(-2).AddMinutes(20), AckBy = "Zhang Wei", CloseTime = now.AddDays(-2).AddHours(3) },
            new() { AlarmId = "ALM-107", AlarmCode = "E501", EquipmentId = "EQ-010", Severity = "Critical", Text = "束流异常中断", AlarmTime = now.AddDays(-2).AddHours(-5), AckTime = now.AddDays(-2).AddHours(-5).AddMinutes(2), AckBy = "Li Ming", CloseTime = now.AddDays(-2).AddHours(-3) },
            new() { AlarmId = "ALM-108", AlarmCode = "E220", EquipmentId = "EQ-018", Severity = "Minor", Text = "排风系统压力低", AlarmTime = now.AddDays(-3), AckTime = now.AddDays(-3).AddMinutes(30), AckBy = "Wang Fang", CloseTime = now.AddDays(-3).AddHours(5) },
        };
    }
}
