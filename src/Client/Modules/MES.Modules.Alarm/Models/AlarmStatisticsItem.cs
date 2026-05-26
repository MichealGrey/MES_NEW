namespace MES.Modules.Alarm.Models;

public class AlarmStatisticsItem
{
    public DateTime Date { get; set; }
    public int CriticalCount { get; set; }
    public int MajorCount { get; set; }
    public int MinorCount { get; set; }
    public int TotalCount { get; set; }
}
