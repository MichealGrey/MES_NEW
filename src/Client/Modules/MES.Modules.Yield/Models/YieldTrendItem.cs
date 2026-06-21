namespace MES.Modules.Yield.Models;

public class YieldTrendItem
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Product { get; set; } = string.Empty;
    public double Yield { get; set; }
    public double TargetYield { get; set; }
    public int TotalQty { get; set; }
    public int GoodQty { get; set; }
}
