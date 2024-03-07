using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Delete;

public sealed record DeleteRestaurantCommand(Guid RestaurantId) : ICommand<ErrorOr<Unit>>;
