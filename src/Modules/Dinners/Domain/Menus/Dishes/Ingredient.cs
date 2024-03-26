namespace Dinners.Domain.Menus.Dishes;

public sealed record Ingredient
{
    public string Value { get; private set; }

    public Ingredient(string value)
    {
        Value = value;
    }

    private Ingredient() { }
}
