namespace MES.Contracts.Warehouse;

public class MaterialDto
{
    public string MaterialId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}
