using Prism.Mvvm;
using MES.Modules.Schedule.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Schedule.ViewModels;

public class DispatchBoardViewModel : BindableBase
{
    private ObservableCollection<DispatchQueueItem> _items = [];
    private DispatchQueueItem? _selectedItem;

    public ObservableCollection<DispatchQueueItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public DispatchQueueItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public DispatchBoardViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<DispatchQueueItem>
        {
            new() { EquipmentId = "DB-01", LotId = "LOT-2026001", Product = "PRD-001", Priority = 1, Step = "DIE_ATTACH", RecipeId = "RCP-001" },
            new() { EquipmentId = "DB-01", LotId = "LOT-2026003", Product = "PRD-002", Priority = 3, Step = "DIE_ATTACH", RecipeId = "RCP-002" },
            new() { EquipmentId = "WB-01", LotId = "LOT-2026002", Product = "PRD-001", Priority = 1, Step = "WIRE_BOND", RecipeId = "RCP-003" },
            new() { EquipmentId = "WB-01", LotId = "LOT-2026005", Product = "PRD-003", Priority = 2, Step = "WIRE_BOND", RecipeId = "RCP-004" },
            new() { EquipmentId = "MP-01", LotId = "LOT-2026004", Product = "PRD-001", Priority = 1, Step = "MOLDING", RecipeId = "RCP-005" },
            new() { EquipmentId = "MK-01", LotId = "LOT-2026006", Product = "PRD-002", Priority = 2, Step = "LASER_MARK", RecipeId = "RCP-006" },
            new() { EquipmentId = "TF-01", LotId = "LOT-2026007", Product = "PRD-004", Priority = 1, Step = "TRIM_FORM", RecipeId = "RCP-007" },
            new() { EquipmentId = "TH-01", LotId = "LOT-2026008", Product = "PRD-001", Priority = 4, Step = "TEST_CP", RecipeId = "RCP-008" },
            new() { EquipmentId = "TH-01", LotId = "LOT-2026010", Product = "PRD-005", Priority = 2, Step = "TEST_FT", RecipeId = "RCP-009" },
            new() { EquipmentId = "AOI-01", LotId = "LOT-2026009", Product = "PRD-003", Priority = 1, Step = "AOI_INSPECT", RecipeId = "RCP-010" },
        };
    }
}
