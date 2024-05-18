using BuildingBlocks.Domain.AggregateRoots;
using Users.Domain.Common;
using Users.Domain.UserRegistrations;
using Users.Domain.Users.Events;

namespace Users.Domain.Users;

public sealed class User : AggregateRoot<UserId, Guid>
{
    public new UserId Id { get; private set; }

    public string Login { get; private set; }

    public Password Password { get; private set; }

    public string Email { get; private set; }

    public List<Role> Roles { get; private set; }

    public string ProfileImageUrl { get; private set; } = string.Empty;

    public DateTime CreatedDateTime { get; private set; }

    public DateTime? UpdatedDateTime { get; private set; }

    internal static User CreateUserFromRegistration(
        UserRegistrationId userRegistrationId,
        string login,
        Password password,
        string email,
        DateTime createdDateTime)
    {
        User user = new User(
            UserId.Create(userRegistrationId.Value),
            login,
            password,
            email,
            new List<Role>()
            {
                Role.Client
            },
            createdDateTime,
            null);

        user.AddDomainEvent(new UserCreatedDomainEvent(
            Guid.NewGuid(),
            user.Id,
            createdDateTime));

        return user;
    }

    public static User Update(
        UserId id,
        string login,
        string password,
        string email,
        List<Role> roles,
        DateTime createdDateTime,
        DateTime updatedDateTime,
        string profileImageName = "")
    {
        return new User(
            id,
            login,
            Password.Create(password),
            email,
            roles,
            profileImageName,
            createdDateTime,
            updatedDateTime);
    }

    public void ChangePassword(Password password)
    {
        Password = password;
    }

    public void AddRole(Role role)
    {
        Roles.Add(role);
    }

    public void SetProfileImageUrl(string url)
    {
        ProfileImageUrl = url;
    }

    public void DeleteProfileImageUrl()
    {
        ProfileImageUrl = "";
    }

    private User(
        UserId id,
        string login,
        Password password,
        string email,
        List<Role> roles,
        DateTime createdDateTime,
        DateTime? updatedDateTime)
        : base(id)
    {
        Id = id;
        Login = login;
        Password = password;
        Email = email;

        Roles = roles;

        ProfileImageUrl = "";
        CreatedDateTime = createdDateTime;
        UpdatedDateTime = updatedDateTime;
    }

    private User(
    UserId id,
    string login,
    Password password,
    string email,
    List<Role> roles,
    string profileImageName,
    DateTime createdDateTime,
    DateTime? updatedDateTime)
    : base(id)
    {
        Id = id;
        Login = login;
        Password = password;
        Email = email;

        Roles = roles;

        ProfileImageUrl = profileImageName;
        CreatedDateTime = createdDateTime;
        UpdatedDateTime = updatedDateTime;
    }

    private User() { }
}
