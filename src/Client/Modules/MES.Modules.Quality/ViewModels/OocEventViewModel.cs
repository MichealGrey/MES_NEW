using Prism.Mvvm;
using MES.Modules.Quality.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Quality.ViewModels;

public class OocEventViewModel : BindableBase
{
    private ObservableCollection<OocEventItem> _items = [];
    private OocEventItem? _selectedItem;

    public ObservableCollection<OocEventItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public OocEventItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public OocEventViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<OocEventItem>
        {
            new() { EventId = "OOC-001", EquipmentId = "WB-01", Parameter = "WirePullStrength", Rule = "Rule 1", EventTime = DateTime.Now.AddMinutes(-10), Status = "Open" },
            new() { EventId = "OOC-002", EquipmentId = "MP-01", Parameter = "EpoxyVoid", Rule = "Rule 5", EventTime = DateTime.Now.AddMinutes(-25), Status = "Open" },
            new() { EventId = "OOC-003", EquipmentId = "WB-02", Parameter = "BallShear", Rule = "Rule 2", EventTime = DateTime.Now.AddHours(-1), Status = "Investigating" },
            new() { EventId = "OOC-004", EquipmentId = "AOI-01", Parameter = "PackageVoid", Rule = "Rule 1", EventTime = DateTime.Now.AddHours(-2), Status = "Resolved" },
            new() { EventId = "OOC-005", EquipmentId = "MP-02", Parameter = "WireSweep", Rule = "Rule 3", EventTime = DateTime.Now.AddHours(-3), Status = "Open" },
            new() { EventId = "OOC-006", EquipmentId = "DB-01", Parameter = "DieShear", Rule = "Rule 6", EventTime = DateTime.Now.AddHours(-5), Status = "Resolved" },
            new() { EventId = "OOC-007", EquipmentId = "WB-01", Parameter = "LoopHeight", Rule = "Rule 2", EventTime = DateTime.Now.AddHours(-8), Status = "Investigating" },
        };
    }
}
