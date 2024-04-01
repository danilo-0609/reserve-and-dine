using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Menus.Dishes;

public sealed record IngredientId : EntityId<Guid>
{
    public override Guid Value { get; protected set; }

    public static IngredientId Create(Guid value) => new IngredientId(value);

    public static IngredientId CreateUnique() => new IngredientId(Guid.NewGuid());  

    private IngredientId(Guid value)
    {
        Value = value;
    }
}
