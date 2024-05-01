using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.ChangeLocalization;

public sealed record ChangeRestaurantLocalizationCommand(Guid RestaurantId,
    string Country,
    string City,
    string Region,
    string Neighborhood,
    string Address,
    string LocalizationDetails = "") : ICommand<ErrorOr<Unit>>;
