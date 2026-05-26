namespace MES.Modules.Equipment.Models;

public class EquipmentInfo
{
    public string EquipmentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Status { get; set; } = "Idle";
    public string? CurrentLot { get; set; }
    public string? RecipeId { get; set; }
}
