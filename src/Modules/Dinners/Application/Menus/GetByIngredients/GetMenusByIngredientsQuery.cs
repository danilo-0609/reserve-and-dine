using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.GetByIngredients;

public sealed record GetMenusByIngredientsQuery(List<string> Ingredients) : IQuery<ErrorOr<List<MenuResponse>>>;
