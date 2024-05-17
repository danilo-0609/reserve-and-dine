using Microsoft.EntityFrameworkCore;
using Users.Domain.UserRegistrations;

namespace Users.Infrastructure.Domain.UserRegistrations;

internal sealed class UserRegistrationRepository : IUserRegistrationRepository
{
    private readonly UsersDbContext _dbContext;

    public UserRegistrationRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(UserRegistration userRegistration)
    {
        await _dbContext
            .UserRegistrations
            .AddAsync(userRegistration);
    }

    public async Task<UserRegistration?> GetByIdAsync(UserRegistrationId userRegistrationId)
    {
        return await _dbContext
            .UserRegistrations
            .Where(r => r.Id == userRegistrationId)
            .SingleOrDefaultAsync();
    }

    public async Task<bool> IsEmailUnique(string email, CancellationToken cancellationToken)
    {
        return await _dbContext
            .UserRegistrations
            .AnyAsync(r => r.Email == email);
    }

    public Task UpdateAsync(UserRegistration userRegistration)
    {
        _dbContext.UserRegistrations.Update(userRegistration);

        return Task.CompletedTask;
    }
}
