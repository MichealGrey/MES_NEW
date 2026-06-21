namespace MES.Modules.Recipe.Models;

public class RecipeParameterItem
{
    public string Id { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public double TargetValue { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string DataType { get; set; } = "Numeric";
    public bool IsRequired { get; set; } = true;
}
