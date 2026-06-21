namespace MES.Modules.Recipe.Models;

public class RecipeEquipmentItem
{
    public string Id { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string Status { get; set; } = "Unbound";
    public DateTime LastUsed { get; set; }
    public string BindDate { get; set; } = string.Empty;
}
