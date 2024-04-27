using Dinners.Application.Restaurants.Post;
using Dinners.Application.Restaurants.Post.Requests;
using Dinners.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Dinners.Tests.IntegrationTests.Restaurants.Post;

public sealed class PostRestaurantIntegrationTests : BaseIntegrationTest
{
    public PostRestaurantIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async void Post_Should_AddARestaurantToTheDatabase_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var restaurantInformation = new RestaurantInformationRequest(restaurant.RestaurantInformation.Title,
            restaurant.RestaurantInformation.Description,
            restaurant.RestaurantInformation.Type);

        var restaurantSchedules = restaurant.RestaurantSchedules.ConvertAll(r =>
        {
            return new RestaurantScheduleRequest(r.Day.DayOfWeek, r.HoursOfOperation.Start, r.HoursOfOperation.End);
        });

        var restaurantLocalization = new RestaurantLocalizationRequest(restaurant.RestaurantLocalization.Country,
            restaurant.RestaurantLocalization.City,
            restaurant.RestaurantLocalization.Region,
            restaurant.RestaurantLocalization.Neighborhood,
            restaurant.RestaurantLocalization.Addresss,
            restaurant.RestaurantLocalization.LocalizationDetails);

        var restaurantTablesRequest = restaurant.RestaurantTables.ConvertAll(r =>
        {
            return new RestaurantTableRequest(r.Number, r.Seats, r.IsPremium, r.Price.Amount, r.Price.Currency);
        });

        var restaurantAdministrationsRequest = restaurant.RestaurantAdministrations.ConvertAll(r =>
        {
            return new RestaurantAdministrationRequest(r.Name, r.AdministratorId, r.AdministratorTitle);
        });

        var restaurantContact = new RestaurantContactRequest(restaurant.RestaurantContact.Email,
                restaurant.RestaurantContact.Whatsapp,
                restaurant.RestaurantContact.Facebook,
                restaurant.RestaurantContact.PhoneNumber,
                restaurant.RestaurantContact.Instagram,
                restaurant.RestaurantContact.Twitter,
                restaurant.RestaurantContact.TikTok,
                restaurant.RestaurantContact.Website);

        var command = new PostRestaurantCommand(restaurantInformation, 
            restaurantLocalization,
            restaurantSchedules,
            restaurantTablesRequest,
            restaurantAdministrationsRequest,
            restaurantContact,
            restaurant.Chefs.ConvertAll(r => r.Value),
            restaurant.Specialities.ConvertAll(r => r.Value));
    
        var result = await Sender.Send(command);

        var getRestaurant = await DbContext.Restaurants.Where(r => r.Id == restaurant.Id).SingleOrDefaultAsync();

        Assert.NotNull(getRestaurant);
    }
}
