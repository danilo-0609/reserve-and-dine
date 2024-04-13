using BuildingBlocks.Application;
using Dinners.Application.Restaurants.Administration.Delete;
using Dinners.Domain.Restaurants;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Dinners.Tests.IntegrationTests.Restaurants.Administration.Delete;

public sealed class DeleteAdministratorIntegrationTests : BaseIntegrationTest
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessorMock;

    public DeleteAdministratorIntegrationTests(IntegrationTestWebAppFactory factory) 
        : base(factory)
    {
        _restaurantRepository = new RestaurantRepository(DbContext);
    
        _executionContextAccessorMock = Substitute.For<IExecutionContextAccessor>();
    }

    [Fact]
    public async void DeleteAdministration_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var command = new DeleteAdministrationCommand(RestaurantId: Guid.NewGuid(), 
            AdministratorId: Guid.NewGuid());

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new DeleteAdministrationCommandHandler(_restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRestaurantDoesNotExist = result
            .FirstError
            .Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantDoesNotExist);
    }

    [Fact]
    public async void DeleteAdministration_Should_ReturnAnError_WhenUserRequesterIsNotAnAdministrator()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new DeleteAdministrationCommand(RestaurantId: restaurant.Id.Value,
        AdministratorId: Guid.NewGuid());

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid()); //User requester

        var handler = new DeleteAdministrationCommandHandler(_restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotChangeRestaurantProperties = result
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public async void DeleteAdministration_Should_ReturnAnError_WhenUserToDeleteDoesNotExist()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new DeleteAdministrationCommand(RestaurantId: restaurant.Id.Value,
        AdministratorId: Guid.NewGuid());

        _executionContextAccessorMock.UserId.Returns(restaurant
            .RestaurantAdministrations
            .First()
            .AdministratorId); //User requester

        var handler = new DeleteAdministrationCommandHandler(_restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRestaurantAdministrationNotFound = result
            .FirstError
            .Code == "RestaurantAdministration.NotFound";

        Assert.True(isErrorRestaurantAdministrationNotFound);
    }

    [Fact]
    public async void DeleteAdministration_Should_RemoveAdministrator_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var administratorId = restaurant.RestaurantAdministrations.First().AdministratorId;

        var command = new DeleteAdministrationCommand(RestaurantId: restaurant.Id.Value,
        AdministratorId: administratorId);

        _executionContextAccessorMock.UserId.Returns(restaurant
            .RestaurantAdministrations
            .Where(r => r.AdministratorId != administratorId)
            .First()
            .AdministratorId); //User requester

        var handler = new DeleteAdministrationCommandHandler(_restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        var getRestaurant = await DbContext
            .Restaurants
            .Where(r => r.Id == restaurant.Id)
            .SingleOrDefaultAsync();
    
        bool isAdminInRestaurant = getRestaurant!
            .RestaurantAdministrations
            .Any(r => r.AdministratorId == administratorId);

        Assert.False(isAdminInRestaurant);
    }
}

