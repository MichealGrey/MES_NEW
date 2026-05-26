using MES.Domain.Common;

namespace MES.Domain.Events.Equipment;

public class EquipmentAlarmEvent : DomainEvent
{
    public string EquipmentId { get; set; } = string.Empty;
    public string AlarmCode { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
