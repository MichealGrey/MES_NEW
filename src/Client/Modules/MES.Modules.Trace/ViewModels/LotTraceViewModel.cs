using Prism.Mvvm;
using MES.Modules.Trace.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Trace.ViewModels;

public class LotTraceViewModel : BindableBase
{
    private ObservableCollection<LotTraceItem> _items = [];
    private LotTraceItem? _selectedItem;

    public ObservableCollection<LotTraceItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public LotTraceItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public LotTraceViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        var baseTime = DateTime.Today.AddHours(8);
        Items = new ObservableCollection<LotTraceItem>
        {
            new() { Step = 1, ProcessName = "Wafer Mount", EquipmentId = "WS-01", RecipeId = "RCP-001", StartTime = baseTime, EndTime = baseTime.AddMinutes(15), Result = "Pass" },
            new() { Step = 2, ProcessName = "Dicing", EquipmentId = "WS-01", RecipeId = "RCP-002", StartTime = baseTime.AddMinutes(20), EndTime = baseTime.AddMinutes(50), Result = "Pass" },
            new() { Step = 3, ProcessName = "Wafer Wash", EquipmentId = "PC-01", RecipeId = "RCP-003", StartTime = baseTime.AddMinutes(55), EndTime = baseTime.AddHours(1).AddMinutes(10), Result = "Pass" },
            new() { Step = 4, ProcessName = "Die Attach", EquipmentId = "DB-01", RecipeId = "RCP-004", StartTime = baseTime.AddHours(1).AddMinutes(15), EndTime = baseTime.AddHours(2), Result = "Pass" },
            new() { Step = 5, ProcessName = "Wire Bond", EquipmentId = "WB-01", RecipeId = "RCP-005", StartTime = baseTime.AddHours(2).AddMinutes(5), EndTime = baseTime.AddHours(3).AddMinutes(30), Result = "Pass" },
            new() { Step = 6, ProcessName = "Molding", EquipmentId = "MP-01", RecipeId = "RCP-006", StartTime = baseTime.AddHours(3).AddMinutes(35), EndTime = baseTime.AddHours(4).AddMinutes(30), Result = "Pass" },
            new() { Step = 7, ProcessName = "Post Mold Cure", EquipmentId = "MP-01", RecipeId = "RCP-007", StartTime = baseTime.AddHours(4).AddMinutes(35), EndTime = baseTime.AddHours(5).AddMinutes(10), Result = "Pass" },
            new() { Step = 8, ProcessName = "Laser Mark", EquipmentId = "MK-01", RecipeId = "RCP-008", StartTime = baseTime.AddHours(5).AddMinutes(15), EndTime = baseTime.AddHours(5).AddMinutes(30), Result = "Pass" },
            new() { Step = 9, ProcessName = "Trim Form", EquipmentId = "TF-01", RecipeId = "RCP-009", StartTime = baseTime.AddHours(5).AddMinutes(35), EndTime = baseTime.AddHours(6).AddMinutes(20), Result = "Fail" },
            new() { Step = 10, ProcessName = "Test CP", EquipmentId = "TH-01", RecipeId = "RCP-010", StartTime = baseTime.AddHours(6).AddMinutes(25), EndTime = baseTime.AddHours(7).AddMinutes(30), Result = "Pass" },
        };
    }
}
