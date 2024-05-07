using Microsoft.AspNetCore.Identity;

namespace API.Modules.Users.Entities;

public sealed class User : IdentityUser<Guid>
{
}
