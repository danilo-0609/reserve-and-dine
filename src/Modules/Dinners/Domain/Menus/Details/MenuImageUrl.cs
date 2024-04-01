using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Menus.Details;

public sealed class MenuImageUrl : Entity<MenuImageUrlId, Guid>
{
    public new MenuImageUrlId Id { get; private set; }

    public MenuId MenuId { get; private set; }

    public string Value { get; private set; }

    public MenuImageUrl(MenuImageUrlId id, string value, MenuId menuId)
    {
        Id = id;
        MenuId = menuId;
        Value = value;
    }

    private MenuImageUrl() { }
}
