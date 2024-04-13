using BuildingBlocks.Application;
using Dinners.Application.Restaurants.Administration.Add;
using Dinners.Domain.Restaurants;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Common.ExtensionFramework;
using NSubstitute;

namespace Dinners.Tests.IntegrationTests.Restaurants.Administration.Add;

public sealed class AddAdministrationIntegrationTests : BaseIntegrationTest
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessorMock;

    public AddAdministrationIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _restaurantRepository = new RestaurantRepository(DbContext);

        _executionContextAccessorMock = Substitute.For<IExecutionContextAccessor>();
    }

    [Fact]
    public async void AddAdministration_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var command = new AddAdministrationCommand(Guid.NewGuid(), "Restaurant name", "Administrator title", Guid.NewGuid());

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new AddAdministrationCommandHandler(_restaurantRepository, _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRestaurantDoesNotExist = result
            .FirstError
            .Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantDoesNotExist);
    }

    [Fact]
    public async void AddAdministration_Should_ReturnAnError_WhenUserIsNotAdministrator()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new AddAdministrationCommand(restaurant.Id.Value,
            "Restaurant name",
            "Administrator title",
            Guid.NewGuid());

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new AddAdministrationCommandHandler(_restaurantRepository, _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotChangeRestaurantProperties = result
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public async void AddAdministration_Should_ReturnAnError_WhenRestaurantAdministratorCurrentlyExists()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new AddAdministrationCommand(restaurant.Id.Value,
            "Restaurant name",
            "Administrator title",
            restaurant.RestaurantAdministrations.First().AdministratorId);

        _executionContextAccessorMock.UserId.Returns(restaurant
            .RestaurantAdministrations
            .First()
            .AdministratorId);

        var handler = new AddAdministrationCommandHandler(_restaurantRepository, _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorAdministratorExists = result
            .FirstError
            .Code == "RestaurantAdministration.AdministratorExists";

        Assert.True(isErrorAdministratorExists);
    }

    [Fact]
    public async void AddAdministration_Should_AddAdministratorToDatabase_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var newAdministrator = Guid.NewGuid();

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new AddAdministrationCommand(restaurant.Id.Value,
            "Restaurant name",
            "Administrator title",
            newAdministrator);

        _executionContextAccessorMock.UserId.Returns(restaurant
            .RestaurantAdministrations
            .First()
            .AdministratorId);

        var handler = new AddAdministrationCommandHandler(_restaurantRepository, _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        var getRestaurant = await DbContext
            .Restaurants
            .Where(r => r.Id == restaurant.Id)
            .SingleOrDefaultAsync();

        bool isAdminAdded = getRestaurant!
            .RestaurantAdministrations
            .Any(r => r.AdministratorId == newAdministrator);

        Assert.True(isAdminAdded);
    }
}
