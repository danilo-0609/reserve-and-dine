namespace Dinners.Domain.Menus.MenuReviews;

public interface IReviewRepository
{
    Task AddAsync(MenuReview menuReview);

    Task UpdateAsync(MenuReview menuReview);

    Task<MenuReview?> GetByIdAsync(MenuReviewId menuReviewId, CancellationToken cancellationToken);  
}
