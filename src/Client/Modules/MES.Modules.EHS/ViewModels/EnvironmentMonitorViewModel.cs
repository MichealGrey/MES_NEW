using Prism.Mvvm;
using MES.Modules.EHS.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.EHS.ViewModels;

public class EnvironmentMonitorViewModel : BindableBase
{
    private ObservableCollection<EnvironmentMonitorItem> _items = [];
    private EnvironmentMonitorItem? _selectedItem;

    public ObservableCollection<EnvironmentMonitorItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public EnvironmentMonitorItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public EnvironmentMonitorViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<EnvironmentMonitorItem>
        {
            new() { Area = "Fab-1/Bay-1", Temperature = 22.3, Humidity = 43.5, Particles = 12, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
            new() { Area = "Fab-1/Bay-2", Temperature = 22.8, Humidity = 44.1, Particles = 15, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
            new() { Area = "Fab-1/Bay-3", Temperature = 23.5, Humidity = 46.2, Particles = 28, TempStatus = "Warning", HumidityStatus = "Warning", ParticleStatus = "Normal" },
            new() { Area = "Fab-1/Bay-4", Temperature = 24.1, Humidity = 48.0, Particles = 45, TempStatus = "Alarm", HumidityStatus = "Alarm", ParticleStatus = "Warning" },
            new() { Area = "Fab-2/Bay-1", Temperature = 22.1, Humidity = 42.8, Particles = 8, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
            new() { Area = "Fab-2/Bay-2", Temperature = 22.5, Humidity = 43.2, Particles = 10, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
            new() { Area = "Fab-2/Bay-3", Temperature = 21.8, Humidity = 41.5, Particles = 52, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Alarm" },
            new() { Area = "Utility/Chiller", Temperature = 25.0, Humidity = 50.0, Particles = 5, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
        };
    }
}
