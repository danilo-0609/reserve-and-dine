using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace API.Configuration;

public static class ConnectionsManager
{
    private static string _keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME")!;

    private static string _kvUri = "https://" + _keyVaultName + ".vault.azure.net";

    private static SecretClient _client = new SecretClient(new Uri(_kvUri), new DefaultAzureCredential());

    private static string? _databaseSecretName = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

    private static string? _azureBlobStorageSecretName = Environment.GetEnvironmentVariable("AZURE_STORAGE_NAME");


    public static async Task<string> GetDatabaseConnectionString()
    {
        var databaseConnectionString = await _client.GetSecretAsync(_databaseSecretName);

        return databaseConnectionString.Value.Value;
    }

    public static async Task<string> GetAzureBlobStorageConnectionString()
    {
        var azureConnectionString = await _client.GetSecretAsync(_azureBlobStorageSecretName);

        return azureConnectionString.Value.Value;
    }
}
