using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed record ReservedHourId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static ReservedHourId Create(Guid value) => new ReservedHourId(value);

    public static ReservedHourId CreateUnique() => new ReservedHourId(Guid.NewGuid());

    private ReservedHourId(Guid value)
    {
        Value = value;
    }
}
