namespace MES.Modules.Warehouse.Models;

public class ReticleItem
{
    public string ReticleId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public string Status { get; set; } = "Available";
    public string Location { get; set; } = string.Empty;
}
