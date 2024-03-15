using Dinners.Domain.Menus.MenuReviews;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Infrastructure.Domain.Menus.Reviews;

internal sealed class ReviewRepository : IMenuReviewRepository
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

    public async Task UpdateAsync(MenuReview menuReview)
    {
        await _dbContext
            .Reviews
            .ExecuteUpdateAsync(x =>
                x.SetProperty(r => r.Id, menuReview.Id)
                 .SetProperty(r => r.Rate, menuReview.Rate)
                 .SetProperty(r => r.ClientId, menuReview.ClientId)
                 .SetProperty(r => r.Comment, menuReview.Comment)
                 .SetProperty(r => r.ReviewedAt, menuReview.ReviewedAt)
                 .SetProperty(r => r.UpdatedAt, menuReview.UpdatedAt));
    }
}
