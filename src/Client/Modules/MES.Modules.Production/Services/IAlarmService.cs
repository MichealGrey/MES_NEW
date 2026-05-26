using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IAlarmService
{
    Task<AlarmRecord> RaiseAlarmAsync(string alarmType, string message, string? lotId = null, string? equipmentId = null, string? stepCode = null, string? detail = null);
    Task AcknowledgeAlarmAsync(string alarmId, string acknowledgedBy);
    Task ResolveAlarmAsync(string alarmId, string resolvedBy);
    Task<List<AlarmRecord>> GetActiveAlarmsAsync();
    Task<List<AlarmRecord>> GetAlarmsByTypeAsync(string alarmType);
    Task CheckAndRaiseAsync(string alarmType, string lotId, string? equipmentId, string? stepCode, double? yieldValue = null, int? qtyValue = null, TimeSpan? duration = null);
}
