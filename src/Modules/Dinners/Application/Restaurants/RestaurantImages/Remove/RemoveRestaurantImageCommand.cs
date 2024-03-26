using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.RestaurantImages.Remove;

internal sealed record RemoveRestaurantImageCommand(Guid Id, string RestaurantImageUrl) : ICommand<ErrorOr<Unit>>;
