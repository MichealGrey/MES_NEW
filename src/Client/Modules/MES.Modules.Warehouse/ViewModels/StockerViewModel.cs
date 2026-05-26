using Prism.Mvvm;
using MES.Modules.Warehouse.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MES.Modules.Warehouse.ViewModels;

public class StockerViewModel : BindableBase
{
    private ObservableCollection<StockerItem> _items = [];
    private StockerItem? _selectedItem;

    public ObservableCollection<StockerItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public StockerItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public StockerViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        var stockers = new[] { "STK-01", "STK-02", "STK-03" };
        var statuses = new[] { "Occupied", "Empty", "Reserved", "Error" };

        Items = new ObservableCollection<StockerItem>(
            Enumerable.Range(1, 20).Select(i => new StockerItem
            {
                LocationId = $"LOC-{i:D3}",
                StockerId = stockers[i % 3],
                Port = $"P{(i / 3) + 1:D2}",
                Status = statuses[i % 4],
                Carrier = statuses[i % 4] == "Occupied" ? $"CAR-{i:D4}" : ""
            }));
    }
}
