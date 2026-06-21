namespace MES.Modules.Schedule.Models;

public class WorkOrderScheduleItem
{
    public string WorkOrderNo { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int PlanQty { get; set; }
    public int CompletedQty { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime PlanEndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public double ProgressPercent => PlanQty > 0 ? (double)CompletedQty / PlanQty * 100 : 0;
}

public class MrpItem
{
    public string MaterialNo { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public int RequiredQty { get; set; }
    public int AvailableQty { get; set; }
    public int ShortageQty { get; set; }
    public string Supplier { get; set; } = string.Empty;
    public int LeadTime { get; set; }
}

public class DeliveryRecord
{
    public string Id { get; set; } = string.Empty;
    public string WorkOrderNo { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int PlanQty { get; set; }
    public int DeliverQty { get; set; }
    public DateTime PlanDeliveryDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CapacityBalanceItem
{
    public string ProcessName { get; set; } = string.Empty;
    public double DemandHours { get; set; }
    public double AvailableHours { get; set; }
    public double BalanceHours { get; set; }
    public double Utilization { get; set; }
}
