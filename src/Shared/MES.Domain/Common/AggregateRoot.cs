namespace MES.Domain.Common;

public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void AddDomainEvent(DomainEvent eventItem) => _domainEvents.Add(eventItem);
    public void RemoveDomainEvent(DomainEvent eventItem) => _domainEvents.Remove(eventItem);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
