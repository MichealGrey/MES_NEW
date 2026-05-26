using Prism.Mvvm;
using MES.Modules.Trace.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Trace.ViewModels;

public class ImpactAnalysisViewModel : BindableBase
{
    private ObservableCollection<ImpactAnalysisItem> _items = [];
    private ImpactAnalysisItem? _selectedItem;

    public ObservableCollection<ImpactAnalysisItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public ImpactAnalysisItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public ImpactAnalysisViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        var baseTime = DateTime.Now.AddHours(-3);
        Items = new ObservableCollection<ImpactAnalysisItem>
        {
            new() { LotId = "LOT-2026001", Product = "PRD-001", CurrentStep = "WIRE_BOND", EquipmentId = "WB-01", ProcessTime = baseTime, RiskLevel = "High" },
            new() { LotId = "LOT-2026002", Product = "PRD-001", CurrentStep = "MOLDING", EquipmentId = "MP-01", ProcessTime = baseTime.AddMinutes(-15), RiskLevel = "High" },
            new() { LotId = "LOT-2026003", Product = "PRD-002", CurrentStep = "DIE_ATTACH", EquipmentId = "DB-01", ProcessTime = baseTime.AddMinutes(-30), RiskLevel = "Medium" },
            new() { LotId = "LOT-2026005", Product = "PRD-003", CurrentStep = "WIRE_BOND", EquipmentId = "WB-02", ProcessTime = baseTime.AddHours(-1), RiskLevel = "Medium" },
            new() { LotId = "LOT-2026006", Product = "PRD-002", CurrentStep = "TRIM_FORM", EquipmentId = "TF-01", ProcessTime = baseTime.AddHours(-1).AddMinutes(-30), RiskLevel = "Low" },
            new() { LotId = "LOT-2026008", Product = "PRD-001", CurrentStep = "TEST_CP", EquipmentId = "TH-01", ProcessTime = baseTime.AddHours(-2), RiskLevel = "Low" },
            new() { LotId = "LOT-2026010", Product = "PRD-005", CurrentStep = "DICING", EquipmentId = "WS-01", ProcessTime = baseTime.AddHours(-2).AddMinutes(-30), RiskLevel = "Low" },
            new() { LotId = "LOT-2026012", Product = "PRD-003", CurrentStep = "AOI_INSPECT", EquipmentId = "AOI-01", ProcessTime = baseTime.AddHours(-3), RiskLevel = "Low" },
        };
    }
}
