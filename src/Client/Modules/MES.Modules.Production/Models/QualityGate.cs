namespace MES.Modules.Production.Models;

public class QualityGate
{
    public string GateId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string GateType { get; set; } = "QACheck";
    public string Status { get; set; } = "Pending";
    public string? CheckedBy { get; set; }
    public string? CheckedByName { get; set; }
    public DateTime? CheckedAt { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpireAt { get; set; }
}
