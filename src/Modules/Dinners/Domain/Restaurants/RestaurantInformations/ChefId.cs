﻿using BuildingBlocks.Domain.Entities;
using Newtonsoft.Json;

namespace Dinners.Domain.Restaurants.RestaurantInformations;

public sealed record ChefId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static ChefId Create(Guid value) => new ChefId(value);

    public static ChefId CreateUnique() => new ChefId(Guid.NewGuid());

    [JsonConstructor]
    private ChefId(Guid value)
    {
        Value = value;
    }
}
