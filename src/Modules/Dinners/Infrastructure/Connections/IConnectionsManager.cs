namespace Dinners.Infrastructure.Connections;

public interface IConnectionsManager
{
    Task<string> GetDatabaseConnectionString();

    Task<string> GetAzureBlobStorageConnectionString();

    Task<string> GetAzureRedisConnectionString();

    Task<string> GetDockerDatabaseConnectionString();
}
