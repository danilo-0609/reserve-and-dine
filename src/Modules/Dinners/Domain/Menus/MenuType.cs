namespace Dinners.Domain.Menus;

public sealed record MenuType
{
    public string Value { get; private set; }

    public static MenuType Breakfast => new MenuType(nameof(Breakfast));

    public static MenuType Lunch => new MenuType(nameof(Lunch));

    public static MenuType Dinner => new MenuType(nameof(Dinner));

    public MenuType(string value)
    {
        Value = value;
    }
}
