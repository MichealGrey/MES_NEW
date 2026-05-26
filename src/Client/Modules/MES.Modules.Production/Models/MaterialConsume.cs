namespace MES.Modules.Production.Models;

public class MaterialConsume
{
    public string ConsumeId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public double ConsumedQty { get; set; }
    public string Unit { get; set; } = "pcs";
    public string? BatchNo { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime ConsumedAt { get; set; } = DateTime.UtcNow;
}
