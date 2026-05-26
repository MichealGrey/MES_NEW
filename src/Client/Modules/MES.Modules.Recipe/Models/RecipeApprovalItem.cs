namespace MES.Modules.Recipe.Models;

public class RecipeApprovalItem
{
    public string RecipeId { get; set; } = string.Empty;
    public string RecipeName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Submitter { get; set; } = string.Empty;
    public string Approver { get; set; } = string.Empty;
    public string Result { get; set; } = "Pending";
    public string Comment { get; set; } = string.Empty;
}
