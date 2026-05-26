using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MES.Modules.Equipment.ViewModels;

public class PortInfo
{
    public int PortId { get; set; }
    public string Status { get; set; } = "Empty";
    public string? CarrierId { get; set; }
}

public class EapControlViewModel : BindableBase
{
    private string _equipmentId = "WB-01";
    private bool _isOnline = true;
    private ObservableCollection<PortInfo> _ports = [];

    public string EquipmentId { get => _equipmentId; set => SetProperty(ref _equipmentId, value); }
    public bool IsOnline { get => _isOnline; set => SetProperty(ref _isOnline, value); }
    public ObservableCollection<PortInfo> Ports { get => _ports; set => SetProperty(ref _ports, value); }

    public EapControlViewModel()
    {
        Ports = new ObservableCollection<PortInfo>
        {
            new() { PortId = 1, Status = "Loaded", CarrierId = "STRIP-001" },
            new() { PortId = 2, Status = "Loaded", CarrierId = "LF-001" },
            new() { PortId = 3, Status = "Empty" },
            new() { PortId = 4, Status = "Empty" },
        };
    }
}
