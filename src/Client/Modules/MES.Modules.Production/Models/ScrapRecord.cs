namespace MES.Modules.Production.Models;

public class ScrapRecord
{
    public string ScrapId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public int ScrapQty { get; set; }
    public string ScrapReason { get; set; } = string.Empty;
    public string ScrapReasonCode { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public DateTime ScrapTime { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
    public bool RequiresApproval { get; set; }
}
