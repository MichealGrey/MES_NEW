namespace MES.Modules.Quality.Models;

public class SpcChartItem
{
    public string EquipmentId { get; set; } = string.Empty;
    public string Parameter { get; set; } = string.Empty;
    public double UCL { get; set; }
    public double CL { get; set; }
    public double LCL { get; set; }
    public double LatestValue { get; set; }
}
