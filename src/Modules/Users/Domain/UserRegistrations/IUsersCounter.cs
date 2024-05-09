namespace Users.Domain.UserRegistrations;

public interface IUsersCounter
{
    int CountUsersWithLogin(string login);
}
