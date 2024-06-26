﻿namespace API.Modules.Dinners.Requests.Menus;

public sealed record UpdateMenuDetailsRequest(string Title,
    string Description,
    string MenuType,
    string DiscountTerms,
    decimal Money,
    string Currency,
    decimal Discount,
    bool IsVegetarian,
    string PrimaryChefName,
    bool HasAlcohol);
