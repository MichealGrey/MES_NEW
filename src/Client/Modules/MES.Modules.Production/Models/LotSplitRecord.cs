namespace MES.Modules.Production.Models;

public class LotSplitRecord
{
    public string SplitId { get; set; } = Guid.NewGuid().ToString("N");
    public string MotherLotId { get; set; } = string.Empty;
    public string ChildLotId { get; set; } = string.Empty;
    public int SplitQty { get; set; }
    public string SplitReason { get; set; } = string.Empty;
    public string SplitType { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime SplitTime { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
}
