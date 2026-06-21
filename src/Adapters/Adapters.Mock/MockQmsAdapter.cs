using MES.Adapters.Abstractions;
using MES.Adapters.Abstractions.Qms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MES.Adapters.Mock;

/// <summary>
/// Mock QMS 适配器 - 模拟质量管理系统对接
/// </summary>
public class MockQmsAdapter : IMesQmsAdapter
{
    private readonly MockConfig _config;
    private readonly ILogger<MockQmsAdapter> _logger;
    private readonly Random _random = new();

    public MockQmsAdapter(IOptions<MockConfig> config, ILogger<MockQmsAdapter> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    private async Task SimulateDelayAsync() => await Task.Delay(_config.DelayMs);

    public async Task<AdapterResult<InspectionPushResult>> PushInspectionDataAsync(InspectionData data)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock QMS: PushInspectionDataAsync failed due to configured failure rate");
            return AdapterResult<InspectionPushResult>.Fail("MOCK_QMS_FAILURE", "Simulated QMS inspection data push failure", "MockQMS");
        }

        _logger.LogInformation("Mock QMS: Pushing inspection data {InspectionId} ({Type}) with {Count} items",
            data.InspectionId, data.InspectionType, data.Items.Count);

        var result = new InspectionPushResult
        {
            InspectionId = data.InspectionId,
            Pushed = true
        };

        return AdapterResult<InspectionPushResult>.Ok(result, "MockQMS");
    }

    public async Task<AdapterResult<NonconformingResult>> PushNonconformingInfoAsync(NonconformingData data)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock QMS: PushNonconformingInfoAsync failed due to configured failure rate");
            return AdapterResult<NonconformingResult>.Fail("MOCK_QMS_FAILURE", "Simulated QMS nonconforming push failure", "MockQMS");
        }

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var qmsRecordNo = $"QMS-NC-{timestamp}";

        _logger.LogInformation("Mock QMS: Pushing nonconforming info {RecordId}, QMS record no: {QmsRecordNo}",
            data.RecordId, qmsRecordNo);

        var result = new NonconformingResult
        {
            RecordId = data.RecordId,
            QmsRecordNo = qmsRecordNo
        };

        return AdapterResult<NonconformingResult>.Ok(result, "MockQMS");
    }

    public async Task<AdapterResult<SpcPushResult>> PushSpcDataAsync(SpcData data)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock QMS: PushSpcDataAsync failed due to configured failure rate");
            return AdapterResult<SpcPushResult>.Fail("MOCK_QMS_FAILURE", "Simulated QMS SPC data push failure", "MockQMS");
        }

        _logger.LogInformation("Mock QMS: Pushing {Count} SPC measurements for process {ProcessCode}",
            data.Measurements.Count, data.ProcessCode);

        var result = new SpcPushResult
        {
            PushedCount = data.Measurements.Count
        };

        return AdapterResult<SpcPushResult>.Ok(result, "MockQMS");
    }

    public async Task<AdapterResult<InspectionStandard>> PullInspectionStandardAsync(StandardQuery query)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock QMS: PullInspectionStandardAsync failed due to configured failure rate");
            return AdapterResult<InspectionStandard>.Fail("MOCK_QMS_FAILURE", "Simulated QMS inspection standard pull failure", "MockQMS");
        }

        var itemCount = _random.Next(5, 9); // 5-8 items
        var items = new List<InspectionItemStandard>();

        var itemNames = new[]
        {
            ("DIM-001", "外形尺寸", "Dimensional Measurement", "10.00", "9.95", "10.05", "AQL-1.0"),
            ("ELE-001", "电阻值", "Resistance Test", "100.00", "95.00", "105.00", "AQL-0.65"),
            ("VIS-001", "外观检查", "Visual Inspection", "N/A", "N/A", "N/A", "AQL-2.5"),
            ("FUN-001", "功能测试", "Functional Test", "N/A", "N/A", "N/A", "AQL-1.0"),
            ("INS-001", "绝缘电阻", "Insulation Resistance", "100.00", "50.00", "N/A", "AQL-0.4"),
            ("TEMP-001", "温升测试", "Temperature Rise Test", "65.00", "N/A", "75.00", "AQL-1.0"),
            ("VIB-001", "振动测试", "Vibration Test", "N/A", "N/A", "N/A", "AQL-1.5"),
            ("HUM-001", "湿度测试", "Humidity Test", "N/A", "N/A", "N/A", "AQL-2.5")
        };

        for (var i = 0; i < itemCount; i++)
        {
            var item = itemNames[i % itemNames.Length];
            items.Add(new InspectionItemStandard
            {
                ItemCode = item.Item1,
                ItemName = item.Item2,
                TestMethod = item.Item3,
                TargetValue = item.Item4,
                LowerLimit = item.Item5,
                UpperLimit = item.Item6,
                SamplingPlan = item.Item7
            });
        }

        var targetId = query.MaterialId ?? query.ProductId ?? "UNKNOWN";

        _logger.LogDebug("Mock QMS: Pulled inspection standard with {Count} items for {TargetId}", items.Count, targetId);

        var result = new InspectionStandard
        {
            StandardId = $"STD-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
            MaterialId = targetId,
            InspectionType = query.InspectionType ?? "IPQC",
            Items = items,
            Version = $"V{_random.Next(1, 5)}.{_random.Next(0, 9)}",
            EffectiveDate = DateTime.UtcNow.AddDays(-_random.Next(1, 30))
        };

        return AdapterResult<InspectionStandard>.Ok(result, "MockQMS");
    }

    public async Task<AdapterResult<FmeaData>> PullFmeaDataAsync(FmeaQuery query)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock QMS: PullFmeaDataAsync failed due to configured failure rate");
            return AdapterResult<FmeaData>.Fail("MOCK_QMS_FAILURE", "Simulated QMS FMEA data pull failure", "MockQMS");
        }

        var itemCount = _random.Next(3, 6); // 3-5 items
        var items = new List<FmeaItem>();

        var failureModes = new[]
        {
            ("焊接不良", "电路短路", "焊接温度不足", 8, 5, 3, "焊接后AOI检测"),
            ("尺寸超差", "装配干涉", "模具磨损", 7, 4, 4, "首件检验+巡检"),
            ("表面划伤", "外观不合格", "搬运操作不当", 3, 6, 2, "包装防护+搬运培训"),
            ("电气参数漂移", "功能失效", "元器件老化", 9, 3, 5, "老化筛选+100%测试"),
            ("异物污染", "性能下降", "洁净室等级不达标", 6, 3, 3, "洁净室监控+定期清洁")
        };

        for (var i = 0; i < itemCount; i++)
        {
            var fm = failureModes[i % failureModes.Length];
            var s = fm.Item4;
            var o = fm.Item5;
            var d = fm.Item6;

            items.Add(new FmeaItem
            {
                FailureMode = fm.Item1,
                FailureEffect = fm.Item2,
                FailureCause = fm.Item3,
                Severity = s,
                Occurrence = o,
                Detection = d,
                Rpn = s * o * d,
                ControlMeasure = fm.Item7
            });
        }

        _logger.LogDebug("Mock QMS: Pulled FMEA data with {Count} failure modes for process {ProcessCode}",
            items.Count, query.ProcessCode ?? "ALL");

        var result = new FmeaData
        {
            FmeaId = $"FMEA-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
            ProcessCode = query.ProcessCode ?? "GENERAL",
            Items = items
        };

        return AdapterResult<FmeaData>.Ok(result, "MockQMS");
    }

    public async Task<AdapterResult<QualityAlert>> ReceiveQualityAlertAsync()
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock QMS: ReceiveQualityAlertAsync failed due to configured failure rate");
            return AdapterResult<QualityAlert>.Fail("MOCK_QMS_FAILURE", "Simulated QMS quality alert receive failure", "MockQMS");
        }

        var severities = new[] { "Critical", "Major", "Minor" };
        var severity = severities[_random.Next(severities.Length)];

        var alert = new QualityAlert
        {
            AlertId = $"QA-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
            AlertType = "QualityDeviation",
            Severity = severity,
            Title = $"质量偏差警报 - {severity} 级别",
            Description = "检测到产品质量偏差，请相关人员立即处理",
            AffectedProducts = new List<string> { $"PROD-{_random.Next(100)}", $"PROD-{_random.Next(100)}" },
            AffectedLots = new List<string> { $"LOT-{_random.Next(10000)}", $"LOT-{_random.Next(10000)}" },
            AlertTime = DateTime.UtcNow,
            IssuedBy = "QMS-System"
        };

        _logger.LogWarning("Mock QMS: Received quality alert {AlertId} - {Severity}", alert.AlertId, severity);

        return AdapterResult<QualityAlert>.Ok(alert, "MockQMS");
    }

    public async Task<AdapterResult<HealthStatus>> HealthCheckAsync()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await SimulateDelayAsync();
        sw.Stop();

        _logger.LogDebug("Mock QMS: Health check completed in {ElapsedMs}ms", sw.ElapsedMilliseconds);

        var result = new HealthStatus
        {
            IsHealthy = true,
            Message = "Mock QMS adapter is healthy",
            ResponseTimeMs = _config.DelayMs
        };

        return AdapterResult<HealthStatus>.Ok(result, "MockQMS");
    }

    public AdapterInfo GetAdapterInfo()
    {
        return new AdapterInfo { Name = "MockQMS", Version = "1.0.0", Provider = "Mock" };
    }
}
