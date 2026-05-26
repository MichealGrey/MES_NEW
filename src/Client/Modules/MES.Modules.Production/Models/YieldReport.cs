namespace MES.Modules.Production.Models;

public class YieldReport
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString("N");
    public string RouteId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalLots { get; set; }
    public double AverageYield { get; set; }
    public double MinYield { get; set; }
    public double MaxYield { get; set; }
    public double StdDev { get; set; }
    public List<StepYieldTrend> StepTrends { get; set; } = [];
    public List<YieldAlert> Alerts { get; set; } = [];
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class StepYieldTrend
{
    public string StepCode { get; set; } = string.Empty;
    public List<DailyYield> DailyData { get; set; } = [];
}

public class DailyYield
{
    public DateTime Date { get; set; }
    public double Yield { get; set; }
    public int LotCount { get; set; }
}

public class YieldAlert
{
    public string StepCode { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty; // LowYield/HighVariation/TrendDown
    public string Message { get; set; } = string.Empty;
    public DateTime TriggeredAt { get; set; }
}
