using BuildingBlocks.Application;

namespace Dinners.Application.Common;

public interface IRestaurantBlobService
{
    public Task<BlobObject?> GetBlobAsync(string name);

    public Task<Uri> UploadFileBlobAsync(string filePath, string fileName);

    public Task DeleteBlobAsync(string name);
}
