using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.MenuSpecification;

public sealed record UpdateMenuDetailsCommand(Guid MenuId,
    string Title,
    string Description,
    string MenuType,
    string DiscountTerms,
    decimal Money,
    string Currency,
    decimal Discount,
    List<string> MenuImagesUrl,
    List<string> Tags,
    bool IsVegetarian,
    string PrimaryChefName,
    bool HasAlcohol): ICommand<ErrorOr<Unit>>;
