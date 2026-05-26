namespace MES.Modules.Production.Models;

public class DueDateRisk
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPN { get; set; } = string.Empty;
    public DateTime PlannedEndDate { get; set; }
    public DateTime? EstimatedEndDate { get; set; }
    public int PlannedQty { get; set; }
    public int CompletedQty { get; set; }
    public double ProgressPercent { get; set; }
    public string RiskLevel { get; set; } = "Green"; // Green/Yellow/Red/Critical
    public int DaysRemaining { get; set; }
    public int EstimatedDaysNeeded { get; set; }
    public List<string> RiskFactors { get; set; } = new();
    public string? Recommendation { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}
