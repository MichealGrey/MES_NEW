namespace MES.Contracts.Phase2;

// ==================== 订单进度 DTOs ====================

public class OrderProgressResponse
{
    public string OrderId { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public int CompletedQuantity { get; set; }
    public int InProgressQuantity { get; set; }
    public int PendingQuantity { get; set; }
    public int DefectiveQuantity { get; set; }
    public decimal? YieldRate { get; set; }
    public decimal? ProgressPercentage { get; set; }
    public string? CurrentStage { get; set; }
    public DateTime? EstimatedCompletion { get; set; }
    public bool IsDelayed { get; set; }
    public int DelayDays { get; set; }
    public int WorkOrderCount { get; set; }
    public int CompletedWorkOrderCount { get; set; }
    public DateTime SnapshotTime { get; set; }
}

public class OtdStatisticsResponse
{
    public long StatId { get; set; }
    public string StatPeriod { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public int OnTimeOrders { get; set; }
    public int LateOrders { get; set; }
    public decimal? OtdRate { get; set; }
    public decimal? AvgDelayDays { get; set; }
    public int? MaxDelayDays { get; set; }
    public int TotalQuantity { get; set; }
    public int OnTimeQuantity { get; set; }
}

public class DelayReasonRecordRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string? WorkOrderId { get; set; }
    public string DelayReasonCategory { get; set; } = string.Empty;
    public string? DelayReasonDetail { get; set; }
    public int DelayDays { get; set; }
    public int? ImpactQuantity { get; set; }
    public string? ResponsibleDept { get; set; }
    public string? CorrectiveAction { get; set; }
    public string? PreventiveAction { get; set; }
}

public class DelayReasonRecordResponse
{
    public long RecordId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string? WorkOrderId { get; set; }
    public string DelayReasonCategory { get; set; } = string.Empty;
    public string? DelayReasonDetail { get; set; }
    public int DelayDays { get; set; }
    public int? ImpactQuantity { get; set; }
    public string? ResponsibleDept { get; set; }
    public string? CorrectiveAction { get; set; }
    public string? PreventiveAction { get; set; }
    public string? ReportedBy { get; set; }
    public DateTime ReportedAt { get; set; }
}

// ==================== 插单处理 DTOs ====================

public class CreateRushOrderRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int RushQuantity { get; set; }
    public DateTime RequiredDate { get; set; }
    public string? RushReason { get; set; }
    public string PriorityLevel { get; set; } = "Urgent";
}

public class RushOrderResponse
{
    public string RequestId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int RushQuantity { get; set; }
    public DateTime RequiredDate { get; set; }
    public string? RushReason { get; set; }
    public string PriorityLevel { get; set; } = "Urgent";
    public string Status { get; set; } = "Pending";
    public bool ImpactAnalysisDone { get; set; }
    public string? AnalysisSummary { get; set; }
    public string? ApprovalResult { get; set; }
    public string? ApprovalBy { get; set; }
    public DateTime? ApprovalAt { get; set; }
    public string? ApprovalComments { get; set; }
    public string? ExecutedBy { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RushOrderImpactResponse
{
    public long ImpactId { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public string AffectedOrderId { get; set; } = string.Empty;
    public string? AffectedWorkOrderId { get; set; }
    public string? ImpactType { get; set; }
    public string? ImpactDescription { get; set; }
    public DateTime? OriginalDeliveryDate { get; set; }
    public DateTime? NewEstimatedDelivery { get; set; }
    public int? DelayDays { get; set; }
    public string Severity { get; set; } = "Medium";
    public string? MitigationPlan { get; set; }
    public DateTime AnalyzedAt { get; set; }
}

public class ApproveRushOrderRequest
{
    public string ApprovalResult { get; set; } = string.Empty;
    public string? ApprovalComments { get; set; }
}

public class RushOrderQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
    public string? PriorityLevel { get; set; }
}
