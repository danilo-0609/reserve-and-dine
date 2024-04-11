using BuildingBlocks.Domain.Entities;
using Newtonsoft.Json;

namespace Dinners.Domain.Restaurants.RestaurantSchedules;

public sealed record RestaurantScheduleId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static RestaurantScheduleId Create(Guid value) => new RestaurantScheduleId(value);

    public static RestaurantScheduleId CreateUnique() => new RestaurantScheduleId(Guid.NewGuid());

    [JsonConstructor]
    private RestaurantScheduleId(Guid value)
    {
        Value = value;
    }
}
