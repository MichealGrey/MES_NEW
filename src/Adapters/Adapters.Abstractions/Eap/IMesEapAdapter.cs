using MES.Adapters.Abstractions;

namespace MES.Adapters.Abstractions.Eap;

/// <summary>
/// EAP (Equipment Automation Program) 设备自动化集成适配器
/// 对接场景：工艺参数下发、设备状态采集、生产数据上报、设备报警推送
/// </summary>
public interface IMesEapAdapter : IMesAdapter
{
    Task<AdapterResult<ParameterDownloadResult>> DownloadParametersAsync(ParameterSetData parameterSet);
    Task<AdapterResult<EquipmentStatusData>> GetEquipmentStatusAsync(string equipmentId);
    Task<AdapterResult<List<EquipmentStatusData>>> GetBatchEquipmentStatusAsync(List<string> equipmentIds);
    Task<AdapterResult<DataCollectionResult>> CollectProcessDataAsync(string equipmentId);
    Task<AdapterResult<AlarmPushResult>> PushAlarmAsync(AlarmData alarmData);
    Task<AdapterResult<CommandResult>> SendCommandAsync(EquipmentCommand command);
}

public class ParameterSetData
{
    public string EquipmentId { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string? LotId { get; set; }
    public List<RecipeParameter> Parameters { get; set; } = new();
}

public class RecipeParameter
{
    public string ParameterName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Unit { get; set; }
}

public class ParameterDownloadResult
{
    public string EquipmentId { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public bool Downloaded { get; set; }
    public string? DownloadTime { get; set; }
}

public class EquipmentStatusData
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Idle, Running, Down, Maintenance
    public string? CurrentRecipe { get; set; }
    public string? CurrentLot { get; set; }
    public int? CurrentStep { get; set; }
    public double? Uptime { get; set; }
    public DateTime? LastHeartbeat { get; set; }
    public List<EquipmentAlarm>? ActiveAlarms { get; set; }
}

public class EquipmentAlarm
{
    public string AlarmId { get; set; } = string.Empty;
    public string AlarmCode { get; set; } = string.Empty;
    public string AlarmMessage { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Warning, Error, Critical
    public DateTime AlarmTime { get; set; }
}

public class DataCollectionResult
{
    public string EquipmentId { get; set; } = string.Empty;
    public int CollectedCount { get; set; }
    public DateTime CollectionTime { get; set; }
}

public class AlarmData
{
    public string EquipmentId { get; set; } = string.Empty;
    public string AlarmCode { get; set; } = string.Empty;
    public string AlarmMessage { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime AlarmTime { get; set; }
    public string? LotId { get; set; }
    public string? StepCode { get; set; }
}

public class AlarmPushResult
{
    public bool Pushed { get; set; }
    public string? AlarmRecordId { get; set; }
}

public class EquipmentCommand
{
    public string EquipmentId { get; set; } = string.Empty;
    public string CommandType { get; set; } = string.Empty; // Start, Stop, Pause, Resume, Reset
    public string? CommandData { get; set; }
    public string? OperatorId { get; set; }
}

public class CommandResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public DateTime? ExecutionTime { get; set; }
}
