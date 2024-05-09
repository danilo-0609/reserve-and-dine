using BuildingBlocks.Application;

namespace Users.Application.Common;

public interface IUserBlobService
{
    public Task<BlobObject?> GetBlobAsync(string name);

    public Task<string> UploadFileBlobAsync(string filePath, string fileName);

    public Task DeleteBlobAsync(string name);
}
