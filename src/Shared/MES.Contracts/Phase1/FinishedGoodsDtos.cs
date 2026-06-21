using MES.Contracts.Common;

namespace MES.Contracts.Phase1;

// ============================================================================
// 成品管理 DTO
// ============================================================================

/// <summary>
/// 成品入库请求
/// </summary>
public class FinishedGoodsReceiptRequest
{
    /// <summary>工单ID</summary>
    public string WorkOrderId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>产品名称</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>数量</summary>
    public int Quantity { get; set; }

    /// <summary>FQC检验记录ID</summary>
    public string FqcRecordId { get; set; } = string.Empty;

    /// <summary>库位ID</summary>
    public string LocationId { get; set; } = string.Empty;

    /// <summary>等级</summary>
    public string? Grade { get; set; }

    /// <summary>收货人</summary>
    public string ReceivedBy { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 成品库存响应
/// </summary>
public class FinishedGoodsInventoryResponse
{
    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>产品名称</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>物料批次ID</summary>
    public string? BatchId { get; set; }

    /// <summary>库位ID</summary>
    public string LocationId { get; set; } = string.Empty;

    /// <summary>等级</summary>
    public string? Grade { get; set; }

    /// <summary>可用数量</summary>
    public int AvailableQty { get; set; }

    /// <summary>已发货数量</summary>
    public int ShippedQty { get; set; }

    /// <summary>入库日期</summary>
    public DateTime ReceiptDate { get; set; }
}

/// <summary>
/// 成品发货请求
/// </summary>
public class ShipFinishedGoodsRequest
{
    /// <summary>出货单ID</summary>
    public string ShipmentId { get; set; } = string.Empty;

    /// <summary>发货项目列表</summary>
    public List<ShipmentItemRequest> Items { get; set; } = [];

    /// <summary>发货人</summary>
    public string ShippedBy { get; set; } = string.Empty;

    /// <summary>承运商</summary>
    public string? Carrier { get; set; }

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 发货项目请求
/// </summary>
public class ShipmentItemRequest
{
    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>数量</summary>
    public int Quantity { get; set; }
}

/// <summary>
/// 发货响应
/// </summary>
public class ShipmentResponse
{
    /// <summary>出货单ID</summary>
    public string ShipmentId { get; set; } = string.Empty;

    /// <summary>客户ID</summary>
    public string? CustomerId { get; set; }

    /// <summary>客户名称</summary>
    public string? CustomerName { get; set; }

    /// <summary>状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>发货人</summary>
    public string? ShippedBy { get; set; }

    /// <summary>发货时间</summary>
    public DateTime? ShippedAt { get; set; }

    /// <summary>运单号</summary>
    public string? TrackingNo { get; set; }
}

/// <summary>
/// 成品查询
/// </summary>
public class FinishedGoodsQuery : PagedQuery
{
    /// <summary>产品ID</summary>
    public string? ProductId { get; set; }

    /// <summary>批次ID</summary>
    public string? LotId { get; set; }

    /// <summary>状态</summary>
    public string? Status { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}
