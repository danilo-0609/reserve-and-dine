using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Rules;
using ErrorOr;
using MediatR;

namespace BuildingBlocks.Domain.Entities;

public abstract class Entity<TId, TIdType> : IEquatable<Entity<TId, TIdType>>, IHasDomainEvents, IEntity, ICheckRule
    where TId : EntityId<TIdType>
    where TIdType : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public TId Id { get; }

    protected Entity(TId id)
    {
        Id = id;
    }

    //Domain events concerns
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents;

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);


    //Business rules validation
    public ErrorOr<Success> CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            return rule.Error;
        }

        return new Success();
    }


    //Equatable operations
    public bool Equals(Entity<TId, TIdType>? other)
    {
        return Equals((object?)other);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || !(obj is Entity<TId, TIdType> entity))
        {
            return false;
        }

        if (Id is null && entity.Id is null)
        {
            return false;
        }

        return ((IEquatable<EntityId<TId>>)Id).Equals(entity.Id);
    }

    public static bool operator ==(Entity<TId, TIdType> left, Entity<TId, TIdType> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId, TIdType> left, Entity<TId, TIdType> right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    protected Entity() { }
}