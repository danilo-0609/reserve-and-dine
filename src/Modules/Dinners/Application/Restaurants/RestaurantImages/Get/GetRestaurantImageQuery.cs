using BuildingBlocks.Application;
using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Restaurants.RestaurantImages.Get;

public sealed record GetRestaurantImageQuery(Guid Id, Guid imageId) : IQuery<ErrorOr<BlobObject>>;