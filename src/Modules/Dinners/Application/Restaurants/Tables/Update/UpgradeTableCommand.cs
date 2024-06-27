using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Tables.Update;

public sealed record UpgradeTableCommand(Guid RestaurantId,
    int Number,
    int Seats,
    bool IsPremium) : ICommand<ErrorOr<Success>>;
