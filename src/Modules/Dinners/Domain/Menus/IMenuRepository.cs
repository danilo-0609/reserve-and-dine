using Dinners.Domain.Menus.MenuReviews;

namespace Dinners.Domain.Menus;

public interface IMenuRepository
{
    Task AddAsync(Menu menu, CancellationToken cancellationToken);

    Task<Menu?> GetByIdAsync(MenuId menuId, CancellationToken cancellationToken);
    
    Task<bool> ExistsAsync(MenuId menuId, CancellationToken cancellationToken);

    Task UpdateAsync(Menu menu, CancellationToken cancellationToken);

    Task<List<Menu>> GetMenusByIngredientAsync(string ingredient, CancellationToken cancellationToken);

    Task<List<Menu>> GetMenusByNameAsync(string name, CancellationToken cancellationToken);

    Task<List<string>> GetMenuImagesUrlById(MenuId menuId, CancellationToken cancellationToken);

    Task<List<MenuReviewId>> GetMenuReviewsIdByIdAsync(MenuId menuId, CancellationToken cancellationToken);
}

