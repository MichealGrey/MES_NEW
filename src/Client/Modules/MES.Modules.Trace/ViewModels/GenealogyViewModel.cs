using Prism.Mvvm;
using MES.Modules.Trace.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Trace.ViewModels;

public class GenealogyViewModel : BindableBase
{
    private ObservableCollection<GenealogyItem> _items = [];
    private GenealogyItem? _selectedItem;

    public ObservableCollection<GenealogyItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public GenealogyItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public GenealogyViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        var baseTime = DateTime.Today.AddDays(-3);
        Items = new ObservableCollection<GenealogyItem>
        {
            new() { LotId = "LOT-2026001", ParentLotId = "", Product = "PRD-001", Operation = "Start", Time = baseTime, StripCount = 10 },
            new() { LotId = "LOT-2026001A", ParentLotId = "LOT-2026001", Product = "PRD-001", Operation = "StripSplit", Time = baseTime.AddHours(8), StripCount = 5 },
            new() { LotId = "LOT-2026001B", ParentLotId = "LOT-2026001", Product = "PRD-001", Operation = "StripSplit", Time = baseTime.AddHours(8), StripCount = 5 },
            new() { LotId = "LOT-2026002", ParentLotId = "", Product = "PRD-002", Operation = "Start", Time = baseTime.AddHours(2), StripCount = 10 },
            new() { LotId = "LOT-2026002A", ParentLotId = "LOT-2026002", Product = "PRD-002", Operation = "StripSplit", Time = baseTime.AddHours(12), StripCount = 10 },
            new() { LotId = "LOT-2026003", ParentLotId = "LOT-2026001A", Product = "PRD-001", Operation = "Merge", Time = baseTime.AddDays(-2), StripCount = 10 },
            new() { LotId = "LOT-2026003", ParentLotId = "LOT-2026001B", Product = "PRD-001", Operation = "Merge", Time = baseTime.AddDays(-2), StripCount = 10 },
            new() { LotId = "LOT-2026004", ParentLotId = "LOT-2026002A", Product = "PRD-002", Operation = "Rework", Time = baseTime.AddDays(-1), StripCount = 2 },
            new() { LotId = "MAT-GW-B001", ParentLotId = "", Product = "GoldWire-25um", Operation = "WireBatchIn", Time = baseTime.AddHours(3), StripCount = 1 },
            new() { LotId = "MAT-EMC-B001", ParentLotId = "", Product = "EMC-G700", Operation = "EMCBatchIn", Time = baseTime.AddHours(5), StripCount = 1 },
        };
    }
}
