using Azure.Storage.Blobs;
using BuildingBlocks.Application;
using Users.Application.Abstractions;

namespace Users.Infrastructure.Blob;

public sealed class UserBlobService : IUserBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "users";

    public UserBlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task DeleteBlobAsync(string name)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        await containerClient.DeleteBlobIfExistsAsync(name);
    }

    public async Task<BlobObject?> GetBlobAsync(string name)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

            var client = containerClient.GetBlobClient(name);

            if (!await client.ExistsAsync())
            {
                return null;
            }

            var content = await client.DownloadAsync();

            var blobProperties = await client.GetPropertiesAsync();
            string contentType = blobProperties.Value.ContentType;

            MemoryStream memoryStream = new MemoryStream();
            await content.Value.Content.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new BlobObject(memoryStream, contentType);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> UploadFileBlobAsync(string filePath, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(filePath, true);

        return blobClient.Uri.ToString();
    }
}
