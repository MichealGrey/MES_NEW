namespace MES.Modules.Yield.Models;

public class YieldKpiItem
{
    public string KpiName { get; set; } = string.Empty;
    public double CurrentValue { get; set; }
    public double TargetValue { get; set; }
    public string Trend { get; set; } = "Stable";
}
