using Prism.Mvvm;
using MES.Modules.Schedule.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Schedule.ViewModels;

public class CapacityAnalysisViewModel : BindableBase
{
    private ObservableCollection<CapacityItem> _items = [];
    private CapacityItem? _selectedItem;

    public ObservableCollection<CapacityItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public CapacityItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public CapacityAnalysisViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<CapacityItem>
        {
            new() { EquipmentId = "WS-01", EquipmentName = "切割机-1", Type = "DicingSaw", PlannedUtilization = 85.0, ActualUtilization = 82.5, WipCount = 12 },
            new() { EquipmentId = "WS-02", EquipmentName = "切割机-2", Type = "DicingSaw", PlannedUtilization = 85.0, ActualUtilization = 78.3, WipCount = 10 },
            new() { EquipmentId = "DB-01", EquipmentName = "贴片机-1", Type = "DieBonder", PlannedUtilization = 90.0, ActualUtilization = 91.2, WipCount = 18 },
            new() { EquipmentId = "DB-02", EquipmentName = "贴片机-2", Type = "DieBonder", PlannedUtilization = 90.0, ActualUtilization = 87.5, WipCount = 15 },
            new() { EquipmentId = "WB-01", EquipmentName = "焊线机-1", Type = "WireBonder", PlannedUtilization = 88.0, ActualUtilization = 86.8, WipCount = 22 },
            new() { EquipmentId = "MP-01", EquipmentName = "塑封机-1", Type = "MoldingPress", PlannedUtilization = 80.0, ActualUtilization = 82.1, WipCount = 8 },
            new() { EquipmentId = "TH-01", EquipmentName = "测试机-1", Type = "TestHandler", PlannedUtilization = 85.0, ActualUtilization = 83.4, WipCount = 15 },
            new() { EquipmentId = "TF-01", EquipmentName = "切筋成型机-1", Type = "TrimForm", PlannedUtilization = 75.0, ActualUtilization = 78.6, WipCount = 5 },
            new() { EquipmentId = "AOI-01", EquipmentName = "AOI检测机-1", Type = "AOI", PlannedUtilization = 60.0, ActualUtilization = 55.2, WipCount = 6 },
        };
    }
}
