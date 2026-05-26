namespace MES.Modules.Production.Models;

public class HoldRecord
{
    public string HoldId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;

    public string HoldType { get; set; } = string.Empty;
    public string HoldReasonCode { get; set; } = string.Empty;
    public string HoldReason { get; set; } = string.Empty;
    public int HoldQty { get; set; }

    public string ResponsibleDept { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;

    public string Status { get; set; } = "Open"; // Open, Released, Escalated, Closed

    public string HoldBy { get; set; } = string.Empty;
    public DateTime HoldTime { get; set; }

    public string? RootCause { get; set; }
    public string? CorrectiveAction { get; set; }
    public string? Disposition { get; set; }

    public string? ReleaseBy { get; set; }
    public DateTime? ReleaseTime { get; set; }
    public string? ReleaseComment { get; set; }
    public string? ApprovedBy { get; set; }
}
