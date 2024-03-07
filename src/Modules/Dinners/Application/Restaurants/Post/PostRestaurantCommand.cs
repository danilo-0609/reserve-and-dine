using Dinners.Application.Common;
using Dinners.Application.Restaurants.Post.Requests;
using ErrorOr;

namespace Dinners.Application.Restaurants.Post;

public sealed record PostRestaurantCommand(RestaurantInformationRequest RestaurantInformation,
    RestaurantLocalizationRequest RestaurantLocalization,
    RestaurantScheduleRequest RestaurantSchedule,
    List<RestaurantTableRequest> RestaurantTables,
    List<RestaurantAdministrationRequest> RestaurantAdministrations,
    RestaurantContactRequest RestaurantContact) : ICommand<ErrorOr<Guid>>;
