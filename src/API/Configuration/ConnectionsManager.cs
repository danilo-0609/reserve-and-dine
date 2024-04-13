using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace API.Configuration;

public static class ConnectionsManager
{
    private static string _kvUri = "https://reservationappkeyvault.vault.azure.net/";

    private static SecretClient _client = new SecretClient(new Uri(_kvUri), new DefaultAzureCredential());

    public async static Task<string> GetDatabaseConnectionString()
    {
        var databaseConnectionString = await _client.GetSecretAsync("DBCONNECTIONSTRING");

        return databaseConnectionString.Value.Value;
    }

    public async static Task<string> GetAzureBlobStorageConnectionString()
    {
        var azureConnectionString = await _client.GetSecretAsync("AZURESTORAGECONNECTIONSTRING");

        return azureConnectionString.Value.Value;
    }

    public async static Task<string> GetAzureRedisConnectionString()
    {
        var redisConnectionString = await _client.GetSecretAsync("AZUREREDISCONNECTIONSTRING");

        return redisConnectionString.Value.Value;
    }
}