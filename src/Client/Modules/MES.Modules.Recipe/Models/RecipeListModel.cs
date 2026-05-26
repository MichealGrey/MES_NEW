namespace MES.Modules.Recipe.Models;

public class RecipeListModel
{
    public string RecipeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public string EquipmentType { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}
