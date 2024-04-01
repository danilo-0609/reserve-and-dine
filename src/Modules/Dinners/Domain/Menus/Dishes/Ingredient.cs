using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Menus.Dishes;

public sealed class Ingredient : Entity<IngredientId, Guid>
{
    public new IngredientId Id { get; private set; }

    public MenuId MenuId { get; private set; }  

    public string Value { get; private set; }

    public Ingredient(IngredientId id ,string value, MenuId menuId)
    {
        Id = id;
        MenuId = menuId;
        Value = value;
    }

    private Ingredient() { }
}
