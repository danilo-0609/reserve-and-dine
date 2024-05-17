using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Users.Infrastructure.Configuration;

public class DatabaseDesignTimeDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<UsersDbContext>();
        
        builder.UseSqlServer("Server=localhost,1433;Initial Catalog=RestaurantReservations;User ID =sa;Password=SqlP@ssword;TrustServerCertificate=True;",
                r => r.EnableRetryOnFailure(4));

        return new UsersDbContext(builder.Options);
    }
}
