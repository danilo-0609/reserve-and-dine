using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.Publish;

public sealed record PublishMenuCommand(Guid RestaurantId,
        string Title,
        string Description,
        string MenuType,
        decimal Price,
        string Currency,
        decimal Discount,
        List<string?> MenuImagesUrl,
        List<string?> Tags,
        bool IsVegetarian,
        string PrimaryChefName,
        bool HasAlcohol,
        List<string> Ingredients,
        List<DayOfWeek> DayOfWeeks,
        TimeSpan Start,
        TimeSpan End,
        string MainCourse = "",
        string SideDishes = "",
        string Appetizers = "",
        string Beverages = "",
        string Desserts = "",
        string Sauces = "",
        string Condiments = "",
        string Coffee = "",
        string DiscountTerms = "") : ICommand<ErrorOr<Guid>>;
