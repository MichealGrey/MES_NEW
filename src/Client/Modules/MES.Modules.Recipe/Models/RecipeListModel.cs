namespace MES.Modules.Recipe.Models;

public class RecipeListModel
{
    public string Id { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public string RecipeName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public string EquipmentType { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string CurrentEquipment { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public RecipeListModel Clone()
    {
        return new RecipeListModel
        {
            RecipeId = this.RecipeId,
            RecipeName = this.RecipeName,
            Version = this.Version,
            EquipmentType = this.EquipmentType,
            Description = this.Description
        };
    }
}
