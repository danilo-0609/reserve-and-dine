using Dinners.Domain.Common;

namespace Dinners.Application.Menus;

public sealed record MenuDetailsResponse(string Title,
    string Description,
    string MenuType,
    Price Price,
    decimal Discount,
    List<string> Tags,
    bool IsVegetarian,
    string PrimaryChefName,
    bool HasAlcohol,
    string DiscountTerms = "");

