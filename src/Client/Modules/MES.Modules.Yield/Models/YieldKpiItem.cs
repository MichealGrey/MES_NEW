namespace MES.Modules.Yield.Models;

public class YieldKpiItem
{
    public string KpiName { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = "%";
    public string Trend { get; set; } = "Stable";
}
