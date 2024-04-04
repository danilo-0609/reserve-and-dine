using Dinners.Domain.Menus.MenuReviews;

namespace Dinners.Domain.Menus;

public sealed class MenusReviews
{
    public MenuId MenuId { get; }

    public MenuReviewId MenuReviewId { get; }

    public MenusReviews(MenuId menuId, MenuReviewId menuReviewId)
    {
        MenuId = menuId;
        MenuReviewId = menuReviewId;
    }
}
