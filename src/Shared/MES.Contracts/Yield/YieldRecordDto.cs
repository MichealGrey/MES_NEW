namespace MES.Contracts.Yield;

public class YieldRecordDto
{
    public string ProductId { get; set; } = string.Empty;
    public double LineYield { get; set; }
    public double DieYield { get; set; }
    public double TestYield { get; set; }
    public int DailyOutput { get; set; }
    public DateTime Date { get; set; }
}
