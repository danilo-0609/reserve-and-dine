using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;

namespace Dinners.Application.Restaurants.GetById;

internal sealed class GetRestaurantByIdQueryHandler : IQueryHandler<GetRestaurantByIdQuery, ErrorOr<RestaurantResponse>>
{
    private readonly IRestaurantRepository _restaurantRepository;

    public GetRestaurantByIdQueryHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task<ErrorOr<RestaurantResponse>> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId)); 
    
        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        var restaurantInformationResponse = new RestaurantInformationResponse(restaurant.RestaurantInformation.Title,
            restaurant.RestaurantInformation.Description,
            restaurant.RestaurantInformation.Type,
            restaurant.RestaurantInformation.Chefs.ToList().ConvertAll(chef => chef.Value),
            restaurant.RestaurantInformation.Specialties.ToList().ConvertAll(speciality => speciality.Value));

        var restaurantLocalizationResponse = new RestaurantLocalizationResponse(restaurant.RestaurantLocalization.Country,
            restaurant.RestaurantLocalization.City,
            restaurant.RestaurantLocalization.Region,
            restaurant.RestaurantLocalization.Neighborhood,
            restaurant.RestaurantLocalization.Addresss,
            restaurant.RestaurantLocalization.LocalizationDetails);

        var restaurantSchedulesResponse = restaurant.RestaurantSchedules.ToList().ConvertAll(schedule =>
        {
            return new RestaurantScheduleResponse(schedule.Day.DayOfWeek,
                schedule.HoursOfOperation.Start,
                schedule.HoursOfOperation.End,
                schedule.ReopeningTime);
        });

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

        RestaurantResponse restaurantResponse = new RestaurantResponse(restaurant.Id.Value,
            restaurant.AvailableTablesStatus.Value,
            restaurantInformationResponse,
            restaurantLocalizationResponse,
            restaurant.RestaurantScheduleStatus.Value,
            restaurantSchedulesResponse,
            restaurantContactResponse,
            restaurantClientsResponse,
            restaurantTablesResponse,
            restaurant.PostedAt);

        return restaurantResponse;
    }
}
