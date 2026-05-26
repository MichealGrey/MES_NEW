using MES.Domain.Common;

namespace MES.Domain.Events.Equipment;

public class EquipmentStateChangedEvent : DomainEvent
{
    public string EquipmentId { get; set; } = string.Empty;
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
}
