namespace MES.Modules.Production.Models;

public class LotCarrierBinding
{
    public string BindingId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string CarrierId { get; set; } = string.Empty;
    public string CarrierType { get; set; } = string.Empty;
    public string? FromCarrierId { get; set; }
    public DateTime BindTime { get; set; } = DateTime.UtcNow;
    public DateTime? UnbindTime { get; set; }
    public string OperatorId { get; set; } = string.Empty;
}
