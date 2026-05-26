namespace MES.Modules.Production.Models;

public class ProductionReport
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime ReportDate { get; set; }
    public string ReportType { get; set; } = "Daily"; // Daily/Weekly/Monthly
    public int TotalLots { get; set; }
    public int CompletedLots { get; set; }
    public int WipLots { get; set; }
    public int HoldLots { get; set; }
    public int TotalInputQty { get; set; }
    public int TotalOutputQty { get; set; }
    public int TotalScrapQty { get; set; }
    public double OverallYield { get; set; }
    public double FTYield { get; set; }
    public List<StepYieldData> StepYields { get; set; } = [];
    public List<EquipmentUtilization> EquipmentUtils { get; set; } = [];
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class StepYieldData
{
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public double Yield { get; set; }
}

public class EquipmentUtilization
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public double Utilization { get; set; }
    public int RunningHours { get; set; }
    public int IdleHours { get; set; }
    public int MaintenanceHours { get; set; }
}
