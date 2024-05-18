using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace API.Connections;

public class ConnectionsManager : IConnectionsManager
{
    private readonly IConfiguration _configuration;
    private readonly SecretClient _client;

    public ConnectionsManager(IConfiguration configuration)
    {
        _configuration = configuration;

        _client = new SecretClient(
        new Uri(_configuration.GetConnectionString("AzureKeyVaultUri")!),
        credential: new DefaultAzureCredential());
    }

    public async Task<string> GetDatabaseConnectionString()
    {
        var databaseConnectionString = await _client.GetSecretAsync("DBCONNECTIONSTRING");

        return databaseConnectionString.Value.Value;
    }

    public async Task<string> GetAzureBlobStorageConnectionString()
    {
        var azureConnectionString = await _client.GetSecretAsync("AZURESTORAGECONNECTIONSTRING");

        return azureConnectionString.Value.Value;
    }

    public async Task<string> GetAzureRedisConnectionString()
    {
        var redisConnectionString = await _client.GetSecretAsync("AZUREREDISCONNECTIONSTRING");

        return redisConnectionString.Value.Value;
    }

    public async Task<string> GetDockerDatabaseConnectionString()
    {
        var dockerDbConnectionString = await _client.GetSecretAsync("DOCKERDBCONNECTIONSTRING");

        return dockerDbConnectionString.Value.Value;
    }

    public async Task<string> GetJWTIssuer()
    {
        var jwtIssuer = await _client.GetSecretAsync("ApplicationJWTIssuer");

        return jwtIssuer.Value.Value;
    }

    public async Task<string> GetJWTAudience()
    {
        var jwtAudience = await _client.GetSecretAsync("ApplicationJWTAudience");

        return jwtAudience.Value.Value;
    }

    public async Task<string> GetJWTSecretKey()
    {
        var jwtKey = await _client.GetSecretAsync("ApplicationJWTKey");

        return jwtKey.Value.Value;
    }
}