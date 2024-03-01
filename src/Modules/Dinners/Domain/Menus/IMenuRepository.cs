namespace Dinners.Domain.Menus;

public interface IMenuRepository
{
    Task AddAsync(Menu menu);

    Task<Menu?> GetByIdAsync(MenuId menuId);
    
    Task UpdateAsync(Menu menu);    
}
