using Dinners.Application.Restaurants.Post.Requests;

namespace API.Modules.Dinners.Requests.Restaurants;

public sealed record PostRestaurantRequest(RestaurantInformationRequest RestaurantInformation,
    RestaurantLocalizationRequest RestaurantLocalization,
    List<RestaurantScheduleRequest> RestaurantSchedules,
    List<RestaurantTableRequest> RestaurantTables,
    List<RestaurantAdministrationRequest> RestaurantAdministrations,
    RestaurantContactRequest RestaurantContact,
    List<string> Chefs,
    List<string> Specialties);
