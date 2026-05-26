namespace MES.Modules.Production.Models;

public class LotMergeRecord
{
    public string MergeId { get; set; } = Guid.NewGuid().ToString("N");
    public string TargetLotId { get; set; } = string.Empty;
    public List<string> SourceLotIds { get; set; } = [];
    public int MergedQty { get; set; }
    public string MergeReason { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime MergeTime { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
}
