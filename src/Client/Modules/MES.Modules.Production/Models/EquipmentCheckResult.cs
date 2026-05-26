namespace MES.Modules.Production.Models;

public class EquipmentCheckResult
{
    public bool IsAllowed { get; set; }
    public string? Reason { get; set; }
    public string EquipmentStatus { get; set; } = string.Empty;
    public bool IsRecipeMatch { get; set; }
    public bool IsInServiceWindow { get; set; }
}
