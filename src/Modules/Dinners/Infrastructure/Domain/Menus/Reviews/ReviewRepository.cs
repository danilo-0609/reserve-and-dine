using Dinners.Domain.Menus.MenuReviews;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Infrastructure.Domain.Menus.Reviews;

internal sealed class ReviewRepository : IReviewRepository
{
    private readonly DinnersDbContext _dbContext;

    public ReviewRepository(DinnersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(MenuReview menuReview)
    {
        await _dbContext.Reviews.AddAsync(menuReview);
    }

    public async Task<MenuReview?> GetByIdAsync(MenuReviewId menuReviewId, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Reviews
            .Where(r => r.Id == menuReviewId)
            .SingleOrDefaultAsync();
    }

    public Task UpdateAsync(MenuReview menuReview)
    {
        _dbContext.Reviews.Update(menuReview);

        return Task.CompletedTask;
    }
}
