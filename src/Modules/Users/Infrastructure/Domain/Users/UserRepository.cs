using Microsoft.EntityFrameworkCore;
using Users.Domain.Users;

namespace Users.Infrastructure.Domain.Users;

internal sealed class UserRepository : IUserRepository
{
    private readonly UsersDbContext _dbContext;

    public UserRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext
            .Users
            .AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Users
            .Where(r => r.Email == email)
            .SingleOrDefaultAsync();
    }

    public async Task<User?> GetByIdAsync(UserId userId, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Users
            .Where(r => r.Id == userId)
            .SingleOrDefaultAsync();
    }

    public async Task<bool> IsLoginUnique(string login, CancellationToken cancellationToken)
    {
        return !await _dbContext
            .Users
            .AnyAsync(t => t.Login == login, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _dbContext.Users.Update(user);

        return Task.CompletedTask;
    }
}
