namespace Dinners.Domain.Menus.MenuReviews;

public interface IMenuReviewRepository
{
    Task AddAsync(MenuReview menuReview);

    Task UpdateAsync(MenuReview menuReview);

    Task<MenuReview?> GetByIdAsync(MenuReviewId menuReviewId);  
}
