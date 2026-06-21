using MES.Contracts.Common;

namespace MES.Contracts.Phase1;

// ============================================================================
// 仓库管理 DTO
// ============================================================================

/// <summary>
/// 仓库入库请求
/// </summary>
public class WarehouseReceiptRequest
{
    /// <summary>批次ID</summary>
    public string BatchId { get; set; } = string.Empty;

    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>物料名称</summary>
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>数量</summary>
    public int Quantity { get; set; }

    /// <summary>单位</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>供应商ID</summary>
    public string? SupplierId { get; set; }

    /// <summary>供应商批次号</summary>
    public string? SupplierBatchNo { get; set; }

    /// <summary>库位ID</summary>
    public string LocationId { get; set; } = string.Empty;

    /// <summary>库位名称</summary>
    public string LocationName { get; set; } = string.Empty;

    /// <summary>有效期</summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>保质期(天)</summary>
    public int? ShelfLifeDays { get; set; }

    /// <summary>MSL等级</summary>
    public string? MslLevel { get; set; }

    /// <summary>MSL地板寿命(小时)</summary>
    public decimal? MslFloorLifeHours { get; set; }

    /// <summary>IQC任务ID</summary>
    public string? IqcTaskId { get; set; }

    /// <summary>收货人</summary>
    public string ReceivedBy { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 仓库入库响应
/// </summary>
public class WarehouseReceiptResponse
{
    /// <summary>入库单ID</summary>
    public string ReceiptId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string BatchId { get; set; } = string.Empty;

    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>物料名称</summary>
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>数量</summary>
    public int Quantity { get; set; }

    /// <summary>库位ID</summary>
    public string LocationId { get; set; } = string.Empty;

    /// <summary>状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>入库时间</summary>
    public DateTime ReceivedAt { get; set; }
}

/// <summary>
/// 库存响应
/// </summary>
public class InventoryResponse
{
    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>物料名称</summary>
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string BatchId { get; set; } = string.Empty;

    /// <summary>库位ID</summary>
    public string LocationId { get; set; } = string.Empty;

    /// <summary>库位名称</summary>
    public string LocationName { get; set; } = string.Empty;

    /// <summary>可用数量</summary>
    public int AvailableQty { get; set; }

    /// <summary>锁定数量</summary>
    public int LockedQty { get; set; }

    /// <summary>已分配数量</summary>
    public int AllocatedQty { get; set; }

    /// <summary>单位</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>有效期</summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>剩余天数</summary>
    public int? RemainingDays { get; set; }

    /// <summary>状态: Available/Locked/Expiring/Expired</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>MSL等级</summary>
    public string? MslLevel { get; set; }

    /// <summary>入库时间</summary>
    public DateTime ReceivedAt { get; set; }

    /// <summary>是否FIFO eligible</summary>
    public bool IsFifoEligible { get; set; }
}

/// <summary>
/// FIFO推荐响应
/// </summary>
public class FifoRecommendResponse
{
    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>请求数量</summary>
    public int RequestedQty { get; set; }

    /// <summary>推荐批次列表</summary>
    public List<FifoBatchRecommendation> Recommendations { get; set; } = [];
}

/// <summary>
/// FIFO批次推荐
/// </summary>
public class FifoBatchRecommendation
{
    /// <summary>批次ID</summary>
    public string BatchId { get; set; } = string.Empty;

    /// <summary>库位ID</summary>
    public string LocationId { get; set; } = string.Empty;

    /// <summary>可用数量</summary>
    public int AvailableQty { get; set; }

    /// <summary>推荐数量</summary>
    public int RecommendQty { get; set; }

    /// <summary>入库时间</summary>
    public DateTime ReceivedAt { get; set; }

    /// <summary>有效期</summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>供应商批次号</summary>
    public string SupplierBatchNo { get; set; } = string.Empty;
}

/// <summary>
/// 有效期预警响应
/// </summary>
public class ExpiryWarningResponse
{
    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>物料名称</summary>
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string BatchId { get; set; } = string.Empty;

    /// <summary>有效期</summary>
    public DateTime ExpiryDate { get; set; }

    /// <summary>剩余天数</summary>
    public int RemainingDays { get; set; }

    /// <summary>数量</summary>
    public int Quantity { get; set; }

    /// <summary>库位ID</summary>
    public string LocationId { get; set; } = string.Empty;

    /// <summary>预警级别: Warning/Urgent/Expired</summary>
    public string WarningLevel { get; set; } = string.Empty;
}

/// <summary>
/// 库存调整请求
/// </summary>
public class InventoryAdjustRequest
{
    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string BatchId { get; set; } = string.Empty;

    /// <summary>调整数量(正/负)</summary>
    public int AdjustQty { get; set; }

    /// <summary>调整原因</summary>
    public string AdjustReason { get; set; } = string.Empty;

    /// <summary>操作员工号</summary>
    public string OperatorId { get; set; } = string.Empty;
}

/// <summary>
/// 库存查询
/// </summary>
public class InventoryQuery : PagedQuery
{
    /// <summary>物料ID</summary>
    public string? MaterialId { get; set; }

    /// <summary>库位ID</summary>
    public string? LocationId { get; set; }

    /// <summary>状态: Available/Locked/Expiring/Expired</summary>
    public string? Status { get; set; }

    /// <summary>是否即将过期</summary>
    public bool? IsExpiring { get; set; }
}

/// <summary>
/// 批次锁定请求
/// </summary>
public class BatchLockRequest
{
    /// <summary>批次ID</summary>
    public string BatchId { get; set; } = string.Empty;

    /// <summary>锁定原因</summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>操作员工号</summary>
    public string OperatorId { get; set; } = string.Empty;
}
