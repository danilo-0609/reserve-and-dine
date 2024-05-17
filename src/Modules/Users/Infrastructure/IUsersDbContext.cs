using Microsoft.EntityFrameworkCore;
using Users.Domain.UserRegistrations;
using Users.Domain.Users;
using Users.Infrastructure.Outbox;

namespace Users.Infrastructure;

internal interface IUsersDbContext
{
    DbSet<User> Users { get; }

    DbSet<UserRegistration> UserRegistrations { get; }
    
    DbSet<OutboxMessage> OutboxMessages { get; }
}
