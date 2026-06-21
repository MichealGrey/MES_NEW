namespace MES.Contracts.Phase2;

// ==================== 订单评审 DTOs ====================

public class CreateOrderRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSpec { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string Priority { get; set; } = "Normal";
    public string? PackageType { get; set; }
    public string? LeadFrameType { get; set; }
    public string? WireType { get; set; }
    public string? SpecialRequirements { get; set; }
    public string QualityLevel { get; set; } = "Commercial";
    public string? Remark { get; set; }
}

public class OrderResponse
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSpec { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalAmount { get; set; }
    public string Currency { get; set; } = "CNY";
    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string Priority { get; set; } = "Normal";
    public string Status { get; set; } = "Draft";
    public string ReviewStatus { get; set; } = "NotStarted";
    public string? ReviewResult { get; set; }
    public string? PackageType { get; set; }
    public string? LeadFrameType { get; set; }
    public string? WireType { get; set; }
    public string? SpecialRequirements { get; set; }
    public string QualityLevel { get; set; } = "Commercial";
    public string? Remark { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? CustomerId { get; set; }
    public string? Status { get; set; }
    public string? ReviewStatus { get; set; }
    public string? Priority { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class StartOrderReviewRequest
{
    public string ReviewType { get; set; } = "Standard";
    public DateTime? Deadline { get; set; }
}

public class OrderReviewResponse
{
    public string ReviewId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ReviewType { get; set; } = "Standard";
    public string Status { get; set; } = "Pending";
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? Deadline { get; set; }
    public string? InitiatedBy { get; set; }
    public string? Conclusion { get; set; }
    public string? Conditions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderReviewItemResponse> Items { get; set; } = new();
}

public class VoteReviewRequest
{
    public string Vote { get; set; } = string.Empty; // Approve/Reject/Conditional
    public string? Comments { get; set; }
    public string? Conditions { get; set; }
}

public class OrderReviewItemResponse
{
    public long ItemId { get; set; }
    public string ReviewId { get; set; } = string.Empty;
    public string ReviewerRole { get; set; } = string.Empty;
    public string? ReviewerName { get; set; }
    public string? Vote { get; set; }
    public string? Comments { get; set; }
    public string? Conditions { get; set; }
    public DateTime? ReviewTime { get; set; }
    public string Status { get; set; } = "Pending";
}

public class OrderVersionResponse
{
    public long VersionId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public int VersionNo { get; set; }
    public string? ChangeType { get; set; }
    public string? ChangeReason { get; set; }
    public string? ChangeDescription { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? ApprovedBy { get; set; }
}
