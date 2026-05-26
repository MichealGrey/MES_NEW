using Prism.Mvvm;
using MES.Modules.Quality.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Quality.ViewModels;

public class SpcChartViewModel : BindableBase
{
    private ObservableCollection<SpcChartItem> _items = [];
    private SpcChartItem? _selectedItem;

    public ObservableCollection<SpcChartItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public SpcChartItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public SpcChartViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<SpcChartItem>
        {
            new() { EquipmentId = "WB-01", Parameter = "WirePullStrength", UCL = 12.0, CL = 8.0, LCL = 4.0, LatestValue = 8.5 },
            new() { EquipmentId = "WB-01", Parameter = "BallShear", UCL = 35.0, CL = 25.0, LCL = 15.0, LatestValue = 26.3 },
            new() { EquipmentId = "WB-02", Parameter = "LoopHeight", UCL = 260, CL = 230, LCL = 200, LatestValue = 235 },
            new() { EquipmentId = "DB-01", Parameter = "DieShear", UCL = 45.0, CL = 30.0, LCL = 15.0, LatestValue = 32.1 },
            new() { EquipmentId = "MP-01", Parameter = "EpoxyVoid", UCL = 15.0, CL = 5.0, LCL = 0, LatestValue = 6.2 },
            new() { EquipmentId = "MP-01", Parameter = "PackageVoid", UCL = 10.0, CL = 3.0, LCL = 0, LatestValue = 3.5 },
            new() { EquipmentId = "MP-02", Parameter = "WireSweep", UCL = 8.0, CL = 4.0, LCL = 0, LatestValue = 4.8 },
            new() { EquipmentId = "WS-01", Parameter = "DieShear", UCL = 45.0, CL = 30.0, LCL = 15.0, LatestValue = 28.7 },
        };
    }
}
