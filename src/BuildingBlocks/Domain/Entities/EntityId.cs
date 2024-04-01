namespace BuildingBlocks.Domain.Entities;

public abstract record EntityId<TId> : IEquatable<EntityId<TId>>    
    where TId : notnull
{
    public abstract TId Value { get; protected set; }

    protected EntityId(TId value)
    {
        Value = value;
    }

    bool IEquatable<EntityId<TId>>.Equals(EntityId<TId>? other)
    {
        if (other == null)
        {
            return false;
        }

        return Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    protected EntityId() { }
}