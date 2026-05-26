namespace MES.Modules.Production.Models;

public class DispatchTask
{
    public string TaskId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string? RecipeId { get; set; }
    public int Qty { get; set; }
    public string Priority { get; set; } = "Normal"; // Urgent/High/Normal/Low
    public string Status { get; set; } = "Pending"; // Pending/Assigned/Running/Completed
    public string? AssignedOperator { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AssignedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double? DueHours { get; set; }
    public double? RemainingHours { get; set; }
    public bool IsOverdue { get; set; }
}
