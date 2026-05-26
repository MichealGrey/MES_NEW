namespace MES.Modules.Schedule.Models;

public class CapacityItem
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double PlannedUtilization { get; set; }
    public double ActualUtilization { get; set; }
    public int WipCount { get; set; }
}
