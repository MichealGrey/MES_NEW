using Prism.Mvvm;
using MES.Modules.Quality.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Quality.ViewModels;

public class InspectionViewModel : BindableBase
{
    private ObservableCollection<InspectionItem> _items = [];
    private InspectionItem? _selectedItem;

    public ObservableCollection<InspectionItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public InspectionItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public InspectionViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<InspectionItem>
        {
            new() { InspectionId = "INS-001", LotId = "LOT-2026001", Product = "PRD-001", InspectionType = "AOI", Result = "Pass", Inspector = "Zhang Wei", InspectTime = DateTime.Now.AddMinutes(-30) },
            new() { InspectionId = "INS-002", LotId = "LOT-2026002", Product = "PRD-002", InspectionType = "XRay", Result = "Fail", Inspector = "Li Ming", InspectTime = DateTime.Now.AddMinutes(-45) },
            new() { InspectionId = "INS-003", LotId = "LOT-2026003", Product = "PRD-001", InspectionType = "VisualInspect", Result = "Pass", Inspector = "Wang Fang", InspectTime = DateTime.Now.AddHours(-1) },
            new() { InspectionId = "INS-004", LotId = "LOT-2026005", Product = "PRD-003", InspectionType = "CrossSection", Result = "Conditional", Inspector = "Liu Jun", InspectTime = DateTime.Now.AddHours(-2) },
            new() { InspectionId = "INS-005", LotId = "LOT-2026008", Product = "PRD-002", InspectionType = "SAM", Result = "Pass", Inspector = "Chen Lei", InspectTime = DateTime.Now.AddHours(-3) },
            new() { InspectionId = "INS-006", LotId = "LOT-2026010", Product = "PRD-004", InspectionType = "ElectricalTest", Result = "Pass", Inspector = "Zhang Wei", InspectTime = DateTime.Now.AddHours(-4) },
            new() { InspectionId = "INS-007", LotId = "LOT-2026012", Product = "PRD-001", InspectionType = "AOI", Result = "Fail", Inspector = "Li Ming", InspectTime = DateTime.Now.AddHours(-5) },
            new() { InspectionId = "INS-008", LotId = "LOT-2026015", Product = "PRD-005", InspectionType = "OpenShort", Result = "Pass", Inspector = "Wang Fang", InspectTime = DateTime.Now.AddHours(-6) },
        };
    }
}
