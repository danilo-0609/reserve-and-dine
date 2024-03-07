using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantInformations;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using Dinners.Domain.Restaurants.RestaurantTables;
using Dinners.Domain.Restaurants.RestaurantUsers;
using Domain.Restaurants;
using ErrorOr;

namespace Dinners.Application.Restaurants.Post;

internal sealed class PostRestaurantCommandHandler : ICommandHandler<PostRestaurantCommand, ErrorOr<Guid>>
{
    private readonly IRestaurantRepository _restaurantRepository;

    public PostRestaurantCommandHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(PostRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurantInformation = RestaurantInformation.Create(request.RestaurantInformation.Title,
            request.RestaurantInformation.Description,
            request.RestaurantInformation.Type,
            request.RestaurantInformation.Chefs,
            request.RestaurantInformation.Specialties,
            request.RestaurantInformation.ImagesUrl);

        var restaurantLocalization = RestaurantLocalization.Create(request.RestaurantLocalization.Country,
            request.RestaurantLocalization.City,
            request.RestaurantLocalization.Region,
            request.RestaurantLocalization.Neighborhood,
            request.RestaurantLocalization.Addresss,
            request.RestaurantLocalization.LocalizationDetails);

        var restaurantSchedule = RestaurantSchedule.Create(request.RestaurantSchedule.Days,
            request.RestaurantSchedule.Start,
            request.RestaurantSchedule.End);

        var restaurantContact = RestaurantContact.Create(request.RestaurantContact.Email,
            request.RestaurantContact.Whatsapp,
            request.RestaurantContact.Facebook,
            request.RestaurantContact.PhoneNumber,
            request.RestaurantContact.Instagram,
            request.RestaurantContact.Twitter,
            request.RestaurantContact.TikTok,
            request.RestaurantContact.Website);

        var restaurantTables = request.RestaurantTables.ConvertAll(table =>
        {
            return RestaurantTable.Create(table.Number, 
                table.Seats, 
                table.IsPremium, 
                table.ReservedHours);
        });

        var restaurantAdministrations = request.RestaurantAdministrations.ConvertAll(admin =>
        {
        return RestaurantAdministration.Create(admin.Name, 
            admin.AdministratorId, 
            admin.AdministratorTitle);
        });

        Restaurant restaurant = Restaurant.Post(restaurantInformation,
            restaurantLocalization,
            restaurantSchedule,
            restaurantContact,
            restaurantTables,
            restaurantAdministrations,
            DateTime.UtcNow);   
    
        await _restaurantRepository.AddAsync(restaurant, cancellationToken);

        return restaurant.Id.Value;
    }
}
