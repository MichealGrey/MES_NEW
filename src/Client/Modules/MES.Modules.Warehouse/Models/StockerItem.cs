namespace MES.Modules.Warehouse.Models;

public class StockerItem
{
    public string LocationId { get; set; } = string.Empty;
    public string StockerId { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Status { get; set; } = "Empty";
    public string Carrier { get; set; } = string.Empty;
}
