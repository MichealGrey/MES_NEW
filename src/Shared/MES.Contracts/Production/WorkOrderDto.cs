namespace MES.Contracts.Production;

public class WorkOrderDto
{
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public int PlannedQty { get; set; }
    public int CompletedQty { get; set; }
    public string Status { get; set; } = "Created";
    public string Priority { get; set; } = "Normal";
    public string DieName { get; set; } = string.Empty;
    public string PackageType { get; set; } = "QFP";
    public int WaferQty { get; set; }
    public int UnitQty { get; set; }
    public string CustomerPN { get; set; } = string.Empty;
    public string InternalPN { get; set; } = string.Empty;
    public string TestProgram { get; set; } = string.Empty;
    public string? SubconLotId { get; set; }
    public double TargetCPYield { get; set; }
    public double TargetFTYield { get; set; }
    public DateTime CreatedAt { get; set; }
}
