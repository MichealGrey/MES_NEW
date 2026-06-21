namespace MES.Modules.Quality.Models;

public class OocEventItem
{
    public string Id { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string Parameter { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public DateTime EventTime { get; set; }
    public string Status { get; set; } = "Pending";
    public string Severity { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public DateTime? ResponseTime { get; set; }
    public string ActionTaken { get; set; } = string.Empty;
}
