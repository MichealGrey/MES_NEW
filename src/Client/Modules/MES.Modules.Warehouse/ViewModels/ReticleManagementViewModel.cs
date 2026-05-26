using Prism.Mvvm;
using MES.Modules.Warehouse.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Warehouse.ViewModels;

public class ReticleManagementViewModel : BindableBase
{
    private ObservableCollection<ReticleItem> _items = [];
    private ReticleItem? _selectedItem;

    public ObservableCollection<ReticleItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public ReticleItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public ReticleManagementViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<ReticleItem>
        {
            new() { ReticleId = "TL-MC-001", Type = "MoldCavity", UsageCount = 1250, Status = "Available", Location = "Tooling-Stock-A/01" },
            new() { ReticleId = "TL-BC-001", Type = "BondCapillary", UsageCount = 890, Status = "In Use", Location = "WB-01/Capillary-Head" },
            new() { ReticleId = "TL-SB-001", Type = "SawBlade", UsageCount = 2100, Status = "Cleaning", Location = "Tooling-Cleaner" },
            new() { ReticleId = "TL-TS-001", Type = "TestSocket", UsageCount = 3400, Status = "Inspection", Location = "Tooling-Inspection" },
            new() { ReticleId = "TL-MC-002", Type = "MoldCavity", UsageCount = 560, Status = "Available", Location = "Tooling-Stock-A/05" },
            new() { ReticleId = "TL-BC-002", Type = "BondCapillary", UsageCount = 780, Status = "In Use", Location = "WB-02/Capillary-Head" },
            new() { ReticleId = "TL-SB-002", Type = "SawBlade", UsageCount = 4500, Status = "End of Life", Location = "Tooling-Stock-B/12" },
            new() { ReticleId = "TL-TS-002", Type = "TestSocket", UsageCount = 320, Status = "Available", Location = "Tooling-Stock-A/08" },
        };
    }
}
