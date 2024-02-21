using BuildingBlocks.Domain.Entities;

namespace BuildingBlocks.Domain.AggregateRoots;

public abstract record AggregateRootId<TId> : EntityId<TId>
    where TId : notnull
{
    protected AggregateRootId(TId value)
    {
        Value = value;
    }

    protected AggregateRootId(EntityId<TId> original) 
        : base(original)
    {
    }

    protected AggregateRootId()
    {
    }
}