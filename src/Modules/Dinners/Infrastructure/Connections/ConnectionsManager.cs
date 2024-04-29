using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Dinners.Infrastructure.Connections;

public class ConnectionsManager : IConnectionsManager
{
    private static string _kvUri = "https://reservationappkeyvault.vault.azure.net/";

    private static SecretClient _client = new SecretClient(
        new Uri(_kvUri), 
        credential: new DefaultAzureCredential());

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
}