namespace MES.Modules.EHS.Models;

public class ChemicalItem
{
    public string ChemicalName { get; set; } = string.Empty;
    public string CAS { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public string Unit { get; set; } = "L";
    public string Location { get; set; } = string.Empty;
    public string MsdsStatus { get; set; } = "Valid";
    public DateTime ExpiryDate { get; set; }
}
