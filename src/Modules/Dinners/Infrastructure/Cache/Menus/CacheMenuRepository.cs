using Dinners.Domain.Menus;
using Dinners.Domain.Menus.MenuReviews;
using Dinners.Infrastructure.Resolvers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Dinners.Infrastructure.Cache.Menus;

internal sealed class CacheMenuRepository : IMenuRepository
{
    private readonly IMenuRepository _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly DinnersDbContext _dbContext;

    public CacheMenuRepository(IMenuRepository decorated, IDistributedCache distributedCache, DinnersDbContext dinnersDbContext)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
        _dbContext = dinnersDbContext;
    }

    public async Task AddAsync(Menu menu, CancellationToken cancellationToken)
    {
        await _decorated.AddAsync(menu, cancellationToken);
    }

    public async Task<Menu?> GetByIdAsync(MenuId menuId, CancellationToken cancellationToken)
    {
        string key = $"menu-{menuId.Value}";
        
        string? cachedMenu = await _distributedCache.GetStringAsync(key,
            cancellationToken);

        Menu? menu;
        if (string.IsNullOrEmpty(cachedMenu))
        {
            menu = await _decorated.GetByIdAsync(menuId, cancellationToken);
        
            if (menu is null)
            {
                return menu;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(menu, 
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return menu;
        }
        
        menu = JsonConvert.DeserializeObject<Menu>(cachedMenu, new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        });

        if (menu is not null)
        {
            _dbContext.Set<Menu>().Attach(menu);
        }

        return menu;
    }

    public async Task<List<string>> GetMenuImagesUrlById(MenuId menuId, CancellationToken cancellationToken)
    {
        string key = $"menuImagesUrl-{menuId.Value}";

        string? cachedMenuImagesUrls = await _distributedCache.GetStringAsync(key,
            cancellationToken);

        List<string> menuImagesUrls;
        if (string.IsNullOrEmpty(cachedMenuImagesUrls))
        {
            menuImagesUrls = await _decorated.GetMenuImagesUrlById(menuId, cancellationToken);

            if (!menuImagesUrls.Any())
            {
                return menuImagesUrls;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(menuImagesUrls, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return menuImagesUrls;
        }

        menuImagesUrls = JsonConvert.DeserializeObject<List<string>>(cachedMenuImagesUrls,
        new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        })!;

        if (!menuImagesUrls.Any())
        {
            _dbContext.Set<Menu>().Attach(await _dbContext
                .Menus
                .Where(r => r.Id == menuId)
                .SingleAsync());
        }

        return menuImagesUrls;
    }

    public async Task<List<MenuReviewId>> GetMenuReviewsIdByIdAsync(MenuId menuId, CancellationToken cancellationToken)
    {
        string key = $"menuReviewsId-{menuId.Value}";

        string? cachedMenuReviewIds = await _distributedCache.GetStringAsync(key,
        cancellationToken);

        List<MenuReviewId> menuReviewsIds;
        if (string.IsNullOrEmpty(cachedMenuReviewIds))
        {
            menuReviewsIds = await _decorated.GetMenuReviewsIdByIdAsync(menuId, cancellationToken);

            if (!menuReviewsIds.Any())
            {
                return menuReviewsIds;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(menuReviewsIds, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
            cancellationToken);

            return menuReviewsIds;
        }

        menuReviewsIds = JsonConvert.DeserializeObject<List<MenuReviewId>>(cachedMenuReviewIds,
        new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        })!;

        if (!menuReviewsIds.Any())
        {
            _dbContext.Set<Menu>().Attach(await _dbContext
                .Menus
                .Where(r => r.Id == menuId)
                .SingleAsync());
        }

        return menuReviewsIds;
    }

    public async Task<List<Menu>> GetMenusByIngredientAsync(string ingredient, CancellationToken cancellationToken)
    {
        string key = $"menu-{ingredient}";

        string? cachedMenus = await _distributedCache.GetStringAsync(key,
            cancellationToken);

        List<Menu> menus;
        if (string.IsNullOrEmpty(cachedMenus))
        {
            menus = await _decorated.GetMenusByIngredientAsync(ingredient, cancellationToken);

            if (!menus.Any())
            {
                return menus;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(menus, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
            cancellationToken);

            return menus;
        }

        menus = JsonConvert.DeserializeObject<List<Menu>>(cachedMenus,
        new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        })!;

        if (!menus.Any())
        {
            _dbContext.Set<Menu>().AttachRange(menus);
        }

        return menus;
    }

    public async Task<List<Menu>> GetMenusByNameAsync(string name, CancellationToken cancellationToken)
    {
        string key = $"menu-{name}";

        string? cachedMenus = await _distributedCache.GetStringAsync(key,
        cancellationToken);

        List<Menu> menus;
        if (string.IsNullOrEmpty(cachedMenus))
        {
            menus = await _decorated.GetMenusByNameAsync(name, cancellationToken);

            if (!menus.Any())
            {
                return menus;
            }

            await _distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(menus, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                }),
                cancellationToken);

            return menus;
        }

        menus = JsonConvert.DeserializeObject<List<Menu>>(cachedMenus,
        new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        })!;

        if (menus.Any())
        {
            _dbContext.Set<Menu>().AttachRange(menus);
        }

        return menus;
    }

    public async Task UpdateAsync(Menu menu, CancellationToken cancellationToken)
    {
        await _decorated.UpdateAsync(menu, cancellationToken);
    }
}
