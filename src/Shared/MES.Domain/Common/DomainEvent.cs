namespace MES.Domain.Common;

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
