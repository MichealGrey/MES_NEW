using MES.Adapters.Abstractions;
using MES.Adapters.Abstractions.Eap;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MES.Adapters.Mock;

/// <summary>
/// Mock EAP 适配器 - 模拟设备自动化程序对接
/// </summary>
public class MockEapAdapter : IMesEapAdapter
{
    private readonly MockConfig _config;
    private readonly ILogger<MockEapAdapter> _logger;
    private readonly Random _random = new();

    public MockEapAdapter(IOptions<MockConfig> config, ILogger<MockEapAdapter> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    private async Task SimulateDelayAsync() => await Task.Delay(_config.DelayMs);

    public async Task<AdapterResult<ParameterDownloadResult>> DownloadParametersAsync(ParameterSetData parameterSet)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock EAP: DownloadParametersAsync failed due to configured failure rate");
            return AdapterResult<ParameterDownloadResult>.Fail("MOCK_EAP_FAILURE", "Simulated EAP parameter download failure", "MockEAP");
        }

        _logger.LogInformation("Mock EAP: Downloading {Count} parameters to equipment {EquipmentId}",
            parameterSet.Parameters.Count, parameterSet.EquipmentId);

        var result = new ParameterDownloadResult
        {
            EquipmentId = parameterSet.EquipmentId,
            RecipeId = parameterSet.RecipeId,
            Downloaded = true,
            DownloadTime = DateTime.UtcNow.ToString("O")
        };

        return AdapterResult<ParameterDownloadResult>.Ok(result, "MockEAP");
    }

    public async Task<AdapterResult<EquipmentStatusData>> GetEquipmentStatusAsync(string equipmentId)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock EAP: GetEquipmentStatusAsync failed due to configured failure rate");
            return AdapterResult<EquipmentStatusData>.Fail("MOCK_EAP_FAILURE", "Simulated EAP equipment status failure", "MockEAP");
        }

        var statuses = new[] { "Idle", "Running", "Down", "Maintenance" };
        var status = statuses[_random.Next(statuses.Length)];

        _logger.LogDebug("Mock EAP: Equipment {EquipmentId} status: {Status}", equipmentId, status);

        var result = new EquipmentStatusData
        {
            EquipmentId = equipmentId,
            EquipmentName = $"Equipment-{equipmentId}",
            Status = status,
            CurrentRecipe = status == "Running" ? $"RECIPE-{_random.Next(100)}" : null,
            CurrentLot = status == "Running" ? $"LOT-{_random.Next(10000)}" : null,
            CurrentStep = status == "Running" ? _random.Next(1, 20) : null,
            Uptime = status == "Running" ? _random.NextDouble() * 8.0 : 0,
            LastHeartbeat = DateTime.UtcNow,
            ActiveAlarms = status == "Down"
                ? new List<EquipmentAlarm>
                {
                    new()
                    {
                        AlarmId = "ALM-001",
                        AlarmCode = "E-ERR-001",
                        AlarmMessage = "Equipment malfunction detected",
                        Severity = "Error",
                        AlarmTime = DateTime.UtcNow.AddMinutes(-5)
                    }
                }
                : new List<EquipmentAlarm>()
        };

        return AdapterResult<EquipmentStatusData>.Ok(result, "MockEAP");
    }

    public async Task<AdapterResult<List<EquipmentStatusData>>> GetBatchEquipmentStatusAsync(List<string> equipmentIds)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock EAP: GetBatchEquipmentStatusAsync failed due to configured failure rate");
            return AdapterResult<List<EquipmentStatusData>>.Fail("MOCK_EAP_FAILURE", "Simulated EAP batch status failure", "MockEAP");
        }

        _logger.LogDebug("Mock EAP: Getting batch status for {Count} equipment", equipmentIds.Count);

        var results = new List<EquipmentStatusData>();
        var statuses = new[] { "Idle", "Running", "Down", "Maintenance" };

        foreach (var eqId in equipmentIds)
        {
            var status = statuses[_random.Next(statuses.Length)];
            results.Add(new EquipmentStatusData
            {
                EquipmentId = eqId,
                EquipmentName = $"Equipment-{eqId}",
                Status = status,
                CurrentRecipe = status == "Running" ? $"RECIPE-{_random.Next(100)}" : null,
                CurrentLot = status == "Running" ? $"LOT-{_random.Next(10000)}" : null,
                CurrentStep = status == "Running" ? _random.Next(1, 20) : null,
                Uptime = status == "Running" ? _random.NextDouble() * 8.0 : 0,
                LastHeartbeat = DateTime.UtcNow,
                ActiveAlarms = new List<EquipmentAlarm>()
            });
        }

        return AdapterResult<List<EquipmentStatusData>>.Ok(results, "MockEAP");
    }

    public async Task<AdapterResult<DataCollectionResult>> CollectProcessDataAsync(string equipmentId)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock EAP: CollectProcessDataAsync failed due to configured failure rate");
            return AdapterResult<DataCollectionResult>.Fail("MOCK_EAP_FAILURE", "Simulated EAP data collection failure", "MockEAP");
        }

        var collectedCount = _random.Next(5, 21); // 5-20

        _logger.LogInformation("Mock EAP: Collected {Count} data points from equipment {EquipmentId}", collectedCount, equipmentId);

        var result = new DataCollectionResult
        {
            EquipmentId = equipmentId,
            CollectedCount = collectedCount,
            CollectionTime = DateTime.UtcNow
        };

        return AdapterResult<DataCollectionResult>.Ok(result, "MockEAP");
    }

    public async Task<AdapterResult<AlarmPushResult>> PushAlarmAsync(AlarmData alarmData)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock EAP: PushAlarmAsync failed due to configured failure rate");
            return AdapterResult<AlarmPushResult>.Fail("MOCK_EAP_FAILURE", "Simulated EAP alarm push failure", "MockEAP");
        }

        var alarmRecordId = $"ALM-{_random.Next(100000, 999999)}";

        _logger.LogInformation("Mock EAP: Pushing alarm {AlarmCode} for equipment {EquipmentId}, record id: {RecordId}",
            alarmData.AlarmCode, alarmData.EquipmentId, alarmRecordId);

        var result = new AlarmPushResult
        {
            Pushed = true,
            AlarmRecordId = alarmRecordId
        };

        return AdapterResult<AlarmPushResult>.Ok(result, "MockEAP");
    }

    public async Task<AdapterResult<CommandResult>> SendCommandAsync(EquipmentCommand command)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock EAP: SendCommandAsync failed due to configured failure rate");
            return AdapterResult<CommandResult>.Fail("MOCK_EAP_FAILURE", "Simulated EAP command send failure", "MockEAP");
        }

        _logger.LogInformation("Mock EAP: Sending command {CommandType} to equipment {EquipmentId}",
            command.CommandType, command.EquipmentId);

        var result = new CommandResult
        {
            Success = true,
            Message = "Command executed",
            ExecutionTime = DateTime.UtcNow
        };

        return AdapterResult<CommandResult>.Ok(result, "MockEAP");
    }

    public async Task<AdapterResult<HealthStatus>> HealthCheckAsync()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await SimulateDelayAsync();
        sw.Stop();

        _logger.LogDebug("Mock EAP: Health check completed in {ElapsedMs}ms", sw.ElapsedMilliseconds);

        var result = new HealthStatus
        {
            IsHealthy = true,
            Message = "Mock EAP adapter is healthy",
            ResponseTimeMs = _config.DelayMs
        };

        return AdapterResult<HealthStatus>.Ok(result, "MockEAP");
    }

    public AdapterInfo GetAdapterInfo()
    {
        return new AdapterInfo { Name = "MockEAP", Version = "1.0.0", Provider = "Mock" };
    }
}
