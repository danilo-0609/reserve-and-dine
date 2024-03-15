using Azure.Storage.Blobs;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, string azureBlobStorageConnectionString)
    {
        services.AddSingleton(x => new BlobServiceClient(azureBlobStorageConnectionString));

        return services;
    }
}
