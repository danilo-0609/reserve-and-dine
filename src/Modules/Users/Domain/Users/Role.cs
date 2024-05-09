namespace Users.Domain.Users;

public sealed class Role 
{
    public string Value { get; private set; }

    public Guid RoleId { get; private set; }

    public Role(string value, Guid id)
    {
        Value = value;
        RoleId = id;
    }

    public static Role RestaurantAdministrator => new Role(nameof(RestaurantAdministrator), Guid.NewGuid());

    public static Role Client => new Role(nameof(Client), Guid.NewGuid());
}
