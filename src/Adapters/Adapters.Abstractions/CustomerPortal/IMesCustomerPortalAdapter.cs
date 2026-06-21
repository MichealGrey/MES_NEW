using MES.Adapters.Abstractions;

namespace MES.Adapters.Abstractions.CustomerPortal;

/// <summary>
/// 客户门户对接适配器
/// 对接场景：客户在线下单、订单进度查询、质量报告下载、客诉提交、出货文件获取
/// </summary>
public interface IMesCustomerPortalAdapter : IMesAdapter
{
    Task<AdapterResult<PortalOrderResult>> ReceivePortalOrderAsync(PortalOrder order);
    Task<AdapterResult<OrderProgressData>> GetOrderProgressAsync(string orderId);
    Task<AdapterResult<QualityReport>> GetQualityReportAsync(ReportQuery query);
    Task<AdapterResult<ComplaintResult>> ReceiveComplaintAsync(ComplaintData complaint);
    Task<AdapterResult<ShippingDocument>> GetShippingDocumentAsync(DocumentQuery query);
    Task<AdapterResult<ProductCatalog>> GetProductCatalogAsync();
}

public class PortalOrder
{
    public string OrderNo { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public List<PortalOrderItem> Items { get; set; } = new();
    public DateTime? RequiredDeliveryDate { get; set; }
    public string? Remark { get; set; }
    public string? Priority { get; set; }
}

public class PortalOrderItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class PortalOrderResult
{
    public string OrderNo { get; set; } = string.Empty;
    public string MesOrderNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Accepted, PendingReview, Rejected
    public string? Message { get; set; }
}

public class OrderProgressData
{
    public string OrderNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalQty { get; set; }
    public int CompletedQty { get; set; }
    public int InProgressQty { get; set; }
    public decimal ProgressPercentage { get; set; }
    public DateTime? EstimatedCompletionDate { get; set; }
    public List<ProcessProgress> ProcessSteps { get; set; } = new();
}

public class ProcessProgress
{
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // NotStarted, InProgress, Completed
    public int? CompletedQty { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletionDate { get; set; }
}

public class ReportQuery
{
    public string OrderNo { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string? ReportType { get; set; } // COA, TestReport, InspectionReport
}

public class QualityReport
{
    public string ReportId { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string OrderNo { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string ReportUrl { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class ComplaintData
{
    public string ComplaintNo { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string OrderNo { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string ComplaintType { get; set; } = string.Empty; // Quality, Delivery, Packaging
    public string Description { get; set; } = string.Empty;
    public List<string> AttachmentUrls { get; set; } = new();
    public DateTime ComplaintDate { get; set; }
}

public class ComplaintResult
{
    public string ComplaintNo { get; set; } = string.Empty;
    public string MesComplaintId { get; set; } = string.Empty;
    public bool Accepted { get; set; }
}

public class DocumentQuery
{
    public string OrderNo { get; set; } = string.Empty;
    public string? DocumentType { get; set; } // PackingList, COA, TestReport, DeliveryNote
}

public class ShippingDocument
{
    public string DocumentId { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string OrderNo { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class ProductCatalog
{
    public List<ProductItem> Products { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class ProductItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Specification { get; set; }
    public string Status { get; set; } = string.Empty; // Active, Discontinued
}
