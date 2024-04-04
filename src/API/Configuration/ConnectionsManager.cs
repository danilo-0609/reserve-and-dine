using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Domain.Restaurants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.Configuration;

public static class ConnectionsManager
{
    private static string _kvUri = "https://reservationappkeyvault.vault.azure.net/";

    private static SecretClient _client = new SecretClient(new Uri(_kvUri), new DefaultAzureCredential());


    public static async Task<string> GetDatabaseConnectionString()
    {
        var databaseConnectionString = await _client.GetSecretAsync("DBCONNECTIONSTRING");

        return databaseConnectionString.Value.Value;
    }

    public static async Task<string> GetAzureBlobStorageConnectionString()
    {
        var azureConnectionString = await _client.GetSecretAsync("AZURESTORAGECONNECTIONSTRING");

        return azureConnectionString.Value.Value;
    }

    public static async Task<string> GetAzureRedisConnectionString()
    {
        var redisConnectionString = await _client.GetSecretAsync("AZUREREDISCONNECTIONSTRING");

        return redisConnectionString.Value.Value;
    }
}