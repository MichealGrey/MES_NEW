namespace MES.Modules.Quality.Models;

public class OocEventItem
{
    public string EventId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string Parameter { get; set; } = string.Empty;
    public string Rule { get; set; } = string.Empty;
    public DateTime EventTime { get; set; }
    public string Status { get; set; } = "Open";
}
