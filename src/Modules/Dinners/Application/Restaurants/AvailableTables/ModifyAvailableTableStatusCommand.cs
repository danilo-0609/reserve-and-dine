using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.AvailableTables;

public sealed record ModifyAvailableTableStatusCommand(Guid RestaurantId,
    string AvailableTableStatus) : ICommand<ErrorOr<Unit>>;
