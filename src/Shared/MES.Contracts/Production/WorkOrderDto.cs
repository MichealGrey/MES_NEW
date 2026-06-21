namespace MES.Contracts.Production;

public class WorkOrderDto
{
    public string OrderId { get; set; } = string.Empty;
    public string WoType { get; set; } = "Parent";
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public int PlannedQty { get; set; }
    public int CompletedQty { get; set; }
    public int WaferQty { get; set; }
    public int UnitQty { get; set; }
    public string Status { get; set; } = "Created";
    public string Priority { get; set; } = "Normal";
    public string DieName { get; set; } = string.Empty;
    public string PackageType { get; set; } = "QFP";
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPn { get; set; } = string.Empty;
    public string InternalPn { get; set; } = string.Empty;
    public double TargetCpYield { get; set; }
    public double TargetFtYield { get; set; }
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateWorkOrderRequest
{
    public string WoType { get; set; } = "Parent";
    public string? ParentOrderId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public int PlannedQty { get; set; }
    public int WaferQty { get; set; }
    public int UnitQty { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public string? Remark { get; set; }
}

public class UpdateWorkOrderRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? Remark { get; set; }
}

