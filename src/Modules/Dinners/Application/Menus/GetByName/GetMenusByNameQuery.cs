using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.GetByName;

public sealed record GetMenusByNameQuery(string Name) : IQuery<ErrorOr<List<MenuResponse>>>;
