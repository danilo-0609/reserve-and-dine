namespace Dinners.Domain.Menus;

public interface IMenusReviewsRepository
{
    Task AddAsync(MenusReviews menusReviews, CancellationToken cancellationToken);

    Task Delete(MenusReviews menusReviews);
}
