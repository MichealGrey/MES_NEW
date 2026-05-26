using Prism.Mvvm;
using MES.Modules.Warehouse.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MES.Modules.Warehouse.ViewModels;

public class FoupManagementViewModel : BindableBase
{
    private ObservableCollection<FoupItem> _items = [];
    private FoupItem? _selectedItem;

    public ObservableCollection<FoupItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public FoupItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public FoupManagementViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        var locations = new[] { "STK-01/A01", "STK-01/B03", "WB-01/LL", "DB-01/LL", "STK-02/C02", "PKG1/Buffer" };
        var carrierTypes = new[] { "STRIP", "LF", "MAG", "TRAY", "REEL", "WF" };
        var cleanlinessLevels = new[] { "ISO Class 1", "ISO Class 2", "ISO Class 3" };
        var statuses = new[] { "In Use", "Empty", "Reserved", "Maintenance" };

        Items = new ObservableCollection<FoupItem>(
            Enumerable.Range(1, 12).Select(i => new FoupItem
            {
                CarrierId = $"{carrierTypes[i % 6]}-{i:D4}",
                Location = locations[i % locations.Length],
                LotId = i % 3 != 1 ? $"LOT-202600{i}" : "",
                Cleanliness = cleanlinessLevels[i % 3],
                StripCount = i % 3 != 1 ? 10 : 0,
                Status = statuses[i % 4]
            }));
    }
}
