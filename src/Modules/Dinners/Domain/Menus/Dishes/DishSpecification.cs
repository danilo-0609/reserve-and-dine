namespace Dinners.Domain.Menus.Dishes;

public sealed record DishSpecification
{
    public List<Ingredient> Ingredients { get; private set; }

    public string MainCourse { get; private set; }

    public string SideDishes { get; private set; }

    public string Appetizers { get; private set; }

    public string Beverages { get; private set; }

    public string Desserts { get; private set; }

    public string Sauces { get; private set; }

    public string Condiments { get; private set; }

    public string Coffee { get; private set; }


    public static DishSpecification Create(List<string> ingredients,
        string mainCourse = "",
        string sideDishes = "",
        string appetizers = "",
        string beverages = "",
        string desserts = "",
        string sauces = "",
        string condiments = "",
        string coffee = "")
    {
        return new DishSpecification(ingredients.ConvertAll(value => new Ingredient(value)),
            mainCourse,
            sideDishes,
            appetizers,
            beverages,
            desserts,
            sauces,
            condiments,
            coffee);
    }

    private DishSpecification(List<Ingredient> ingredients,
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

#pragma warning disable CS0169

    private DishSpecification() { }

#pragma warning disable CS0169
}
