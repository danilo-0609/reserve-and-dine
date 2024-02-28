namespace Dinners.Domain.Menus;

public sealed record MenuConsumer
{
    public Guid ClientId { get; private set; }

    public MenuId MenuId { get; private set; }

    public MenuConsumer(Guid clientId, MenuId menuId)
    {
        ClientId = clientId;
        MenuId = menuId;
    }

    private MenuConsumer() { }
}
