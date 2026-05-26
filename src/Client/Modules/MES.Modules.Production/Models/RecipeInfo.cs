namespace MES.Modules.Production.Models;

public class RecipeInfo
{
    public string RecipeId { get; set; } = string.Empty;
    public string RecipeName { get; set; } = string.Empty;
    public string EquipmentGroup { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public bool IsActive { get; set; } = true;
    public string? Parameters { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public DateTime ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
