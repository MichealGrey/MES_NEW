namespace MES.Modules.Recipe.Models;

public class RecipeParameterItem
{
    public string ParameterName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Min { get; set; } = string.Empty;
    public string Max { get; set; } = string.Empty;
}
