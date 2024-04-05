using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.GetByIngredients;

public sealed record GetMenusByIngredientQuery(string Ingredient) : IQuery<ErrorOr<List<MenuResponse>>>;
