namespace MES.Modules.Production.Models;

public class CustomerLotProgress
{
    public string LotId { get; set; } = string.Empty;
    public string? OrderId { get; set; }
    public string? ParentOrderId { get; set; }
    public string? WoType { get; set; } // Parent, Child-Assemble, Child-Test
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPN { get; set; } = string.Empty;
    public string InternalPN { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public int CurrentStepSeq { get; set; }
    public string Status { get; set; } = string.Empty;
    public int OriginalQty { get; set; }
    public int CurrentQty { get; set; }
    public int TotalPassQty { get; set; }
    public int TotalScrapQty { get; set; }
    public double YieldPercent { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? EstimatedEndDate { get; set; }
    public bool IsAtRisk { get; set; }
    public string? RiskReason { get; set; }
    public string? LastUpdateTime { get; set; }
    public List<string> CompletedSteps { get; set; } = new();
    public List<string> PendingSteps { get; set; } = new();
}
