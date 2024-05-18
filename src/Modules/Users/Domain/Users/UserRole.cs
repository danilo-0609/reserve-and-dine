namespace Users.Domain.Users;

public sealed class UserRole
{
    public UserId UserId { get; set; }  

    public Guid RoleId { get; set; }
}
