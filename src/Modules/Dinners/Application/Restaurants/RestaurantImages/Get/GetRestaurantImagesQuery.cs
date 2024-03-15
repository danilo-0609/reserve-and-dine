using BuildingBlocks.Application;
using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.RestaurantImages.Get;

internal sealed record GetRestaurantImagesQuery(Guid Id) : IQuery<ErrorOr<List<BlobObject>>>;