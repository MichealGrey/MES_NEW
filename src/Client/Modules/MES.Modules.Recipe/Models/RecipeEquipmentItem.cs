namespace MES.Modules.Recipe.Models;

public class RecipeEquipmentItem
{
    public string EquipmentType { get; set; } = string.Empty;
    public int RecipeCount { get; set; }
    public string BindingStatus { get; set; } = string.Empty;
}
