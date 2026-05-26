using MES.Domain.Common;

namespace MES.Domain.Events.Production;

public class WorkOrderCreatedEvent : DomainEvent
{
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
}
