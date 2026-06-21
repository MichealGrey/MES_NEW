using System.Text.Json.Serialization;
using MES.Domain.Production;

namespace MES.Modules.Order.Models;

public class WorkOrderInfo
{
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int PlannedQty { get; set; }
    public int CompletedQty { get; set; }
    public ProcessStatus Status { get; set; } = ProcessStatus.Created;
    public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Normal;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string RouteId { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public string DieName { get; set; } = string.Empty;
    public PackageType PackageType { get; set; } = PackageType.QFP;
    public int WaferQty { get; set; } = 25;
    public int UnitQty { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPN { get; set; } = string.Empty;
    public string InternalPN { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string SpecId { get; set; } = string.Empty;
    public string SpecVersion { get; set; } = string.Empty;
    public string TestProgram { get; set; } = string.Empty;
    public string BinSpec { get; set; } = string.Empty;
    public string GradeSpec { get; set; } = string.Empty;
    public string WaferSource { get; set; } = string.Empty;
    public string? SubconLotId { get; set; }
    public string Creator { get; set; } = string.Empty;
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string? HoldReason { get; set; }
    public double TargetCPYield { get; set; } = 99.0;
    public double TargetFTYield { get; set; } = 98.0;
    public double YieldTarget { get; set; }
    public string? Remark { get; set; }

    [JsonIgnore]
    public double ProgressPercent => PlannedQty > 0
        ? Math.Round((double)CompletedQty / PlannedQty * 100, 1)
        : 0;

    [JsonIgnore]
    public string PackageTypeDisplay => PackageType.ToString();
}
