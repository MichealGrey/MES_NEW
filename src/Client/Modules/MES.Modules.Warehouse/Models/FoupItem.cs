namespace MES.Modules.Warehouse.Models;

public class FoupItem
{
    public string CarrierId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string Cleanliness { get; set; } = string.Empty;
    public int StripCount { get; set; }
    public string Status { get; set; } = "In Use";
}
