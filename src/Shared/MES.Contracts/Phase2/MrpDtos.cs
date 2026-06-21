namespace MES.Contracts.Phase2;

// ==================== BOM DTOs ====================

public class CreateBomRequest
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string BomVersion { get; set; } = "1.0";
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public List<BomItemRequest> Items { get; set; } = new();
}

public class BomItemRequest
{
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? MaterialSpec { get; set; }
    public decimal QuantityPerUnit { get; set; }
    public string? Unit { get; set; }
    public decimal LossRate { get; set; }
    public int SortOrder { get; set; }
    public string? Remark { get; set; }
}

public class BomResponse
{
    public string BomId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string BomVersion { get; set; } = "1.0";
    public string Status { get; set; } = "Active";
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int TotalItems { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<BomItemResponse> Items { get; set; } = new();
}

public class BomItemResponse
{
    public long ItemId { get; set; }
    public string BomId { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? MaterialSpec { get; set; }
    public decimal QuantityPerUnit { get; set; }
    public string? Unit { get; set; }
    public decimal LossRate { get; set; }
    public int SortOrder { get; set; }
    public string? Remark { get; set; }
}

// ==================== MRP DTOs ====================

public class MrpCalculationRequest
{
    public string? PlanId { get; set; }
    public string? WorkOrderId { get; set; }
    public string CalculationType { get; set; } = "Full";
    public string? CalculationParams { get; set; }
}

public class MrpCalculationResponse
{
    public string CalculationId { get; set; } = string.Empty;
    public string? PlanId { get; set; }
    public string? WorkOrderId { get; set; }
    public string CalculationType { get; set; } = "Full";
    public string Status { get; set; } = "Completed";
    public int? TotalDemandItems { get; set; }
    public int? ShortageItems { get; set; }
    public int? SufficientItems { get; set; }
    public string? ResultSummary { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MrpShortageWarningResponse
{
    public long WarningId { get; set; }
    public string CalculationId { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public decimal RequiredQty { get; set; }
    public decimal AvailableQty { get; set; }
    public decimal ShortageQty { get; set; }
    public DateTime? ExpectedArrival { get; set; }
    public string? PurchaseOrderNo { get; set; }
    public string Severity { get; set; } = "Medium";
    public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; }
}

public class MrpQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? CalculationId { get; set; }
    public string? Severity { get; set; }
    public string? Status { get; set; }
}
