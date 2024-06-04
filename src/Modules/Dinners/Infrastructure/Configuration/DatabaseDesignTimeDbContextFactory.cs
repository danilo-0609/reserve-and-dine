using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Configuration;

namespace Dinners.Infrastructure.Configuration;

public class DatabaseDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DinnersDbContext>
{
    private readonly IConfiguration _configuration;

    public DatabaseDesignTimeDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DatabaseDesignTimeDbContextFactory()
    {
    }

    public DinnersDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<DinnersDbContext>();

        builder.UseSqlServer(_configuration.GetConnectionString("DockerSqlDatabase"));
        
        return new DinnersDbContext(builder.Options);
    }
}
