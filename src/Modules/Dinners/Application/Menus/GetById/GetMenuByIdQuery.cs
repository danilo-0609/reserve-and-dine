using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.GetById;

public sealed record GetMenuByIdQuery(Guid MenuId) : IQuery<ErrorOr<MenuResponse>>;
