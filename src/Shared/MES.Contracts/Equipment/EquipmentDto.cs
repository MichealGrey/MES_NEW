namespace MES.Contracts.Equipment;

public class EquipmentDto
{
    public string EquipmentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Status { get; set; } = "Idle";
    public string? CurrentLot { get; set; }
    public string? RecipeId { get; set; }
    public DateTime? LastPmTime { get; set; }
}
