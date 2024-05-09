using BuildingBlocks.Domain.AggregateRoots;

namespace Users.Domain.Users;

public sealed record UserId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    public static UserId Create(Guid value) => new UserId(value);

    public static UserId CreateUnique() => new UserId(Guid.NewGuid());

    private UserId(Guid value)
    {
        Value = value;
    }

    private UserId() { }
}