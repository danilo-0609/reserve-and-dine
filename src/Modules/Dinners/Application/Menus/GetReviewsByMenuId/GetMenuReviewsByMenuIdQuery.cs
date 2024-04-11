using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.GetReviewsByMenuId;

public sealed record GetMenuReviewsByMenuIdQuery(Guid MenuId) : IQuery<ErrorOr<List<MenuReviewResponse>>>;
