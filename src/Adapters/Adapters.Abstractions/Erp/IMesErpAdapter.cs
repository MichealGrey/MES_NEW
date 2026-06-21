using MES.Adapters.Abstractions;

namespace MES.Adapters.Abstractions.Erp;

// ==================== 接口定义 ====================

/// <summary>
/// ERP 系统对接适配器
/// 对接场景：ERP→MES：销售订单、BOM、物料主数据
///          MES→ERP：工单完工、物料消耗、成品入库、报废、工时
/// </summary>
public interface IMesErpAdapter : IMesAdapter
{
    // ERP → MES 同步
    Task<AdapterResult<OrderSyncResult>> SyncSalesOrdersAsync(OrderSyncRequest request);
    Task<AdapterResult<BomSyncResult>> SyncBomAsync(BomSyncRequest request);
    Task<AdapterResult<MaterialSyncResult>> SyncMaterialsAsync(MaterialSyncRequest request);

    // MES → ERP 回传
    Task<AdapterResult<CompletionSyncResult>> ReportWorkOrderCompletionAsync(CompletionData data);
    Task<AdapterResult<MaterialConsumeResult>> ReportMaterialConsumeAsync(ConsumeData data);
    Task<AdapterResult<FinishedGoodsResult>> ReportFinishedGoodsReceiptAsync(ReceiptData data);
    Task<AdapterResult<ScrapReportResult>> ReportScrapAsync(ScrapData data);
}

// ==================== ERP → MES DTOs ====================

public class OrderSyncRequest
{
    public string? SyncId { get; set; }
    public List<ErpOrder> Orders { get; set; } = new();
}

public class ErpOrder
{
    public string OrderNo { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime? DeliveryDate { get; set; }
    public DateTime OrderDate { get; set; }
    public string? Remark { get; set; }
    public string? Priority { get; set; }
}

public class OrderSyncResult
{
    public int SyncedCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> FailedOrderNos { get; set; } = new();
}

public class BomSyncRequest
{
    public string ProductId { get; set; } = string.Empty;
    public string BomVersion { get; set; } = string.Empty;
    public List<BomItem> Items { get; set; } = new();
}

public class BomItem
{
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int Level { get; set; }
    public string? ParentMaterialId { get; set; }
    public decimal? ScrapRate { get; set; }
}

public class BomSyncResult
{
    public string ProductId { get; set; } = string.Empty;
    public string BomVersion { get; set; } = string.Empty;
    public int ItemsCount { get; set; }
}

public class MaterialSyncRequest
{
    public List<ErpMaterial> Materials { get; set; } = new();
}

public class ErpMaterial
{
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty; // Raw, SemiFinished, Finished
    public string? Specification { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int? ShelfLife { get; set; } // days
    public string? MslLevel { get; set; }
    public string? Supplier { get; set; }
}

public class MaterialSyncResult
{
    public int SyncedCount { get; set; }
    public int FailedCount { get; set; }
}

// ==================== MES → ERP DTOs ====================

public class CompletionData
{
    public string WorkOrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public int CompletedQty { get; set; }
    public DateTime CompletionTime { get; set; }
    public string? OperatorId { get; set; }
    public string? EquipmentId { get; set; }
    public decimal? ActualHours { get; set; }
}

public class CompletionSyncResult
{
    public string WorkOrderId { get; set; } = string.Empty;
    public string ErpConfirmation { get; set; } = string.Empty;
}

public class ConsumeData
{
    public string WorkOrderId { get; set; } = string.Empty;
    public List<MaterialConsumption> Consumptions { get; set; } = new();
}

public class MaterialConsumption
{
    public string MaterialId { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public decimal ConsumedQty { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class MaterialConsumeResult
{
    public string WorkOrderId { get; set; } = string.Empty;
    public int RecordedCount { get; set; }
}

public class ReceiptData
{
    public string WorkOrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string? BatchNo { get; set; }
    public int Quantity { get; set; }
    public DateTime ReceiptTime { get; set; }
    public string? WarehouseCode { get; set; }
    public string? LocationCode { get; set; }
}

public class FinishedGoodsResult
{
    public string ReceiptNo { get; set; } = string.Empty;
    public string? ErpConfirmation { get; set; }
}

public class ScrapData
{
    public string WorkOrderId { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string MaterialId { get; set; } = string.Empty;
    public int ScrapQty { get; set; }
    public string ScrapReason { get; set; } = string.Empty;
    public DateTime ScrapTime { get; set; }
    public string? OperatorId { get; set; }
}

public class ScrapReportResult
{
    public string RecordId { get; set; } = string.Empty;
}
