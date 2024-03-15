using Dinners.Domain.Menus;
using Dinners.Domain.Menus.MenuReviews;
using Microsoft.EntityFrameworkCore;

namespace Dinners.Infrastructure.Domain.Menus;

internal sealed class MenuRepository : IMenuRepository
{
    private readonly DinnersDbContext _dbContext;

    public MenuRepository(DinnersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Menu menu, CancellationToken cancellationToken)
    {
        await _dbContext.Menus.AddAsync(menu, cancellationToken);
    }

    public async Task<Menu?> GetByIdAsync(MenuId menuId, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Menus
            .Where(r => r.Id == menuId)
            .SingleOrDefaultAsync();
    }

    public async Task<List<string>> GetMenuImagesUrlById(MenuId menuId, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Menus
            .Where(r => r.Id == menuId)
            .SelectMany(x => x.MenuDetails.MenuImagesUrl.Select(r => r.AbsoluteUri))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MenuReviewId>> GetMenuReviewsIdByIdAsync(MenuId menuId, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Menus
            .Where(r => r.Id == menuId)
            .SelectMany(x => x.MenuReviewIds)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Menu>> GetMenusByIngredientAsync(List<string> ingredients, CancellationToken cancellationToken)
    {
        List<Menu> menus = new();
    
        foreach(var ingredient in ingredients)
        {
            await _dbContext
                .Menus
                .Where(x => x.DishSpecification.Ingredients.Contains(ingredient))
                .SingleOrDefaultAsync();
        }

        return menus;
    }

    public async Task<List<Menu>> GetMenusByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Menus
            .Where(r => r.MenuDetails.Title == name)
            .ToListAsync();
    }

    public async Task UpdateAsync(Menu menu, CancellationToken cancellationToken)
    {
        await _dbContext
            .Menus
            .ExecuteUpdateAsync(x =>
                x.SetProperty(r => r.Id, menu.Id)
                 .SetProperty(r => r.RestaurantId, menu.RestaurantId)
                 .SetProperty(r => r.MenuDetails, menu.MenuDetails)
                 .SetProperty(r => r.DishSpecification, menu.DishSpecification)
                 .SetProperty(r => r.MenuReviewIds, menu.MenuReviewIds)
                 .SetProperty(r => r.MenuSchedule, menu.MenuSchedule)
                 .SetProperty(r => r.MenuConsumers, menu.MenuConsumers)
                 .SetProperty(r => r.CreatedOn, menu.CreatedOn)
                 .SetProperty(r => r.UpdatedOn, menu.UpdatedOn));
    }
}
