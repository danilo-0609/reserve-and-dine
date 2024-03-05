using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.GetReviewsByMenuId;

internal sealed record GetMenuReviewsByMenuIdQuery(Guid MenuId) : IQuery<ErrorOr<List<MenuReviewResponse>>>;
