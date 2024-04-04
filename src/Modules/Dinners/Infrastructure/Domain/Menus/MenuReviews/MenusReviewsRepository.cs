using Dinners.Domain.Menus;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Infrastructure.Domain.Menus.MenuReviews;

internal sealed class MenusReviewsRepository : IMenusReviewsRepository
{
    private readonly DinnersDbContext _dbContext;

    public MenusReviewsRepository(DinnersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(MenusReviews menusReviews, CancellationToken cancellationToken)
    {
        await _dbContext.MenusReviews.AddAsync(menusReviews, cancellationToken);
    }

    public Task Delete(MenusReviews menusReviews)
    {
        _dbContext.MenusReviews
            .Where(r => r.MenuReviewId == menusReviews.MenuReviewId && 
                   r.MenuId == menusReviews.MenuId)
            .ExecuteDelete();

        return Task.CompletedTask;
    }
}
