using Prism.Mvvm;
using MES.Modules.Warehouse.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Warehouse.ViewModels;

public class MaterialListViewModel : BindableBase
{
    private ObservableCollection<MaterialItem> _items = [];
    private MaterialItem? _selectedItem;

    public ObservableCollection<MaterialItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public MaterialItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public MaterialListViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<MaterialItem>
        {
            new() { MaterialId = "MAT-001", Name = "EMC G700", Type = "EMC", Quantity = 200.0, Unit = "kg", Location = "Mat-Store-A" },
            new() { MaterialId = "MAT-002", Name = "DAF L180", Type = "DAF", Quantity = 50.0, Unit = "roll", Location = "Mat-Store-A" },
            new() { MaterialId = "MAT-003", Name = "Gold Wire 25um", Type = "GoldWire", Quantity = 100.0, Unit = "spool", Location = "Mat-Store-B" },
            new() { MaterialId = "MAT-004", Name = "Copper Wire 20um", Type = "CopperWire", Quantity = 80.0, Unit = "spool", Location = "Mat-Store-B" },
            new() { MaterialId = "MAT-005", Name = "LeadFrame 16L-QFP", Type = "LeadFrame", Quantity = 5000.0, Unit = "pcs", Location = "Mat-Store-C" },
            new() { MaterialId = "MAT-006", Name = "Substrate BGA-256", Type = "Substrate", Quantity = 3000.0, Unit = "pcs", Location = "Mat-Store-C" },
            new() { MaterialId = "MAT-007", Name = "SolderBall 0.3mm", Type = "SolderBall", Quantity = 500.0, Unit = "kpcs", Location = "Mat-Store-D" },
            new() { MaterialId = "MAT-008", Name = "TapeReel 16mm", Type = "TapeReel", Quantity = 100.0, Unit = "roll", Location = "Mat-Store-D" },
            new() { MaterialId = "MAT-009", Name = "Tube 16L-DIP", Type = "Tube", Quantity = 2000.0, Unit = "pcs", Location = "Mat-Store-D" },
            new() { MaterialId = "MAT-010", Name = "Label QR-01", Type = "Label", Quantity = 10000.0, Unit = "pcs", Location = "Mat-Store-A" },
        };
    }
}
