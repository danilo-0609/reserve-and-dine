using BuildingBlocks.Application;
using Dinners.Application.Restaurants.Tables.Add;
using Dinners.Domain.Restaurants;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using Docker.DotNet.Models;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using static MassTransit.ValidationResultExtensions;

namespace Dinners.Tests.IntegrationTests.Restaurants.Tables.Add;

public sealed class AddTableIntegrationTests : BaseIntegrationTest
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public AddTableIntegrationTests(IntegrationTestWebAppFactory factory) 
        : base(factory)
    {
        _restaurantRepository = new RestaurantRepository(DbContext);

        _executionContextAccessor = Substitute.For<IExecutionContextAccessor>();
    }

    [Fact]
    public async void AddTable_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {
        var command = new AddTableCommand(RestaurantId: Guid.NewGuid(),
            1,
            5,
            true,
            15.21m,
            "USD");

        var result = await Sender.Send(command);

        bool isResultRestaurantNotFound = result
            .FirstError
            .Code == "Restaurant.NotFound";
    
        Assert.True(isResultRestaurantNotFound);
    }

    [Fact]
    public async void AddTable_Should_ReturnAnError_WhenUserRequesterIsNotAnAdministrator()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new AddTableCommand(RestaurantId: restaurant.Id.Value,
            1,
            5,
            true,
            15.21m,
            "USD");

        _executionContextAccessor.UserId.Returns(Guid.NewGuid());

        var handler = new AddTableCommandHandler(_restaurantRepository, _executionContextAccessor);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isResultCannotChangeRestaurantProperties = result
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isResultCannotChangeRestaurantProperties);
    }

    [Fact]
    public async void AddTable_Should_ReturnAnError_WhenTableNumberDoesExistYet()
    {
        //It has a table number equals to 1
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new AddTableCommand(RestaurantId: restaurant.Id.Value,
            Number: 1,
            5,
            true,
            15.21m,
            "USD");

        _executionContextAccessor.UserId.Returns(restaurant.RestaurantAdministrations.First().AdministratorId);

        var handler = new AddTableCommandHandler(_restaurantRepository, _executionContextAccessor);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isResultCannotAddTableWithDuplicateNumber = result
            .FirstError
            .Code == "Restaurant.CannotAddTableWithDuplicateNumber";

        Assert.True(isResultCannotAddTableWithDuplicateNumber);
    }

    [Fact]
    public async void AddTable_Should_InsertTheNewTableInDatabase_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var numberOfTable = 5;

        var command = new AddTableCommand(RestaurantId: restaurant.Id.Value,
            Number: numberOfTable,
            5,
            true,
            15.21m,
            "USD");

        _executionContextAccessor.UserId.Returns(restaurant.RestaurantAdministrations.First().AdministratorId);

        var handler = new AddTableCommandHandler(_restaurantRepository, _executionContextAccessor);

        await handler.Handle(command, CancellationToken.None);

        var getRestaurant = await DbContext.Restaurants.Where(t => t.Id == restaurant.Id).SingleAsync();

        bool isTableAdded = getRestaurant!
            .RestaurantTables
            .Any(r => r.Number == numberOfTable);

        Assert.True(isTableAdded);
    }
}
