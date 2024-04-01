using BuildingBlocks.Domain.Entities;

namespace Dinners.Domain.Menus.Details;

public sealed class Tag : Entity<TagId, Guid>
{
    public new TagId Id { get; private set; }   

    public MenuId MenuId { get; private set; }

    public string Value { get; private set; }

    public Tag(TagId id, string value, MenuId menuId)
    {
        Id = id;
        MenuId = menuId;
        Value = value;
    }

    private Tag() { }
}
