using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Restaurants.RestaurantUsers;

public sealed record RestaurantAdministrationId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static RestaurantAdministrationId Create(Guid value) => new RestaurantAdministrationId(value);

    public static RestaurantAdministrationId CreateUnique() => new RestaurantAdministrationId(Guid.NewGuid());

    private RestaurantAdministrationId(Guid value)
    {
        Value = value;
    }
}
