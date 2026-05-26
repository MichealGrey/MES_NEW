namespace MES.Modules.EHS.Models;

public class GasMonitorItem
{
    public string GasType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double Concentration { get; set; }
    public string Unit { get; set; } = "ppm";
    public double Threshold { get; set; }
    public string Status { get; set; } = "Normal";
}
