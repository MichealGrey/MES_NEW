using MES.Adapters.Abstractions;
using MES.Adapters.Abstractions.CustomerPortal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MES.Adapters.Mock;

/// <summary>
/// Mock 客户门户适配器 - 模拟客户门户对接
/// </summary>
public class MockCustomerPortalAdapter : IMesCustomerPortalAdapter
{
    private readonly MockConfig _config;
    private readonly ILogger<MockCustomerPortalAdapter> _logger;
    private readonly Random _random = new();

    public MockCustomerPortalAdapter(IOptions<MockConfig> config, ILogger<MockCustomerPortalAdapter> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    private async Task SimulateDelayAsync() => await Task.Delay(_config.DelayMs);

    public async Task<AdapterResult<PortalOrderResult>> ReceivePortalOrderAsync(PortalOrder order)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock CustomerPortal: ReceivePortalOrderAsync failed due to configured failure rate");
            return AdapterResult<PortalOrderResult>.Fail("MOCK_PORTAL_FAILURE", "Simulated portal order receive failure", "MockCustomerPortal");
        }

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var mesOrderNo = $"MES-{timestamp}";

        _logger.LogInformation("Mock CustomerPortal: Received portal order {OrderNo} from customer {CustomerId}, MES order no: {MesOrderNo}",
            order.OrderNo, order.CustomerId, mesOrderNo);

        var result = new PortalOrderResult
        {
            OrderNo = order.OrderNo,
            MesOrderNo = mesOrderNo,
            Status = "Accepted",
            Message = "Order received and accepted"
        };

        return AdapterResult<PortalOrderResult>.Ok(result, "MockCustomerPortal");
    }

    public async Task<AdapterResult<OrderProgressData>> GetOrderProgressAsync(string orderId)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock CustomerPortal: GetOrderProgressAsync failed due to configured failure rate");
            return AdapterResult<OrderProgressData>.Fail("MOCK_PORTAL_FAILURE", "Simulated portal order progress failure", "MockCustomerPortal");
        }

        var totalQty = _random.Next(100, 1000);
        var completedQty = _random.Next(0, totalQty);
        var inProgressQty = totalQty - completedQty;
        var progressPercentage = totalQty > 0 ? (decimal)completedQty / totalQty * 100 : 0;

        var processSteps = new List<ProcessProgress>
        {
            new()
            {
                ProcessCode = "SMT",
                ProcessName = "SMT贴片",
                Status = completedQty > 0 ? "Completed" : "NotStarted",
                CompletedQty = completedQty,
                StartDate = DateTime.UtcNow.AddDays(-3),
                CompletionDate = completedQty > 0 ? DateTime.UtcNow.AddDays(-1) : null
            },
            new()
            {
                ProcessCode = "DIP",
                ProcessName = "DIP插件",
                Status = completedQty > totalQty * 0.3 ? "InProgress" : (completedQty > 0 ? "Completed" : "NotStarted"),
                CompletedQty = completedQty > totalQty * 0.3 ? (int)(completedQty * 0.5) : completedQty,
                StartDate = DateTime.UtcNow.AddDays(-2),
                CompletionDate = completedQty > totalQty * 0.8 ? DateTime.UtcNow : null
            },
            new()
            {
                ProcessCode = "TEST",
                ProcessName = "功能测试",
                Status = completedQty > totalQty * 0.6 ? "InProgress" : "NotStarted",
                CompletedQty = completedQty > totalQty * 0.6 ? (int)(completedQty * 0.3) : 0,
                StartDate = completedQty > totalQty * 0.6 ? DateTime.UtcNow.AddDays(-1) : null,
                CompletionDate = null
            },
            new()
            {
                ProcessCode = "PACK",
                ProcessName = "包装入库",
                Status = "NotStarted",
                CompletedQty = 0,
                StartDate = null,
                CompletionDate = null
            }
        };

        _logger.LogDebug("Mock CustomerPortal: Order {OrderId} progress: {Percentage}%", orderId, Math.Round(progressPercentage, 2));

        var result = new OrderProgressData
        {
            OrderNo = orderId,
            Status = completedQty >= totalQty ? "Completed" : (completedQty > 0 ? "InProgress" : "Pending"),
            TotalQty = totalQty,
            CompletedQty = completedQty,
            InProgressQty = inProgressQty,
            ProgressPercentage = Math.Round(progressPercentage, 2),
            EstimatedCompletionDate = DateTime.UtcNow.AddDays(_random.Next(1, 5)),
            ProcessSteps = processSteps
        };

        return AdapterResult<OrderProgressData>.Ok(result, "MockCustomerPortal");
    }

    public async Task<AdapterResult<QualityReport>> GetQualityReportAsync(ReportQuery query)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock CustomerPortal: GetQualityReportAsync failed due to configured failure rate");
            return AdapterResult<QualityReport>.Fail("MOCK_PORTAL_FAILURE", "Simulated portal quality report failure", "MockCustomerPortal");
        }

        var reportId = $"QR-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var reportType = query.ReportType ?? "COA";

        _logger.LogDebug("Mock CustomerPortal: Generating quality report {ReportId} for order {OrderNo}", reportId, query.OrderNo);

        var result = new QualityReport
        {
            ReportId = reportId,
            ReportType = reportType,
            OrderNo = query.OrderNo,
            LotId = query.LotId,
            ReportUrl = $"https://mock/quality-report/{reportId}",
            GeneratedAt = DateTime.UtcNow
        };

        return AdapterResult<QualityReport>.Ok(result, "MockCustomerPortal");
    }

    public async Task<AdapterResult<ComplaintResult>> ReceiveComplaintAsync(ComplaintData complaint)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock CustomerPortal: ReceiveComplaintAsync failed due to configured failure rate");
            return AdapterResult<ComplaintResult>.Fail("MOCK_PORTAL_FAILURE", "Simulated portal complaint receive failure", "MockCustomerPortal");
        }

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var mesComplaintId = $"COMP-{timestamp}";

        _logger.LogInformation("Mock CustomerPortal: Received complaint {ComplaintNo} from customer {CustomerId}, type: {Type}, MES complaint id: {MesComplaintId}",
            complaint.ComplaintNo, complaint.CustomerId, complaint.ComplaintType, mesComplaintId);

        var result = new ComplaintResult
        {
            ComplaintNo = complaint.ComplaintNo,
            MesComplaintId = mesComplaintId,
            Accepted = true
        };

        return AdapterResult<ComplaintResult>.Ok(result, "MockCustomerPortal");
    }

    public async Task<AdapterResult<ShippingDocument>> GetShippingDocumentAsync(DocumentQuery query)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock CustomerPortal: GetShippingDocumentAsync failed due to configured failure rate");
            return AdapterResult<ShippingDocument>.Fail("MOCK_PORTAL_FAILURE", "Simulated portal shipping document failure", "MockCustomerPortal");
        }

        var documentId = $"DOC-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var docType = query.DocumentType ?? "PackingList";

        _logger.LogDebug("Mock CustomerPortal: Generating shipping document {DocumentId} for order {OrderNo}, type: {DocType}",
            documentId, query.OrderNo, docType);

        var result = new ShippingDocument
        {
            DocumentId = documentId,
            DocumentType = docType,
            OrderNo = query.OrderNo,
            DocumentUrl = $"https://mock/doc/{documentId}",
            GeneratedAt = DateTime.UtcNow
        };

        return AdapterResult<ShippingDocument>.Ok(result, "MockCustomerPortal");
    }

    public async Task<AdapterResult<ProductCatalog>> GetProductCatalogAsync()
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock CustomerPortal: GetProductCatalogAsync failed due to configured failure rate");
            return AdapterResult<ProductCatalog>.Fail("MOCK_PORTAL_FAILURE", "Simulated portal product catalog failure", "MockCustomerPortal");
        }

        var productCount = _random.Next(3, 6); // 3-5 products
        var products = new List<ProductItem>();

        var productData = new[]
        {
            ("PRD-001", "智能控制器 V3.0", "基于ARM Cortex-M4的智能控制器，支持多种通信协议", "120x80x25mm", "Active"),
            ("PRD-002", "工业传感器模组", "高精度温度/湿度/压力三合一传感器模组", "60x40x15mm", "Active"),
            ("PRD-003", "电源管理模块", "高效DC-DC转换电源管理模块，输入范围9-36V", "90x60x20mm", "Active"),
            ("PRD-004", "通信网关设备", "支持Modbus/CAN/Ethernet多协议工业网关", "150x100x35mm", "Active"),
            ("PRD-005", "LED驱动板 V2.1", "恒流LED驱动板，支持PWM调光", "100x50x10mm", "Discontinued")
        };

        for (var i = 0; i < productCount; i++)
        {
            var pd = productData[i % productData.Length];
            products.Add(new ProductItem
            {
                ProductId = pd.Item1,
                ProductName = pd.Item2,
                Description = pd.Item3,
                Specification = pd.Item4,
                Status = pd.Item5
            });
        }

        _logger.LogDebug("Mock CustomerPortal: Retrieved product catalog with {Count} products", products.Count);

        var result = new ProductCatalog
        {
            Products = products,
            LastUpdated = DateTime.UtcNow
        };

        return AdapterResult<ProductCatalog>.Ok(result, "MockCustomerPortal");
    }

    public async Task<AdapterResult<HealthStatus>> HealthCheckAsync()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await SimulateDelayAsync();
        sw.Stop();

        _logger.LogDebug("Mock CustomerPortal: Health check completed in {ElapsedMs}ms", sw.ElapsedMilliseconds);

        var result = new HealthStatus
        {
            IsHealthy = true,
            Message = "Mock Customer Portal adapter is healthy",
            ResponseTimeMs = _config.DelayMs
        };

        return AdapterResult<HealthStatus>.Ok(result, "MockCustomerPortal");
    }

    public AdapterInfo GetAdapterInfo()
    {
        return new AdapterInfo { Name = "MockCustomerPortal", Version = "1.0.0", Provider = "Mock" };
    }
}
