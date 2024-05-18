using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Users.Domain.UserRegistrations;
using Users.Domain.Users;
using Users.Infrastructure.Outbox;

namespace Users.Infrastructure;

public sealed class UsersDbContext : DbContext, IUsersDbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<UserRegistration> UserRegistrations { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);

        modelBuilder.Ignore<UserId>();
        modelBuilder.Ignore<UserRegistrationId>();

        base.OnModelCreating(modelBuilder);
    }
}
