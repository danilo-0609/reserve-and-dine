﻿using BuildingBlocks.Domain.Entities;
using Newtonsoft.Json;

namespace Dinners.Domain.Restaurants.RestaurantUsers;

public sealed record RestaurantClientId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static RestaurantClientId Create(Guid value) => new RestaurantClientId(value);

    public static RestaurantClientId CreateUnique() => new RestaurantClientId(Guid.NewGuid());

    [JsonConstructor]
    private RestaurantClientId(Guid value)
    {
        Value = value;
    }
}
