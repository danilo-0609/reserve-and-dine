using BuildingBlocks.Application;
using Dinners.Application.Restaurants.ModifySchedule;
using Dinners.Domain.Restaurants;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Dinners.Tests.IntegrationTests.Restaurants.ModifySchedule;

public sealed class ModifyScheduleIntegrationTests : BaseIntegrationTest
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessorMock;

    public ModifyScheduleIntegrationTests(IntegrationTestWebAppFactory factory) 
        : base(factory)
    {
        _restaurantRepository = new RestaurantRepository(DbContext);
        _executionContextAccessorMock = Substitute.For<IExecutionContextAccessor>();
    }

    [Fact]
    public async void ModifySchedule_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var command = new ModifyRestaurantScheduleCommand(
            RestaurantId: Guid.NewGuid(),
            DayOfWeek.Monday,
            DateTime.Now.TimeOfDay.ToString(),
            DateTime.Now.AddHours(8).TimeOfDay.ToString());

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new ModifyRestaurantScheduleCommandHandler(
            _restaurantRepository,
            _executionContextAccessorMock);
    
        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRestaurantDoesNotExist = result
            .FirstError
            .Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantDoesNotExist);
    }

    [Fact]
    public async void ModifySchedule_Should_ReturnAnError_WhenUserRequesterIsNotAnAdministrator()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());
        
        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new ModifyRestaurantScheduleCommand(
            RestaurantId: restaurant.Id.Value,
            DayOfWeek.Monday,
            DateTime.Now.TimeOfDay.ToString(),
            DateTime.Now.AddHours(8).TimeOfDay.ToString());

        //User is not admin
        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new ModifyRestaurantScheduleCommandHandler(
            _restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotChangeRestaurantProperties = result
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public async void ModifySchedule_Should_UpdateSchedule_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new ModifyRestaurantScheduleCommand(
            RestaurantId: restaurant.Id.Value,
            DayOfWeek.Monday,
            DateTime.Now.TimeOfDay.ToString(),
            DateTime.Now.AddHours(8).TimeOfDay.ToString());

        _executionContextAccessorMock.UserId.Returns(restaurant
            .RestaurantAdministrations
            .First()
            .AdministratorId);

        var handler = new ModifyRestaurantScheduleCommandHandler(
            _restaurantRepository,
            _executionContextAccessorMock);

        await handler.Handle(command, CancellationToken.None);

        var getRestaurant = await DbContext
            .Restaurants
            .Where(r => r.Id == restaurant.Id)
            .SingleAsync();

        bool isScheduleAdded = getRestaurant
            .RestaurantSchedules
            .Any(r => r.Day.DayOfWeek == command.Day &&
                r.HoursOfOperation.Start == TimeSpan.Parse(command.Start) &&
                r.HoursOfOperation.End == TimeSpan.Parse(command.End));

        Assert.True(isScheduleAdded);
    }
}
