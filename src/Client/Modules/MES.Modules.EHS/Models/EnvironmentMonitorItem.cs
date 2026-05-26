namespace MES.Modules.EHS.Models;

public class EnvironmentMonitorItem
{
    public string Area { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public int Particles { get; set; }
    public string TempStatus { get; set; } = "Normal";
    public string HumidityStatus { get; set; } = "Normal";
    public string ParticleStatus { get; set; } = "Normal";
}
