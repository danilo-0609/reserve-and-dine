using Microsoft.AspNetCore.Mvc.Testing;
using API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Dinners.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace Dinners.Tests.IntegrationTests;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string Database = "RestaurantReservations";
    private const string Username = "sa";
    private const string Password = "SqlP@ssword";
    private const ushort MsSqlPort = 1433;

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPortBinding(MsSqlPort)
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithEnvironment("SQLCMDUSER", Username)
        .WithEnvironment("SQLCMDPASSWORD", Password)
        .WithEnvironment("MSSQL_SA_PASSWORD", Password)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.FirstOrDefault(r => 
                r.ServiceType == typeof(DbContextOptions<DinnersDbContext>));
        
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<DinnersDbContext>(options =>
            {
                _dbContainer.ExecAsync(new List<string>() { "CREATE DATABASE RestaurantReservations;" });

                options.UseSqlServer(
                    $"Server=localhost,1433;Initial Catalog={Database};User ID =sa;Password={Password};TrustServerCertificate=True;");
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        try
        {
            var dbContextOptions = new DbContextOptionsBuilder<DinnersDbContext>()
                .UseSqlServer($"Server=localhost,1433;Initial Catalog={Database};User ID =sa;Password=SqlP@ssword;TrustServerCertificate=True;")
                .Options;

            using (var dbContext = new DinnersDbContext(dbContextOptions))
            {
                await dbContext.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during database migration: {ex.Message}");
            throw;
        }
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}
