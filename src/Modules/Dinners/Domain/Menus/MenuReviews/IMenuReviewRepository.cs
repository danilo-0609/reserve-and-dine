namespace Dinners.Domain.Menus.MenuReviews;

public interface IMenuReviewRepository
{
    Task AddAsync(MenuReview menuReview);
}
