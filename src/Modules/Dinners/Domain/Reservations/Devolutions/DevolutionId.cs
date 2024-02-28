using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Reservations.Devolutions;

public sealed record DevolutionId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static DevolutionId Create(Guid id) => new DevolutionId(id);

    public static DevolutionId CreateUnique() => new DevolutionId(Guid.NewGuid());

    private DevolutionId(Guid value)
    {
        Value = value;
    }

    private DevolutionId() { }
}
