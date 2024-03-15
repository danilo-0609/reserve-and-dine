using Dinners.Domain.Menus;
using Dinners.Domain.Menus.MenuReviews;
using Microsoft.Extensions.Caching.Memory;

namespace Dinners.Infrastructure.Cache.Menus;

internal sealed class CacheMenuRepository : IMenuRepository
{
    private readonly IMenuRepository _decorated;
    private readonly IMemoryCache _memoryCache;

    public CacheMenuRepository(IMenuRepository decorated, IMemoryCache memoryCache)
    {
        _decorated = decorated;
        _memoryCache = memoryCache;
    }

    public async Task AddAsync(Menu menu, CancellationToken cancellationToken)
    {
        await _decorated.AddAsync(menu, cancellationToken);
    }

    public async Task<Menu?> GetByIdAsync(MenuId menuId, CancellationToken cancellationToken)
    {
        string key = $"menu-{menuId.Value}";
    
        return await _memoryCache.GetOrCreateAsync(
            key,
            async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                return await _decorated.GetByIdAsync(menuId, cancellationToken);
            });
    }

    public async Task<List<string>> GetMenuImagesUrlById(MenuId menuId, CancellationToken cancellationToken)
    {
        string key = $"menuImagesUrl-{menuId}";

        return await _memoryCache.GetOrCreateAsync(
            key,
            async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                return await _decorated.GetMenuImagesUrlById(menuId, cancellationToken);
            });
    }

    public async Task<List<MenuReviewId>> GetMenuReviewsIdByIdAsync(MenuId menuId, CancellationToken cancellationToken)
    {
        string key = $"menuReviewsId-{menuId}";

        return await _memoryCache.GetOrCreateAsync(
            key,
            async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                return await _decorated.GetMenuReviewsIdByIdAsync(menuId, cancellationToken);
            });
    }

    public async Task<List<Menu>> GetMenusByIngredientAsync(List<string> ingredients, CancellationToken cancellationToken)
    {
        string key = $"menus-ingredients";

        return await _memoryCache.GetOrCreateAsync(
            key,
            async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                return await _decorated.GetMenusByIngredientAsync(ingredients, cancellationToken);
            });
    }

    public async Task<List<Menu>> GetMenusByNameAsync(string name, CancellationToken cancellationToken)
    {
        string key = $"menus-{name}";

        return await _memoryCache.GetOrCreateAsync(
            key,
            async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                return await _decorated.GetMenusByNameAsync(name, cancellationToken);
            });
    }

    public async Task UpdateAsync(Menu menu, CancellationToken cancellationToken)
    {
        await _decorated.UpdateAsync(menu, cancellationToken);
    }
}
