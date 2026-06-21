namespace MES.Modules.Recipe.Models;

public class RecipeApprovalItem
{
    public string Id { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public string RecipeName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime SubmittedDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string Reviewer { get; set; } = string.Empty;
    public DateTime? ApprovedDate { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}
