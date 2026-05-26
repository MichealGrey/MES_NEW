namespace MES.Modules.Production.Models;

public class ReworkRecord
{
    public string ReworkId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string OriginalRouteId { get; set; } = string.Empty;
    public string ReworkRouteId { get; set; } = string.Empty;
    public string FromStepCode { get; set; } = string.Empty;
    public string TargetStepCode { get; set; } = string.Empty;
    public int ReworkQty { get; set; }
    public string ReworkReason { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
    public int ReworkCount { get; set; }
}
