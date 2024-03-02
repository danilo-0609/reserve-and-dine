using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.DishSpecifications;

public sealed record UpdateDishSpecificationCommand(Guid MenuId,
    List<string> Ingredients,
    string MainCourse,
    string SideDishes,
    string Appetizers,
    string Beverages,
    string Desserts,
    string Sauces,
    string Condiments,
    string Coffee) : ICommand<ErrorOr<Unit>>;

