using BuildingBlocks.Application;
using Dinners.Application.Restaurants.Rate.Delete;
using Dinners.Application.Restaurants.Rate.Publish;
using Dinners.Application.Restaurants.Rate.Upgrade;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Infrastructure.Domain.Restaurants.Ratings;
using Dinners.Tests.UnitTests.Restaurants;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Dinners.Tests.IntegrationTests.Restaurants.Rate;

public sealed class RateIntegrationTests : BaseIntegrationTest
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IExecutionContextAccessor _executionContextAccessorMock;

    public RateIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _restaurantRepository = new RestaurantRepository(DbContext);
        _restaurantRatingRepository = new RatingRepository(DbContext);

        _executionContextAccessorMock = Substitute.For<IExecutionContextAccessor>();
    }

    [Fact]
    public async void RateRestaurant_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var command = new RateRestaurantCommand(RestaurantId: Guid.NewGuid(),
            Stars: 4,
            Comment: "I liked so much the food!");

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new RateRestaurantCommandHandler(
            _restaurantRepository, 
            _restaurantRatingRepository, 
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRestaurantDoesNotExist = result
            .FirstError
            .Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantDoesNotExist);
    }

    [Fact]
    public async void DeleteRate_Should_ReturnAnError_WhenRateDoesNotExist()
    {
        var command = new DeleteRateCommand(RatingId: Guid.NewGuid());

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new DeleteRateCommandHandler(
            _restaurantRatingRepository, 
            _executionContextAccessorMock); 
        
        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRatingWasNotFound = result
            .FirstError
            .Code == "RestaurantRating.NotFound";

        Assert.True(isErrorRatingWasNotFound);
    }

    [Fact]
    public async void UpgradeRate_Should_ReturnAnError_WhenRateDoesNotExist()
    {
        var command = new UpgradeRateCommand(RateId: Guid.NewGuid(),
            3);

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new UpgradeRateCommandHandler(
            _restaurantRatingRepository, 
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRatingWasNotFound = result
            .FirstError
            .Code == "RestaurantRating.NotFound";

        Assert.True(isErrorRatingWasNotFound);
    }

    [Fact]
    public async void RateRestaurant_Should_ReturnAnError_WhenUserRatingTheRestaurantIsARestaurantAdministrator()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new RateRestaurantCommand(RestaurantId: restaurant.Id.Value,
            Stars: 4,
            Comment: "I liked so much the food!");

        _executionContextAccessorMock.UserId.Returns(restaurant
            .RestaurantAdministrations
            .First()
            .AdministratorId); //Administrator trying to rate restaurant

        var handler = new RateRestaurantCommandHandler(
            _restaurantRepository,
            _restaurantRatingRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRateWhenUserIsAdministrator = result
            .FirstError
            .Code == "Restaurant.RateWhenUserIsAdministrator";

        Assert.True(isErrorRateWhenUserIsAdministrator);
    }

    [Fact]
    public async void RateRestaurant_Should_ReturnAnError_WhenUserRatingHasNotVisitedTheRestaurant()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new RateRestaurantCommand(RestaurantId: restaurant.Id.Value,
            Stars: 4,
            Comment: "I liked so much the food!");

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid()); //User who has not visited the restaurant

        var handler = new RateRestaurantCommandHandler(
            _restaurantRepository,
            _restaurantRatingRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotRateWhenClientHasNotVisitedTheRestaurant = result
            .FirstError
            .Code == "RestaurantRatings.CannotRateWhenClientHasNotVisitedTheRestaurant";

        Assert.True(isErrorCannotRateWhenClientHasNotVisitedTheRestaurant);
    }

    [Fact]
    public async void RateRestaurant_Should_AddANewRatingInDatabase_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var clientId = Guid.NewGuid();

        restaurant.AddRestaurantClient(clientId); //Now it's a restaurant client

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new RateRestaurantCommand(RestaurantId: restaurant.Id.Value,
            Stars: 4,
            Comment: "I liked so much the food!");

        _executionContextAccessorMock.UserId.Returns(clientId); 

        var handler = new RateRestaurantCommandHandler(
            _restaurantRepository,
            _restaurantRatingRepository,
            _executionContextAccessorMock);

        var rateId = await handler.Handle(command, CancellationToken.None);

        var rate = await DbContext
            .Ratings
            .Where(r => r.Id == RestaurantRatingId.Create(rateId.Value))
            .SingleOrDefaultAsync();

        Assert.NotNull(rate);
    }

    [Fact]
    public async void DeleteRate_Should_ReturnAnError_WhenUserDeletingRateIsNotTheSameWhoPostedTheRating()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var clientId = Guid.NewGuid();

        restaurant.AddRestaurantClient(clientId);

        var rate = restaurant.Rate(4, clientId, "I liked so much the food");

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new DeleteRateCommand(rate.Value.Id.Value);

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid()); //Another user

        var handler = new DeleteRateCommandHandler
            (_restaurantRatingRepository, 
            _executionContextAccessorMock);
    
        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotDeleteRatingIfUserHasNotPostedThatRating = result
            .FirstError
            .Code == "RestaurantRatings.CannotDeleteRatingIfUserHasNotPostedThatRating";

        Assert.True(isErrorCannotDeleteRatingIfUserHasNotPostedThatRating);
    }

    [Fact]
    public async void UpgradeRate_Should_ReturnAnError_WhenUpdatingRateIsNotTheSameWhoPostedTheRating()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var clientId = Guid.NewGuid();

        restaurant.AddRestaurantClient(clientId);

        var rate = restaurant.Rate(4, clientId, "I liked so much the food");

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new UpgradeRateCommand(rate.Value.Id.Value, 5);

        var handler = new UpgradeRateCommandHandler(
            _restaurantRatingRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotUpdateRateWhenIsNotUserRater = result
            .FirstError
            .Code == "RestaurantRatings.CannotUpdateRateWhenIsNotUserRater";

        Assert.True(isErrorCannotUpdateRateWhenIsNotUserRater);
    }

    [Fact]
    public async void DeleteRate_Should_RemoveRatingFromDatabase_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var clientId = Guid.NewGuid();

        restaurant.AddRestaurantClient(clientId);

        var rate = restaurant.Rate(4, clientId, "I liked so much the food");

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new DeleteRateCommand(rate.Value.Id.Value);

        _executionContextAccessorMock.UserId.Returns(clientId); 

        var handler = new DeleteRateCommandHandler
            (_restaurantRatingRepository,
            _executionContextAccessorMock);

        await handler.Handle(command, CancellationToken.None);

        bool isRateDeleted = await DbContext.Ratings.AnyAsync(r => r.Id == rate.Value.Id);

        Assert.False(isRateDeleted);
    }
}
