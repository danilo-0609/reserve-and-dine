using Dinners.Domain.Menus;
using Dinners.Domain.Menus.MenuReviews;

namespace Dinners.Infrastructure.Domain.Menus.MenuReviews;

public sealed class MenusReviews
{
    public MenuId MenuId { get; }

    public MenuReviewId MenuReviewId { get; }
}
