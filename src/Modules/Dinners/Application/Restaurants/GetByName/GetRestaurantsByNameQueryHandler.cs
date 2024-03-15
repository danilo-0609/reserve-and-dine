using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;

namespace Dinners.Application.Restaurants.GetByName;

internal sealed class GetRestaurantsByNameQueryHandler : IQueryHandler<GetRestaurantsByNameQuery, ErrorOr<IReadOnlyList<RestaurantResponse>>>
{
    private readonly IRestaurantRepository _restaurantRepository;

    public GetRestaurantsByNameQueryHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<RestaurantResponse>>> Handle(GetRestaurantsByNameQuery request, CancellationToken cancellationToken)
    {
        List<Restaurant> restaurants = await _restaurantRepository.GetRestaurantsByNameAsync(request.Name, cancellationToken);
    
        if (!restaurants.Any())
        {
            return RestaurantErrorCodes.NotFound;
        }

        var restaurantResponses = restaurants.ConvertAll(restaurant =>
        {

            var restaurantInformationResponse = new RestaurantInformationResponse(restaurant.RestaurantInformation.Title,
                restaurant.RestaurantInformation.Description,
                restaurant.RestaurantInformation.Type,
                restaurant.RestaurantInformation.Chefs,
                restaurant.RestaurantInformation.Specialties);

            var restaurantLocalizationResponse = new RestaurantLocalizationResponse(restaurant.RestaurantLocalization.Country,
                restaurant.RestaurantLocalization.City,
                restaurant.RestaurantLocalization.Region,
                restaurant.RestaurantLocalization.Neighborhood,
                restaurant.RestaurantLocalization.Addresss,
                restaurant.RestaurantLocalization.LocalizationDetails);

            var restaurantScheduleResponse = new RestaurantScheduleResponse(restaurant.RestaurantSchedule.Days,
                restaurant.RestaurantSchedule.HoursOfOperation.Start,
                restaurant.RestaurantSchedule.HoursOfOperation.End);

            var restaurantContactResponse = new RestaurantContactResponse(restaurant.RestaurantContact.Email,
                restaurant.RestaurantContact.Whatsapp,
                restaurant.RestaurantContact.Facebook,
                restaurant.RestaurantContact.PhoneNumber,
                restaurant.RestaurantContact.Instagram,
                restaurant.RestaurantContact.Twitter,
                restaurant.RestaurantContact.TikTok,
                restaurant.RestaurantContact.Website);

            var restaurantClientsResponse = restaurant
                .RestaurantClients
                .ToList().ConvertAll(restaurantClient =>
                {
                    return new RestaurantClientResponse(restaurantClient.ClientId,
                    restaurantClient.NumberOfVisits);
                });

            var restaurantTablesResponse = restaurant
                .RestaurantTables
                .ToList()
                .ConvertAll(table =>
                {
                    return new RestaurantTableResponse(table.Number,
                        table.Seats,
                        table.IsPremium,
                        table.IsPremium,
                        table.ReservedHours);
                });

            return new RestaurantResponse(restaurant.Id.Value,
                restaurant.AvailableTablesStatus.Value,
                restaurantInformationResponse,
                restaurantLocalizationResponse,
                restaurant.RestaurantScheduleStatus.Value,
                restaurantScheduleResponse,
                restaurantContactResponse,
                restaurantClientsResponse,
                restaurantTablesResponse,
                restaurant.PostedAt);
        });

        return restaurantResponses.AsReadOnly();
    }
}
