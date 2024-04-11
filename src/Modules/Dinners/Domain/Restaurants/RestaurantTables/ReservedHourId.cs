using BuildingBlocks.Domain.Entities;
using Newtonsoft.Json;

namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed record ReservedHourId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static ReservedHourId Create(Guid value) => new ReservedHourId(value);

    public static ReservedHourId CreateUnique() => new ReservedHourId(Guid.NewGuid());

    [JsonConstructor]
    private ReservedHourId(Guid value)
    {
        Value = value;
    }
}
