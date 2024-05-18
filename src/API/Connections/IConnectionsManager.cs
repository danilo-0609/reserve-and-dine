namespace API.Connections;

public interface IConnectionsManager
{
    Task<string> GetDatabaseConnectionString();

    Task<string> GetAzureBlobStorageConnectionString();

    Task<string> GetAzureRedisConnectionString();

    Task<string> GetDockerDatabaseConnectionString();

    Task<string> GetJWTIssuer();

    Task<string> GetJWTAudience();

    Task<string> GetJWTSecretKey();
}
