namespace MES.Contracts.Recipe;

public class RecipeDto
{
    public string RecipeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public string Status { get; set; } = "Draft";
    public string EquipmentType { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
