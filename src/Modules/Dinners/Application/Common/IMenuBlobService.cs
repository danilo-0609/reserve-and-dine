using BuildingBlocks.Application;

namespace Dinners.Application.Common;

public interface IMenuBlobService
{
    public Task<BlobObject?> GetBlobAsync(string name);

    public Task<string> UploadFileBlobAsync(string filePath, string fileName);

    public Task DeleteBlobAsync(string name);
}