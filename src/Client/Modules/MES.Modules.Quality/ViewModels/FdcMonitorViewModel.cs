using Prism.Mvvm;
using MES.Modules.Quality.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Quality.ViewModels;

public class FdcMonitorViewModel : BindableBase
{
    private ObservableCollection<FdcMonitorItem> _items = [];
    private FdcMonitorItem? _selectedItem;

    public ObservableCollection<FdcMonitorItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public FdcMonitorItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public FdcMonitorViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<FdcMonitorItem>
        {
            new() { EquipmentId = "WB-01", Chamber = "Head-1", T2 = 12.5, SPE = 3.2, Status = "Normal" },
            new() { EquipmentId = "WB-01", Chamber = "Head-2", T2 = 28.7, SPE = 8.9, Status = "Warning" },
            new() { EquipmentId = "WB-02", Chamber = "Head-1", T2 = 45.1, SPE = 15.3, Status = "Alarm" },
            new() { EquipmentId = "MP-01", Chamber = "Cavity-1", T2 = 8.3, SPE = 2.1, Status = "Normal" },
            new() { EquipmentId = "MP-01", Chamber = "Cavity-2", T2 = 22.4, SPE = 6.5, Status = "Warning" },
            new() { EquipmentId = "DB-01", Chamber = "Head-1", T2 = 5.8, SPE = 1.4, Status = "Normal" },
            new() { EquipmentId = "TH-01", Chamber = "Site-1", T2 = 9.1, SPE = 2.8, Status = "Normal" },
            new() { EquipmentId = "MP-02", Chamber = "Cavity-1", T2 = 35.6, SPE = 11.2, Status = "Alarm" },
            new() { EquipmentId = "WS-01", Chamber = "Spindle-1", T2 = 15.2, SPE = 4.1, Status = "Normal" },
        };
    }
}
