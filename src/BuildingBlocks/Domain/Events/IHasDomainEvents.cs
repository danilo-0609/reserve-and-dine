namespace BuildingBlocks.Domain.Events;

public interface IHasDomainEvents
{
    public IReadOnlyList<IDomainEvent> DomainEvents { get; }

    public IReadOnlyList<IDomainEvent> GetDomainEvents();

    public void ClearDomainEvents();
}
