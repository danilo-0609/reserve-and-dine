using BuildingBlocks.Application;
using Dinners.Application.Restaurants.Open;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Dinners.Tests.IntegrationTests.Restaurants.Open;

public sealed class OpenRestaurantIntegrationTests : BaseIntegrationTest
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessorMock;

    public OpenRestaurantIntegrationTests(IntegrationTestWebAppFactory app)
        : base(app)
    {
        _restaurantRepository = new RestaurantRepository(DbContext);
        _executionContextAccessorMock = Substitute.For<IExecutionContextAccessor>();
    }

    [Fact]
    public async void Open_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var command = new OpenRestaurantCommand(Guid.NewGuid());

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new OpenRestaurantCommandHandler(_restaurantRepository, _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRestaurantDoesNotExist = result
            .FirstError
            .Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantDoesNotExist);
    }

    [Fact]
    public async void Open_Should_ReturnAnError_WhenUserIsNotAnAdministrator()
    {
        var restaurantId = RestaurantId.CreateUnique();

        var restaurant = new RestaurantTests().CreateRestaurant(restaurantId);

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var command = new OpenRestaurantCommand(restaurantId.Value);

        var handler = new OpenRestaurantCommandHandler(_restaurantRepository, _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotChangeRestaurantProperties = result
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public async void Open_Should_ReturnAnError_WhenRestaurantScheduleIsOpened()
    {
        var restaurantId = RestaurantId.CreateUnique();

        var restaurant = new RestaurantTests().CreateRestaurant(restaurantId);

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        _executionContextAccessorMock.UserId.Returns(restaurantId.Value);

        var command = new OpenRestaurantCommand(restaurantId.Value);

        var handler = new OpenRestaurantCommandHandler(_restaurantRepository, _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotOpenTheRestaurant = result
            .FirstError
            .Code == "Restaurant.CannotOpenTheRestaurant";

        Assert.True(isErrorCannotOpenTheRestaurant);
    }

    [Fact]
    public async void Open_Should_TurnRestaurantScheduleStatusToOpened_WhenSuccessful()
    {
        var restaurantId = RestaurantId.CreateUnique();

        var restaurant = new RestaurantTests().CreateRestaurant(restaurantId);

        restaurant.Close(restaurant.RestaurantAdministrations.First().AdministratorId);

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        _executionContextAccessorMock.UserId.Returns(restaurantId.Value);

        var command = new OpenRestaurantCommand(restaurantId.Value);

        var handler = new OpenRestaurantCommandHandler(_restaurantRepository, _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        var getRestaurant = await DbContext
            .Restaurants
            .Where(r => r.Id == restaurantId)
            .SingleOrDefaultAsync();
        
        bool scheduleStatusIsOpened = getRestaurant!.RestaurantScheduleStatus == RestaurantScheduleStatus.Open;

        Assert.True(scheduleStatusIsOpened);
    }
}
