namespace MES.Modules.EHS.Models;

/// <summary>
/// ESD (Electrostatic Discharge) monitoring data for workstations and personnel.
/// </summary>
public class EsdMonitorItem
{
    public string Id { get; set; } = string.Empty;
    public string StationId { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public double WriststrapResistance { get; set; }
    public string ResistanceUnit { get; set; } = "Mohm";
    public double FloorResistance { get; set; }
    public double IonizerBalance { get; set; }
    public string Status { get; set; } = "Normal";
    public DateTime LastTestTime { get; set; }
    public bool WriststrapTest { get; set; }
    public bool FloorTest { get; set; }
    public bool IonizerTest { get; set; }
}
