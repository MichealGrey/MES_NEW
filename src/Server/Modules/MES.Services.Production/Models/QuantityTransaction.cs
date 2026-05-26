namespace MES.Services.Production.Models;

public class QuantityTransaction
{
    public string TransactionId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty; // TrackIn, TrackOut, Scrap, Return, Split, Merge
    public int Quantity { get; set; }
    public string Unit { get; set; } = "EA";
    public string? SourceStep { get; set; }
    public string? TargetStep { get; set; }
    public string? ReasonCode { get; set; }
    public string? OperatorId { get; set; }
    public DateTime TransactionTime { get; set; } = DateTime.UtcNow;
    public string? Remark { get; set; }
}
