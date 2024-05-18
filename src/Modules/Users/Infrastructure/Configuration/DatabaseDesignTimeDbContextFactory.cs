using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Users.Infrastructure.Configuration;

public class DatabaseDesignTimeDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    private readonly IConfiguration _configuration;

    public DatabaseDesignTimeDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public UsersDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<UsersDbContext>();
        
        builder.UseSqlServer(_configuration.GetConnectionString("DockerSqlDatabase"),
                r => r.EnableRetryOnFailure(4));

        return new UsersDbContext(builder.Options);
    }
}
