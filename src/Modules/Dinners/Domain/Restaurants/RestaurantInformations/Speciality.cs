using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed class Speciality : Entity<SpecialityId, Guid>
{
    public new SpecialityId Id { get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public string Value { get; private set; }

    public Speciality(SpecialityId id, RestaurantId restaurantId, string value)
    {
        Id = id;
        RestaurantId = restaurantId;
        Value = value;
    }

    private Speciality() { }

}
