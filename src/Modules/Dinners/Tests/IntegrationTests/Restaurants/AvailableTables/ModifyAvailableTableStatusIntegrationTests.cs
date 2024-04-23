using BuildingBlocks.Application;
using Dinners.Application.Restaurants.AvailableTables;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantTables;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System.ComponentModel.DataAnnotations;

namespace Dinners.Tests.IntegrationTests.Restaurants.AvailableTables;

public sealed class ModifyAvailableTableStatusIntegrationTests : BaseIntegrationTest
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessorMock;

    public ModifyAvailableTableStatusIntegrationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _restaurantRepository = new RestaurantRepository(DbContext);
        _executionContextAccessorMock = Substitute.For<IExecutionContextAccessor>();
    }

    [Fact]
    public async void ModifyAvailableTableStatus_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var command = new ModifyAvailableTableStatusCommand(RestaurantId: Guid.NewGuid(), "Few");

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new ModifyAvailableTableStatusCommandHandler(
            _restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRestaurantDoesNotExist = result
            .FirstError
            .Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantDoesNotExist);
    }

    [Fact]
    public async void ModifyAvailableTableStatus_Should_ReturnAnError_WhenUserRequesterIsNotARestaurantAdministrator()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new ModifyAvailableTableStatusCommand(
            RestaurantId: restaurant.Id.Value, 
            "Few");

        //Not a Restaurant administrator
        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new ModifyAvailableTableStatusCommandHandler(
            _restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotChangeRestaurantProperties = result
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public async void ModifyAvailableTableStatus_Should_ReturnAnError_WhenAvailableTableStatusRequestedIsNotAnActualAvailableTableStatus()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new ModifyAvailableTableStatusCommand(
            RestaurantId: restaurant.Id.Value,
            AvailableTableStatus: "AvailableTableStatus"); //It should be Availables, Few or NoAvailables

        var result = await Sender.Send(command);

        bool isValidationError = result
            .FirstError
            .Description == "Available table status must be NoAvailables, Few or Availables";

        Assert.True(isValidationError);
    }

    [Fact]
    public async void ModifyAvailableTableStatus_Should_ReturnAnError_WhenAvailableTableStatusIsEqualToAvailableTableStatusRequested()
    {
        //AvailableTablesStatus = Availables;
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new ModifyAvailableTableStatusCommand(
            RestaurantId: restaurant.Id.Value,
            "Availables");

        _executionContextAccessorMock.UserId.Returns(restaurant
            .RestaurantAdministrations
            .First()
            .AdministratorId);

        var handler = new ModifyAvailableTableStatusCommandHandler(
            _restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorEqualAvailableTableStatus = result
            .FirstError
            .Code == "Restaurant.EqualAvailableTableStatus";

        Assert.True(isErrorEqualAvailableTableStatus);
    }

    [Fact]
    public async void ModifyAvailableTableStatus_Should_TurnAvailableTablesStatusToTheRequestedStatus_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new ModifyAvailableTableStatusCommand(
            RestaurantId: restaurant.Id.Value,
            "Few");

        _executionContextAccessorMock.UserId.Returns(restaurant
            .RestaurantAdministrations
            .First()
            .AdministratorId);

        var handler = new ModifyAvailableTableStatusCommandHandler(
            _restaurantRepository,
            _executionContextAccessorMock);

        await handler.Handle(command, CancellationToken.None);

        var getRestaurant = await DbContext
            .Restaurants
            .Where(r => r.Id == restaurant.Id)
            .SingleAsync();

        bool isAvailableTableStatusFew = getRestaurant
                .AvailableTablesStatus == AvailableTablesStatus.Few;

        Assert.True(isAvailableTableStatusFew);
    }
}
