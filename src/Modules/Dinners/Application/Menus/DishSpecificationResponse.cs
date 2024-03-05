namespace Dinners.Application.Menus;

public sealed record DishSpecificationResponse(List<string> Ingredients,
    string MainCourse,
    string SideDishes,
    string Appetizers,
    string Beverages,
    string Desserts,
    string Sauces,
    string Condiments,
    string Coffee);
