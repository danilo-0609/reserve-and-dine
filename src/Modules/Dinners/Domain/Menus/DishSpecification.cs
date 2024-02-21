namespace Dinners.Domain.Menus;

public sealed record DishSpecification
{
    public List<string> Ingredients { get; private set; }

    public string MainCourse { get; private set; }

    public string SideDishes { get; private set; } = string.Empty;

    public string Appetizers { get; private set; } = string.Empty;

    public string Beverages { get; private set; } = string.Empty;

    public string Desserts { get; private set; } = string.Empty;

    public string Sauces { get; private set; } = string.Empty;

    public string Condiments { get; private set; } = string.Empty;

    public string Coffee { get; private set; } = string.Empty;

    public DishSpecification(List<string> ingredients, 
        string mainCourse, 
        string sideDishes, 
        string appetizers, 
        string beverages, 
        string desserts, 
        string sauces, 
        string condiments, 
        string coffee)
    {
        Ingredients = ingredients;
        MainCourse = mainCourse;
        SideDishes = sideDishes;
        Appetizers = appetizers;
        Beverages = beverages;
        Desserts = desserts;
        Sauces = sauces;
        Condiments = condiments;
        Coffee = coffee;
    }
}
