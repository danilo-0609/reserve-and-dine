using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Domain.Rules;

namespace BuildingBlocks.Domain.AggregateRoots;

public abstract class AggregateRoot<TId, TIdType> : Entity<TId, TIdType>, IAggregateRoot, IHasDomainEvents, ICheckRule
    where TId : AggregateRootId<TIdType>
    where TIdType : notnull
{
    public new AggregateRootId<TIdType> Id { get; protected set; }

    protected AggregateRoot(TId id)
        : base(id)
    {
        Id = id;
    }

    protected AggregateRoot() { }
}