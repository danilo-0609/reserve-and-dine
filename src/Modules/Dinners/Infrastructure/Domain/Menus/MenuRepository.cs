using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Details;
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

    public async Task<bool> ExistsAsync(MenuId menuId, CancellationToken cancellationToken)
    {
        return await _dbContext.Menus.AnyAsync(menu => menu.Id == menuId, cancellationToken);
    }

    public async Task<Menu?> GetByIdAsync(MenuId menuId, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Menus
            .Where(r => r.Id == menuId)
            .SingleOrDefaultAsync();
    }

    public async Task<string?> GetMenuImageUrlById(MenuId menuId, MenuImageUrlId imageUrlId, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Menus
            .Where(r => r.Id == menuId)
            .SelectMany(x => x.MenuImagesUrl
                .Where(r => r.Id == imageUrlId)
                .Select(r => r.Value))
            .SingleOrDefaultAsync();
    }

    public async Task<List<MenuReviewId>> GetMenuReviewsIdByMenuIdAsync(MenuId menuId, CancellationToken cancellationToken)
    {
        var menu = await _dbContext
            .Menus
            .Where(r => r.Id == menuId)
            .SingleOrDefaultAsync(cancellationToken);
    
        if (menu is null)
        {
            return new List<MenuReviewId>();
        }

        return menu.MenuReviewIds.ToList();
    }

    public async Task<List<Menu>> GetMenusByIngredientAsync(string ingredient, CancellationToken cancellationToken)
    {
        var menus = await _dbContext
            .Menus
            .Where(r => r.Ingredients.Any(t => t.Value == ingredient) == true)
            .ToListAsync();

        return menus;
    }

    public async Task<List<Menu>> GetMenusByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Menus
            .Where(r => r.MenuDetails.Title == name)
            .ToListAsync();
    }

    public Task UpdateAsync(Menu menu, CancellationToken cancellationToken)
    {
        _dbContext.Menus.Update(menu);

        return Task.CompletedTask;
    }

    public async Task DeleteAsync(MenuId menuId)
    {
        await _dbContext.Menus.Where(r => r.Id == menuId).ExecuteDeleteAsync();
    }
}
