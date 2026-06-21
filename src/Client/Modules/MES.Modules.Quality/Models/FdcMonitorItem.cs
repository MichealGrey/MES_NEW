namespace MES.Modules.Quality.Models;

public class FdcMonitorItem
{
    public string Id { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string Chamber { get; set; } = string.Empty;
    public double T2 { get; set; }
    public double SPE { get; set; }
    public string Status { get; set; } = "Normal";
    public string RunId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
