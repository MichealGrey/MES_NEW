namespace MES.Infrastructure.Persistence.Entities;

/// <summary>
/// 原材料入库单
/// </summary>
public class WarehouseReceipt
{
    /// <summary>
    /// 入库单号
    /// </summary>
    public string ReceiptId { get; set; } = string.Empty;
    public string? PoNumber { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? MaterialSpec { get; set; }
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public string? LotNumber { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public string WarehouseId { get; set; } = string.Empty;
    public string? LocationId { get; set; }
    public string ReceiptType { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? IqcBatchId { get; set; }
    public string? IqcStatus { get; set; }
    public string? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 原材料库存
/// </summary>
public class WarehouseInventory
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    public string WarehouseId { get; set; } = string.Empty;
    public string? LocationId { get; set; }
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? MaterialSpec { get; set; }
    public string? LotNumber { get; set; }
    public int Quantity { get; set; }
    public int? ReservedQty { get; set; }
    public int AvailableQty { get; set; }
    public string? Unit { get; set; }
    public string? BatchId { get; set; }
    public string? SupplierId { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "Available";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 库位
/// </summary>
public class WarehouseLocation
{
    /// <summary>
    /// 库位ID
    /// </summary>
    public string LocationId { get; set; } = string.Empty;
    public string WarehouseId { get; set; } = string.Empty;
    public string? WarehouseName { get; set; }
    public string? Zone { get; set; }
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Level { get; set; }
    public string? Bin { get; set; }
    public string FullPath { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public int? MaxCapacity { get; set; }
    public int? CurrentCapacity { get; set; }
    public string Status { get; set; } = "Available";
    public string? Temperature { get; set; }
    public string? Humidity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 物料保质期管理
/// </summary>
public class MaterialShelfLife
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public int? ShelfLifeDays { get; set; }
    public int? OpenShelfLifeHours { get; set; }
    public string? StorageCondition { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? LotNumber { get; set; }
    public string? SupplierId { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 原材料出库单
/// </summary>
public class WarehouseIssueOrder
{
    /// <summary>
    /// 出库单号
    /// </summary>
    public string IssueOrderId { get; set; } = string.Empty;
    public string? WorkOrderId { get; set; }
    public string? LotId { get; set; }
    public string IssueType { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? Destination { get; set; }
    public string? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public DateTime? IssueTime { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 原材料出库明细
/// </summary>
public class WarehouseIssueItem
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    public string IssueOrderId { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? LotNumber { get; set; }
    public int RequestQty { get; set; }
    public int ActualQty { get; set; }
    public string? Unit { get; set; }
    public string? WarehouseId { get; set; }
    public string? LocationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 原材料退料单
/// </summary>
public class WarehouseReturnOrder
{
    /// <summary>
    /// 退料单号
    /// </summary>
    public string ReturnOrderId { get; set; } = string.Empty;
    public string? WorkOrderId { get; set; }
    public string? LotId { get; set; }
    public string ReturnType { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? Reason { get; set; }
    public string? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public DateTime? ReturnTime { get; set; }
    public string? WarehouseId { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 原材料退料明细
/// </summary>
public class WarehouseReturnItem
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    public string ReturnOrderId { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string? LotNumber { get; set; }
    public int ReturnQty { get; set; }
    public string? Unit { get; set; }
    public string? WarehouseId { get; set; }
    public string? LocationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 成品入库单
/// </summary>
public class FinishedGoodsReceipt
{
    /// <summary>
    /// 入库单号
    /// </summary>
    public string ReceiptId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public string WarehouseId { get; set; } = string.Empty;
    public string? LocationId { get; set; }
    public DateTime? ReceiptTime { get; set; }
    public string? FqcRecordId { get; set; }
    public string Status { get; set; } = "Pending";
    public string? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 成品库存
/// </summary>
public class FinishedGoodsInventory
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    public string WarehouseId { get; set; } = string.Empty;
    public string? LocationId { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int Quantity { get; set; }
    public int? ReservedQty { get; set; }
    public int AvailableQty { get; set; }
    public string? Unit { get; set; }
    public DateTime? ProducedDate { get; set; }
    public string Status { get; set; } = "Available";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 成品出库单
/// </summary>
public class FinishedGoodsShipment
{
    /// <summary>
    /// 出库单号
    /// </summary>
    public string ShipmentId { get; set; } = string.Empty;
    public string? SalesOrderId { get; set; }
    public string? DeliveryNoteId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ShipmentType { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime? ShipmentTime { get; set; }
    public string? Carrier { get; set; }
    public string? TrackingNumber { get; set; }
    public string? OperatorId { get; set; }
    public string? OperatorName { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 成品出库明细
/// </summary>
public class FinishedGoodsShipmentItem
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    public string ShipmentId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public string? WarehouseId { get; set; }
    public string? LocationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
