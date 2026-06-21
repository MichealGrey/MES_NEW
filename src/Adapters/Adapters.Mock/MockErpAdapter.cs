using MES.Adapters.Abstractions;
using MES.Adapters.Abstractions.Erp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MES.Adapters.Mock;

/// <summary>
/// Mock ERP 适配器 - 模拟 ERP 系统对接
/// </summary>
public class MockErpAdapter : IMesErpAdapter
{
    private readonly MockConfig _config;
    private readonly ILogger<MockErpAdapter> _logger;
    private readonly Random _random = new();

    public MockErpAdapter(IOptions<MockConfig> config, ILogger<MockErpAdapter> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    private async Task SimulateDelayAsync() => await Task.Delay(_config.DelayMs);

    public async Task<AdapterResult<OrderSyncResult>> SyncSalesOrdersAsync(OrderSyncRequest request)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock ERP: SyncSalesOrdersAsync failed due to configured failure rate");
            return AdapterResult<OrderSyncResult>.Fail("MOCK_ERP_FAILURE", "Simulated ERP order sync failure", "MockERP");
        }

        _logger.LogInformation("Mock ERP: Syncing {Count} sales orders", request.Orders.Count);

        var result = new OrderSyncResult
        {
            SyncedCount = request.Orders.Count,
            FailedCount = 0,
            FailedOrderNos = new List<string>()
        };

        return AdapterResult<OrderSyncResult>.Ok(result, "MockERP");
    }

    public async Task<AdapterResult<BomSyncResult>> SyncBomAsync(BomSyncRequest request)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock ERP: SyncBomAsync failed due to configured failure rate");
            return AdapterResult<BomSyncResult>.Fail("MOCK_ERP_FAILURE", "Simulated ERP BOM sync failure", "MockERP");
        }

        _logger.LogInformation("Mock ERP: Syncing BOM for product {ProductId} with {Count} items", request.ProductId, request.Items.Count);

        var result = new BomSyncResult
        {
            ProductId = request.ProductId,
            BomVersion = request.BomVersion,
            ItemsCount = request.Items.Count
        };

        return AdapterResult<BomSyncResult>.Ok(result, "MockERP");
    }

    public async Task<AdapterResult<MaterialSyncResult>> SyncMaterialsAsync(MaterialSyncRequest request)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock ERP: SyncMaterialsAsync failed due to configured failure rate");
            return AdapterResult<MaterialSyncResult>.Fail("MOCK_ERP_FAILURE", "Simulated ERP material sync failure", "MockERP");
        }

        _logger.LogInformation("Mock ERP: Syncing {Count} materials", request.Materials.Count);

        var result = new MaterialSyncResult
        {
            SyncedCount = request.Materials.Count,
            FailedCount = 0
        };

        return AdapterResult<MaterialSyncResult>.Ok(result, "MockERP");
    }

    public async Task<AdapterResult<CompletionSyncResult>> ReportWorkOrderCompletionAsync(CompletionData data)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock ERP: ReportWorkOrderCompletionAsync failed due to configured failure rate");
            return AdapterResult<CompletionSyncResult>.Fail("MOCK_ERP_FAILURE", "Simulated ERP completion report failure", "MockERP");
        }

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var confirmation = $"ERP-CONF-{timestamp}";

        _logger.LogInformation("Mock ERP: Reporting work order completion for {WorkOrderId}, confirmation: {Confirmation}",
            data.WorkOrderId, confirmation);

        var result = new CompletionSyncResult
        {
            WorkOrderId = data.WorkOrderId,
            ErpConfirmation = confirmation
        };

        return AdapterResult<CompletionSyncResult>.Ok(result, "MockERP");
    }

    public async Task<AdapterResult<MaterialConsumeResult>> ReportMaterialConsumeAsync(ConsumeData data)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock ERP: ReportMaterialConsumeAsync failed due to configured failure rate");
            return AdapterResult<MaterialConsumeResult>.Fail("MOCK_ERP_FAILURE", "Simulated ERP material consume report failure", "MockERP");
        }

        _logger.LogInformation("Mock ERP: Reporting {Count} material consumptions for work order {WorkOrderId}",
            data.Consumptions.Count, data.WorkOrderId);

        var result = new MaterialConsumeResult
        {
            WorkOrderId = data.WorkOrderId,
            RecordedCount = data.Consumptions.Count
        };

        return AdapterResult<MaterialConsumeResult>.Ok(result, "MockERP");
    }

    public async Task<AdapterResult<FinishedGoodsResult>> ReportFinishedGoodsReceiptAsync(ReceiptData data)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock ERP: ReportFinishedGoodsReceiptAsync failed due to configured failure rate");
            return AdapterResult<FinishedGoodsResult>.Fail("MOCK_ERP_FAILURE", "Simulated ERP finished goods receipt failure", "MockERP");
        }

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var receiptNo = $"FG-{timestamp}";

        _logger.LogInformation("Mock ERP: Reporting finished goods receipt, receipt no: {ReceiptNo}", receiptNo);

        var result = new FinishedGoodsResult
        {
            ReceiptNo = receiptNo,
            ErpConfirmation = $"ERP-RC-{timestamp}"
        };

        return AdapterResult<FinishedGoodsResult>.Ok(result, "MockERP");
    }

    public async Task<AdapterResult<ScrapReportResult>> ReportScrapAsync(ScrapData data)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock ERP: ReportScrapAsync failed due to configured failure rate");
            return AdapterResult<ScrapReportResult>.Fail("MOCK_ERP_FAILURE", "Simulated ERP scrap report failure", "MockERP");
        }

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var recordId = $"SCRAP-{timestamp}";

        _logger.LogInformation("Mock ERP: Reporting scrap for material {MaterialId}, record id: {RecordId}",
            data.MaterialId, recordId);

        var result = new ScrapReportResult
        {
            RecordId = recordId
        };

        return AdapterResult<ScrapReportResult>.Ok(result, "MockERP");
    }

    public async Task<AdapterResult<HealthStatus>> HealthCheckAsync()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await SimulateDelayAsync();
        sw.Stop();

        _logger.LogDebug("Mock ERP: Health check completed in {ElapsedMs}ms", sw.ElapsedMilliseconds);

        var result = new HealthStatus
        {
            IsHealthy = true,
            Message = "Mock ERP adapter is healthy",
            ResponseTimeMs = _config.DelayMs
        };

        return AdapterResult<HealthStatus>.Ok(result, "MockERP");
    }

    public AdapterInfo GetAdapterInfo()
    {
        return new AdapterInfo { Name = "MockERP", Version = "1.0.0", Provider = "Mock" };
    }
}
