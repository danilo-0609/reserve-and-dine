namespace Dinners.Domain.Menus;

public sealed record MenuType
{
    public string Value { get; private set; }

    public MenuType Breakfast => new MenuType(nameof(Breakfast));

    public MenuType Lunch => new MenuType(nameof(Lunch));

    public MenuType Dinner => new MenuType(nameof(Dinner));

    public MenuType(string value)
    {
        Value = value;
    }
}
