using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Tables.Delete;

public sealed record DeleteTableCommand(Guid RestaurantId, int Number) : ICommand<ErrorOr<Unit>>;
