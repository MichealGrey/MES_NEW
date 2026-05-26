namespace MES.Modules.Production.Models;

public class EquipmentInfo
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentGroup { get; set; } = string.Empty;
    public string EquipmentType { get; set; } = string.Empty;
    public string Status { get; set; } = "Available"; // Available/Running/Maintenance/Offline
    public string? CurrentLotId { get; set; }
    public string? CurrentRecipe { get; set; }
    public List<string> SupportedRoutes { get; set; } = [];
    public List<string> SupportedSteps { get; set; } = [];
    public DateTime LastMaintenanceDate { get; set; }
    public int MaintenanceIntervalHours { get; set; }
    public int RunningHours { get; set; }
    public string Location { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
