using Users.Domain.UserRegistrations;

namespace Users.Infrastructure.Domain.Users.UsersCounter;

internal sealed class UsersCounter : IUsersCounter
{
    private readonly UsersDbContext _dbContext;

    public UsersCounter(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public int CountUsersWithLogin(string login)
    {
        return _dbContext
            .UserRegistrations
            .Where(r => r.Login == login)
            .Count();
    }
}
