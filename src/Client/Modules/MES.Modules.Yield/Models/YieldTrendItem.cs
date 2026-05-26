namespace MES.Modules.Yield.Models;

public class YieldTrendItem
{
    public DateTime Date { get; set; }
    public double LineYield { get; set; }
    public double DieYield { get; set; }
    public double TestYield { get; set; }
    public int DailyOutput { get; set; }
}
