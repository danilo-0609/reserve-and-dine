using Microsoft.AspNetCore.Identity;

namespace API.Modules.Users.Entities;

public sealed class Role : IdentityRole<Guid>
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
