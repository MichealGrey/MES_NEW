using MES.Adapters.Abstractions;
using MES.Adapters.Abstractions.Wms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MES.Adapters.Mock;

/// <summary>
/// Mock WMS 适配器 - 模拟仓储管理系统对接
/// </summary>
public class MockWmsAdapter : IMesWmsAdapter
{
    private readonly MockConfig _config;
    private readonly ILogger<MockWmsAdapter> _logger;
    private readonly Random _random = new();

    public MockWmsAdapter(IOptions<MockConfig> config, ILogger<MockWmsAdapter> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    private async Task SimulateDelayAsync() => await Task.Delay(_config.DelayMs);

    public async Task<AdapterResult<MaterialRequestResult>> SendMaterialRequestAsync(MaterialRequest request)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock WMS: SendMaterialRequestAsync failed due to configured failure rate");
            return AdapterResult<MaterialRequestResult>.Fail("MOCK_WMS_FAILURE", "Simulated WMS material request failure", "MockWMS");
        }

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var wmsRequestNo = $"WMS-REQ-{timestamp}";

        _logger.LogInformation("Mock WMS: Sending material request {RequestId} for work order {WorkOrderId}, WMS no: {WmsRequestNo}",
            request.RequestId, request.WorkOrderId, wmsRequestNo);

        var result = new MaterialRequestResult
        {
            RequestId = request.RequestId,
            WmsRequestNo = wmsRequestNo,
            Status = "Submitted"
        };

        return AdapterResult<MaterialRequestResult>.Ok(result, "MockWMS");
    }

    public async Task<AdapterResult<IssueResult>> SendIssueInstructionAsync(IssueInstruction instruction)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock WMS: SendIssueInstructionAsync failed due to configured failure rate");
            return AdapterResult<IssueResult>.Fail("MOCK_WMS_FAILURE", "Simulated WMS issue instruction failure", "MockWMS");
        }

        _logger.LogInformation("Mock WMS: Sending issue instruction {IssueNo} for work order {WorkOrderId}",
            instruction.IssueNo, instruction.WorkOrderId);

        var result = new IssueResult
        {
            IssueNo = instruction.IssueNo,
            Confirmed = true,
            WmsConfirmation = $"WMS-CNF-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"
        };

        return AdapterResult<IssueResult>.Ok(result, "MockWMS");
    }

    public async Task<AdapterResult<ReturnResult>> SendReturnInstructionAsync(ReturnInstruction instruction)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock WMS: SendReturnInstructionAsync failed due to configured failure rate");
            return AdapterResult<ReturnResult>.Fail("MOCK_WMS_FAILURE", "Simulated WMS return instruction failure", "MockWMS");
        }

        _logger.LogInformation("Mock WMS: Sending return instruction {ReturnNo} for work order {WorkOrderId}",
            instruction.ReturnNo, instruction.WorkOrderId);

        var result = new ReturnResult
        {
            ReturnNo = instruction.ReturnNo,
            Confirmed = true
        };

        return AdapterResult<ReturnResult>.Ok(result, "MockWMS");
    }

    public async Task<AdapterResult<InventoryData>> GetInventoryAsync(InventoryQuery query)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock WMS: GetInventoryAsync failed due to configured failure rate");
            return AdapterResult<InventoryData>.Fail("MOCK_WMS_FAILURE", "Simulated WMS inventory query failure", "MockWMS");
        }

        var recordCount = _random.Next(3, 6); // 3-5 items
        var records = new List<InventoryRecord>();

        var materials = new[]
        {
            ("MAT-001", "C-RES-10K", "10K Resistor"),
            ("MAT-002", "C-CAP-100N", "100nF Capacitor"),
            ("MAT-003", "C-IC-STM32", "STM32 MCU"),
            ("MAT-004", "C-PCB-FR4", "FR4 PCB Board"),
            ("MAT-005", "C-LED-R", "Red LED")
        };

        var warehouses = new[] { "WH-A01", "WH-B02", "WH-C03" };
        var qualityStatuses = new[] { "Qualified", "Pending", "Qualified", "Qualified", "Quarantined" };

        for (var i = 0; i < recordCount; i++)
        {
            var mat = materials[i % materials.Length];
            records.Add(new InventoryRecord
            {
                MaterialId = mat.Item1,
                MaterialCode = mat.Item2,
                MaterialName = mat.Item3,
                BatchNo = $"BATCH-{DateTime.UtcNow:yyyyMMdd}-{_random.Next(100, 999)}",
                AvailableQty = (decimal)(_random.NextDouble() * 10000),
                LockedQty = (decimal)(_random.NextDouble() * 500),
                WarehouseCode = warehouses[_random.Next(warehouses.Length)],
                LocationCode = $"LOC-{_random.Next(1, 20):D2}-{_random.Next(1, 10):D2}",
                ExpiryDate = DateTime.UtcNow.AddDays(_random.Next(30, 365)),
                QualityStatus = qualityStatuses[_random.Next(qualityStatuses.Length)]
            });
        }

        _logger.LogDebug("Mock WMS: Retrieved {Count} inventory records", records.Count);

        var result = new InventoryData
        {
            Records = records,
            QueryTime = DateTime.UtcNow
        };

        return AdapterResult<InventoryData>.Ok(result, "MockWMS");
    }

    public async Task<AdapterResult<ReceiptConfirm>> ReceiveReceiptConfirmAsync(ReceiptData receiptData)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock WMS: ReceiveReceiptConfirmAsync failed due to configured failure rate");
            return AdapterResult<ReceiptConfirm>.Fail("MOCK_WMS_FAILURE", "Simulated WMS receipt confirm failure", "MockWMS");
        }

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var wmsReceiptNo = $"WMS-RC-{timestamp}";

        _logger.LogInformation("Mock WMS: Receiving receipt confirm {ReceiptNo}, WMS receipt no: {WmsReceiptNo}",
            receiptData.ReceiptNo, wmsReceiptNo);

        var result = new ReceiptConfirm
        {
            ReceiptNo = receiptData.ReceiptNo,
            Confirmed = true,
            WmsReceiptNo = wmsReceiptNo
        };

        return AdapterResult<ReceiptConfirm>.Ok(result, "MockWMS");
    }

    public async Task<AdapterResult<TransferResult>> SendTransferInstructionAsync(TransferInstruction instruction)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock WMS: SendTransferInstructionAsync failed due to configured failure rate");
            return AdapterResult<TransferResult>.Fail("MOCK_WMS_FAILURE", "Simulated WMS transfer instruction failure", "MockWMS");
        }

        _logger.LogInformation("Mock WMS: Sending transfer instruction {TransferNo} from {From} to {To}",
            instruction.TransferNo, instruction.FromLocation, instruction.ToLocation);

        var result = new TransferResult
        {
            TransferNo = instruction.TransferNo,
            Confirmed = true
        };

        return AdapterResult<TransferResult>.Ok(result, "MockWMS");
    }

    public async Task<AdapterResult<HealthStatus>> HealthCheckAsync()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await SimulateDelayAsync();
        sw.Stop();

        _logger.LogDebug("Mock WMS: Health check completed in {ElapsedMs}ms", sw.ElapsedMilliseconds);

        var result = new HealthStatus
        {
            IsHealthy = true,
            Message = "Mock WMS adapter is healthy",
            ResponseTimeMs = _config.DelayMs
        };

        return AdapterResult<HealthStatus>.Ok(result, "MockWMS");
    }

    public AdapterInfo GetAdapterInfo()
    {
        return new AdapterInfo { Name = "MockWMS", Version = "1.0.0", Provider = "Mock" };
    }
}
