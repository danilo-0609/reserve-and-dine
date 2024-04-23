using BuildingBlocks.Application;
using Dinners.Application.Restaurants.Close;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using Domain.Restaurants;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Dinners.Tests.IntegrationTests.Restaurants.Close;

public sealed class CloseRestaurantIntegrationTests : BaseIntegrationTest
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessorMock;

    public CloseRestaurantIntegrationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _restaurantRepository = new RestaurantRepository(DbContext);
        _executionContextAccessorMock = Substitute.For<IExecutionContextAccessor>();    
    }

    [Fact]
    public async void Close_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var command = new CloseRestaurantCommand(RestaurantId: Guid.NewGuid());

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new CloseRestaurantCommandHandler(
            _restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRestaurantDoesNotExist = result
            .FirstError
            .Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantDoesNotExist);
    }

    [Fact]
    public async void Close_Should_ReturnAnError_WhenUserRequesterIsNotAnAdministrator()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());
    
        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new CloseRestaurantCommand(RestaurantId: restaurant.Id.Value);

        //User is not a restaurant admin
        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new CloseRestaurantCommandHandler(
            _restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotChangeRestaurantProperties = result
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public async void Close_Should_ReturnAnError_WhenRestaurantIsClosed()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var adminId = restaurant.RestaurantAdministrations.First().AdministratorId;

        //ScheduleStatus = Closed;
        restaurant.Close(adminId);

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new CloseRestaurantCommand(RestaurantId: restaurant.Id.Value);

        _executionContextAccessorMock.UserId.Returns(adminId);

        var handler = new CloseRestaurantCommandHandler(
            _restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotCloseTheRestaurant = result
            .FirstError
            .Code == "Restaurant.CannotCloseTheRestaurant";

        Assert.True(isErrorCannotCloseTheRestaurant);

    }

    [Fact]
    public async void Close_Should_SetScheduleStatusToClosed_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var adminId = restaurant.RestaurantAdministrations.First().AdministratorId;

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new CloseRestaurantCommand(RestaurantId: restaurant.Id.Value);

        _executionContextAccessorMock.UserId.Returns(adminId);

        var handler = new CloseRestaurantCommandHandler(
            _restaurantRepository,
            _executionContextAccessorMock);

        await handler.Handle(command, CancellationToken.None);

        var getRestaurant = await DbContext
            .Restaurants
            .Where(r => r.Id == restaurant.Id)
            .SingleAsync();

        bool isScheduleStatusClosed = getRestaurant
            .RestaurantScheduleStatus == RestaurantScheduleStatus.Closed;

        Assert.True(isScheduleStatusClosed);
    }
}
