using MES.Adapters.Abstractions;

namespace MES.Adapters.Abstractions.Wms;

/// <summary>
/// WMS 仓储管理系统对接适配器
/// 对接场景：MES→WMS：物料需求、发料指令、退料指令
///          WMS→MES：库存信息、出入库确认
/// </summary>
public interface IMesWmsAdapter : IMesAdapter
{
    Task<AdapterResult<MaterialRequestResult>> SendMaterialRequestAsync(MaterialRequest request);
    Task<AdapterResult<IssueResult>> SendIssueInstructionAsync(IssueInstruction instruction);
    Task<AdapterResult<ReturnResult>> SendReturnInstructionAsync(ReturnInstruction instruction);
    Task<AdapterResult<InventoryData>> GetInventoryAsync(InventoryQuery query);
    Task<AdapterResult<ReceiptConfirm>> ReceiveReceiptConfirmAsync(ReceiptData receiptData);
    Task<AdapterResult<TransferResult>> SendTransferInstructionAsync(TransferInstruction instruction);
}

public class MaterialRequest
{
    public string RequestId { get; set; } = string.Empty;
    public string WorkOrderId { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty; // Production, Rework, Sample
    public List<MaterialRequestItem> Items { get; set; } = new();
    public DateTime RequiredTime { get; set; }
    public string? RequestedBy { get; set; }
}

public class MaterialRequestItem
{
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialCode { get; set; } = string.Empty;
    public decimal RequestedQty { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? BatchRequirement { get; set; } // FIFO, FEFO, Specific
}

public class MaterialRequestResult
{
    public string RequestId { get; set; } = string.Empty;
    public string WmsRequestNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class IssueInstruction
{
    public string IssueNo { get; set; } = string.Empty;
    public string WorkOrderId { get; set; } = string.Empty;
    public string TargetLine { get; set; } = string.Empty;
    public List<IssueItem> Items { get; set; } = new();
    public string IssuedBy { get; set; } = string.Empty;
}

public class IssueItem
{
    public string MaterialId { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string? LocationCode { get; set; }
}

public class IssueResult
{
    public string IssueNo { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
    public string? WmsConfirmation { get; set; }
}

public class ReturnInstruction
{
    public string ReturnNo { get; set; } = string.Empty;
    public string WorkOrderId { get; set; } = string.Empty;
    public string ReturnReason { get; set; } = string.Empty;
    public List<ReturnItem> Items { get; set; } = new();
    public string ReturnedBy { get; set; } = string.Empty;
}

public class ReturnItem
{
    public string MaterialId { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string? QualityStatus { get; set; }
}

public class ReturnResult
{
    public string ReturnNo { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
}

public class InventoryQuery
{
    public string? MaterialId { get; set; }
    public string? WarehouseCode { get; set; }
    public string? LocationCode { get; set; }
    public bool? IncludeLocked { get; set; }
}

public class InventoryData
{
    public List<InventoryRecord> Records { get; set; } = new();
    public DateTime QueryTime { get; set; }
}

public class InventoryRecord
{
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public decimal AvailableQty { get; set; }
    public decimal LockedQty { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public string? QualityStatus { get; set; }
}

public class ReceiptData
{
    public string ReceiptNo { get; set; } = string.Empty;
    public string ReceiptType { get; set; } = string.Empty; // Purchase, Return, Production
    public List<ReceiptItem> Items { get; set; } = new();
    public string ReceivedBy { get; set; } = string.Empty;
    public DateTime ReceiptTime { get; set; }
}

public class ReceiptItem
{
    public string MaterialId { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string? LocationCode { get; set; }
}

public class ReceiptConfirm
{
    public string ReceiptNo { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
    public string? WmsReceiptNo { get; set; }
}

public class TransferInstruction
{
    public string TransferNo { get; set; } = string.Empty;
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public List<TransferItem> Items { get; set; } = new();
}

public class TransferItem
{
    public string MaterialId { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public decimal Qty { get; set; }
}

public class TransferResult
{
    public string TransferNo { get; set; } = string.Empty;
    public bool Confirmed { get; set; }
}
