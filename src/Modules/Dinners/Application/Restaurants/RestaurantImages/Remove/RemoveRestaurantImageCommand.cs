using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.RestaurantImages.Remove;

public sealed record RemoveRestaurantImageCommand(Guid Id, 
    Guid imageId) : ICommand<ErrorOr<Unit>>;
