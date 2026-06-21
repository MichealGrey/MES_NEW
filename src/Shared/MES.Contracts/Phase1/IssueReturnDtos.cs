using MES.Contracts.Common;

namespace MES.Contracts.Phase1;

// ============================================================================
// 物料领用/退料 DTO
// ============================================================================

/// <summary>
/// 领料请求
/// </summary>
public class IssueMaterialRequest
{
    /// <summary>工单ID</summary>
    public string WorkOrderId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string? LotId { get; set; }

    /// <summary>工序代码</summary>
    public string? StepCode { get; set; }

    /// <summary>领料项目列表</summary>
    public List<IssueItemRequest> Items { get; set; } = [];

    /// <summary>发料人</summary>
    public string IssuedBy { get; set; } = string.Empty;

    /// <summary>收货人ID</summary>
    public string ReceiverId { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 领料项目请求
/// </summary>
public class IssueItemRequest
{
    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>请求数量</summary>
    public int RequestedQty { get; set; }

    /// <summary>指定批次ID</summary>
    public string? BatchId { get; set; }

    /// <summary>是否跳过FIFO</summary>
    public bool SkipFifo { get; set; }

    /// <summary>跳过FIFO原因</summary>
    public string? SkipFifoReason { get; set; }
}

/// <summary>
/// 领料单响应
/// </summary>
public class IssueOrderResponse
{
    /// <summary>领料单ID</summary>
    public string IssueOrderId { get; set; } = string.Empty;

    /// <summary>工单ID</summary>
    public string WorkOrderId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string? LotId { get; set; }

    /// <summary>领料项目列表</summary>
    public List<IssueItemResponse> Items { get; set; } = [];

    /// <summary>状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>发料时间</summary>
    public DateTime IssuedAt { get; set; }
}

/// <summary>
/// 领料项目响应
/// </summary>
public class IssueItemResponse
{
    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>物料名称</summary>
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>请求数量</summary>
    public int RequestedQty { get; set; }

    /// <summary>已发数量</summary>
    public int IssuedQty { get; set; }

    /// <summary>批次ID</summary>
    public string BatchId { get; set; } = string.Empty;

    /// <summary>是否跳过FIFO</summary>
    public bool FifoSkipped { get; set; }
}

/// <summary>
/// 退料请求
/// </summary>
public class ReturnMaterialRequest
{
    /// <summary>工单ID</summary>
    public string WorkOrderId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string? LotId { get; set; }

    /// <summary>退料项目列表</summary>
    public List<ReturnItemRequest> Items { get; set; } = [];

    /// <summary>退料人</summary>
    public string ReturnedBy { get; set; } = string.Empty;

    /// <summary>退料原因</summary>
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 退料项目请求
/// </summary>
public class ReturnItemRequest
{
    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>原始批次ID</summary>
    public string OriginalBatchId { get; set; } = string.Empty;

    /// <summary>退料数量</summary>
    public int ReturnQty { get; set; }

    /// <summary>目标库位ID</summary>
    public string? TargetLocationId { get; set; }
}

/// <summary>
/// 齐套检查响应
/// </summary>
public class KitCheckResponse
{
    /// <summary>工单ID</summary>
    public string WorkOrderId { get; set; } = string.Empty;

    /// <summary>是否齐套</summary>
    public bool IsComplete { get; set; }

    /// <summary>物料项目列表</summary>
    public List<KitItemStatus> Items { get; set; } = [];
}

/// <summary>
/// 齐套物料状态
/// </summary>
public class KitItemStatus
{
    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>物料名称</summary>
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>需求数量</summary>
    public int RequiredQty { get; set; }

    /// <summary>可用数量</summary>
    public int AvailableQty { get; set; }

    /// <summary>已发数量</summary>
    public int IssuedQty { get; set; }

    /// <summary>短缺数量</summary>
    public int ShortageQty { get; set; }

    /// <summary>是否充足</summary>
    public bool IsSufficient { get; set; }
}

/// <summary>
/// 领料单查询
/// </summary>
public class IssueOrderQuery : PagedQuery
{
    /// <summary>工单ID</summary>
    public string? WorkOrderId { get; set; }

    /// <summary>状态</summary>
    public string? Status { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}

/// <summary>
/// 退料单查询
/// </summary>
public class ReturnOrderQuery : PagedQuery
{
    /// <summary>工单ID</summary>
    public string? WorkOrderId { get; set; }

    /// <summary>状态</summary>
    public string? Status { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}

/// <summary>
/// 退料单响应
/// </summary>
public class ReturnOrderResponse
{
    /// <summary>退料单ID</summary>
    public string ReturnOrderId { get; set; } = string.Empty;

    /// <summary>工单ID</summary>
    public string WorkOrderId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string? LotId { get; set; }

    /// <summary>退料项目列表</summary>
    public List<ReturnItemResponse> Items { get; set; } = [];

    /// <summary>状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>退料原因</summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>退料时间</summary>
    public DateTime ReturnedAt { get; set; }
}

/// <summary>
/// 退料项目响应
/// </summary>
public class ReturnItemResponse
{
    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>物料名称</summary>
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>原始批次ID</summary>
    public string OriginalBatchId { get; set; } = string.Empty;

    /// <summary>退料数量</summary>
    public int ReturnQty { get; set; }

    /// <summary>目标库位ID</summary>
    public string? TargetLocationId { get; set; }
}
