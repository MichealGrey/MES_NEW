using MES.Domain.Common;

namespace MES.Domain.Events.Production;

public class LotHoldEvent : DomainEvent
{
    public string LotId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
