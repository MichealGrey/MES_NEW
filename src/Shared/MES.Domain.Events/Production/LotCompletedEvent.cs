using MES.Domain.Common;

namespace MES.Domain.Events.Production;

public class LotCompletedEvent : DomainEvent
{
    public string LotId { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
}
