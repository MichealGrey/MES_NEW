namespace MES.Contracts.Production;

public class LotDto
{
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string CurrentEquipment { get; set; } = string.Empty;
    public string Status { get; set; } = "Waiting";
    public int UnitCount { get; set; }
    public int StripCount { get; set; }
    public string CarrierType { get; set; } = "Strip";
    public string CarrierId { get; set; } = string.Empty;
    public string? BinResult { get; set; }
    public string? TestResult { get; set; }
    public int QtyPass { get; set; }
    public int QtyFail { get; set; }
}
