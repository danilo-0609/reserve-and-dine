using BuildingBlocks.Application;
using Dinners.Application.Restaurants.Administration.Update;
using Dinners.Domain.Restaurants;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using NSubstitute;

namespace Dinners.Tests.IntegrationTests.Restaurants.Administration.Update;

public sealed class UpdateIntegrationTests : BaseIntegrationTest
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessorMock;

    public UpdateIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _restaurantRepository = new RestaurantRepository(DbContext);
    
        _executionContextAccessorMock = Substitute.For<IExecutionContextAccessor>();    
    }

    [Fact]
    public async void UpdateAdministration_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var command = new UpdateAdministrationCommand(RestaurantId: Guid.NewGuid(),
            "Admin name",
            "Admin title",
            Guid.NewGuid());

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new UpdateAdministrationCommandHandler(_restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRestaurantDoesNotExist = result
            .FirstError
            .Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantDoesNotExist);
    }

    [Fact]
    public async void UpdateAdministration_Should_ReturnAnError_WhenUserRequesterIsNotAnAdministrator()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new UpdateAdministrationCommand(RestaurantId: restaurant.Id.Value,
            "Admin name",
            "Admin title",
            Guid.NewGuid());

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid()); //User requester

        var handler = new UpdateAdministrationCommandHandler(_restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotChangeRestaurantProperties = result
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public async void UpdateAdministration_Should_ReturnAnError_WhenUserToDeleteDoesNotExist()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new UpdateAdministrationCommand(RestaurantId: restaurant.Id.Value,
            "Admin name",
            "Admin title",
            AdminId: Guid.NewGuid());

        _executionContextAccessorMock.UserId.Returns(restaurant
            .RestaurantAdministrations
            .First()
            .AdministratorId);

        var handler = new UpdateAdministrationCommandHandler(_restaurantRepository,
            _executionContextAccessorMock);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorRestaurantAdministrationNotFound = result
            .FirstError
            .Code == "RestaurantAdministration.NotFound";

        Assert.True(isErrorRestaurantAdministrationNotFound);
    }
}
