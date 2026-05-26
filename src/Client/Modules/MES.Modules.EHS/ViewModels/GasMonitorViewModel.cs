using Prism.Mvvm;
using MES.Modules.EHS.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.EHS.ViewModels;

public class GasMonitorViewModel : BindableBase
{
    private ObservableCollection<GasMonitorItem> _items = [];
    private GasMonitorItem? _selectedItem;

    public ObservableCollection<GasMonitorItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public GasMonitorItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public GasMonitorViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<GasMonitorItem>
        {
            new() { GasType = "SiH4", Location = "Gas-Cabinet-1", Concentration = 0.5, Unit = "ppm", Threshold = 5.0, Status = "Normal" },
            new() { GasType = "PH3", Location = "Gas-Cabinet-1", Concentration = 0.1, Unit = "ppm", Threshold = 0.5, Status = "Normal" },
            new() { GasType = "AsH3", Location = "Gas-Cabinet-2", Concentration = 0.05, Unit = "ppm", Threshold = 0.05, Status = "Warning" },
            new() { GasType = "NF3", Location = "Gas-Cabinet-2", Concentration = 1.2, Unit = "ppm", Threshold = 10.0, Status = "Normal" },
            new() { GasType = "Cl2", Location = "Gas-Cabinet-3", Concentration = 0.3, Unit = "ppm", Threshold = 1.0, Status = "Normal" },
            new() { GasType = "H2", Location = "Gas-Cabinet-3", Concentration = 0.8, Unit = "ppm", Threshold = 4.0, Status = "Normal" },
            new() { GasType = "HF", Location = "Exhaust-1", Concentration = 1.5, Unit = "ppm", Threshold = 2.0, Status = "Warning" },
            new() { GasType = "HCl", Location = "Exhaust-2", Concentration = 0.4, Unit = "ppm", Threshold = 5.0, Status = "Normal" },
            new() { GasType = "CO", Location = "Fab-1/General", Concentration = 2.0, Unit = "ppm", Threshold = 25.0, Status = "Normal" },
            new() { GasType = "O3", Location = "Fab-1/General", Concentration = 0.05, Unit = "ppm", Threshold = 0.1, Status = "Normal" },
        };
    }
}
