using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Application.Reservations.Request;
using Dinners.Domain.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Menus.Details;
using Dinners.Domain.Menus.Dishes;
using Dinners.Domain.Menus.Schedules;
using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Dinners.Infrastructure;
using Dinners.Infrastructure.Domain.Menus;
using Dinners.Infrastructure.Domain.Reservations;
using Dinners.Infrastructure.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Dinners.Tests.IntegrationTests.Reservations;

public sealed class RequestReservationIntegrationTests : BaseIntegrationTest
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IExecutionContextAccessor _executionContextAccessorMock;
    private readonly IMenuRepository _menuRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RequestReservationIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _restaurantRepository = new RestaurantRepository(DbContext);
        _reservationRepository = new ReservationRepository(DbContext);
        _menuRepository = new MenuRepository(DbContext);

        _executionContextAccessorMock = Substitute.For<IExecutionContextAccessor>();
    }

    [Fact]
    public async void Request_Should_ReturnAnError_WhenRestaurantDoesNotExistInDatabase()
    {
        var command = new RequestReservationCommand(1,
            DateTime.Now.AddHours(10).TimeOfDay.ToString(),
            DateTime.Now.AddHours(10).AddMinutes(45).TimeOfDay.ToString(),
            DateTime.Now.AddHours(10),
            RestaurantId: Guid.NewGuid(),
            "Customer name",
            4,
            new List<Guid>());

        var result = await Sender.Send(command);

        bool isErrorRestaurantDoesNotExist = result.FirstError.Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantDoesNotExist);
    }

    [Fact]
    public async void Request_Should_ReturnAnError_WhenAnyMenuIdDoesNotExistInDatabase()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new RequestReservationCommand(1,
            DateTime.Now.AddHours(10).TimeOfDay.ToString(),
            DateTime.Now.AddHours(10).AddMinutes(45).TimeOfDay.ToString(),
            DateTime.Now.AddHours(10),
            RestaurantId: restaurant.Id.Value,
            "Customer name",
            4,
            new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() });

        _executionContextAccessorMock.UserId.Returns(Guid.NewGuid());

        var handler = new RequestReservationCommandHandler(_reservationRepository,
            _restaurantRepository,
            _executionContextAccessorMock,
            _menuRepository);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorMenuNotFound = result.FirstError.Code == "Menu.NotFound";

        Assert.True(isErrorMenuNotFound);
    }

    [Fact]
    public async void Request_Should_ReturnAnError_WhenRestaurantTableDoesNotExistInRestaurant()
    {
        //Restaurant has three tables: 1, 2, 3
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var menu = Menu.Publish(MenuId.CreateUnique(),
            restaurant.Id,
            MenuDetails.Create("Title",
                "Description",
                MenuType.Lunch,
                new Price(10.23m, "USD"),
                0.0m,
                false,
                "Chef name",
                false),
            DishSpecification.Create(),
            new List<string>(),
            new List<string>(),
            new List<string>(),
            new List<MenuSchedule>(),
            DateTime.UtcNow);

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        var command = new RequestReservationCommand(ReservedTable: 50,
            DateTime.Now.AddHours(10).TimeOfDay.ToString(),
            DateTime.Now.AddHours(10).AddMinutes(45).TimeOfDay.ToString(),
            DateTime.Now.AddHours(10),
            RestaurantId: restaurant.Id.Value,
            "Customer name",
            4,
            new List<Guid>() { menu.Id.Value });

        var handler = new RequestReservationCommandHandler(_reservationRepository,
            _restaurantRepository,
            _executionContextAccessorMock,
            _menuRepository);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorTableDoesNotExist = result.FirstError.Code == "Reservation.TableNotFound";

        Assert.True(isErrorTableDoesNotExist);
    }

    [Fact]
    public async void Request_Should_ReturnAnError_WhenNumberOfAttendeesIsGreaterThanTableSeats()
    {
        //Number of seats on tables aren't greather than 5
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var menu = Menu.Publish(MenuId.CreateUnique(),
            restaurant.Id,
            MenuDetails.Create("Title",
                "Description",
                MenuType.Lunch,
                new Price(10.23m, "USD"),
                0.0m,
                false,
                "Chef name",
                false),
            DishSpecification.Create(),
            new List<string>(),
            new List<string>(),
            new List<string>(),
            new List<MenuSchedule>(),
            DateTime.UtcNow);

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        var command = new RequestReservationCommand(ReservedTable: restaurant.RestaurantTables.First().Number,
            DateTime.Now.AddHours(10).TimeOfDay.ToString(),
            DateTime.Now.AddHours(10).AddMinutes(45).TimeOfDay.ToString(),
            DateTime.Now.AddHours(10),
            RestaurantId: restaurant.Id.Value,
            "Customer name",
            NumberOfAttendees: 50,
            new List<Guid>() { menu.Id.Value });

        var handler = new RequestReservationCommandHandler(_reservationRepository,
            _restaurantRepository,
            _executionContextAccessorMock,
            _menuRepository);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotReserveWhenNumberOfAttendeesIsGreaterThanSeats = result
            .FirstError
            .Code == "Reservation.CannotReserveWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved";

        Assert.True(isErrorCannotReserveWhenNumberOfAttendeesIsGreaterThanSeats);
    }

    [Fact]
    public async void Request_Should_ReturnAnError_WhenTimeOfReservationIsOutOfRestaurantSchedule()
    {
        //Restaurant schedule goes since now up to 8 hours after now.
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var menu = Menu.Publish(MenuId.CreateUnique(),
            restaurant.Id,
            MenuDetails.Create("Title",
                "Description",
                MenuType.Lunch,
                new Price(10.23m, "USD"),
                0.0m,
                false,
                "Chef name",
                false),
            DishSpecification.Create(),
            new List<string>(),
            new List<string>(),
            new List<string>(),
            new List<MenuSchedule>(),
            DateTime.UtcNow);

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        var command = new RequestReservationCommand(ReservedTable: restaurant.RestaurantTables.First().Number,
            Start: DateTime.Now.AddHours(10).TimeOfDay.ToString(), //Restaurant will be closed
            End: DateTime.Now.AddHours(10).AddMinutes(45).TimeOfDay.ToString(),
            ReservationDateTime: DateTime.Now.AddHours(10),
            RestaurantId: restaurant.Id.Value,
            "Customer name",
            NumberOfAttendees: 3,
            new List<Guid>() { menu.Id.Value });

        var handler = new RequestReservationCommandHandler(_reservationRepository,
            _restaurantRepository,
            _executionContextAccessorMock,
            _menuRepository);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotReserveWhenTimeOfReservationIsOutOfSchedule = result
            .FirstError
            .Code == "Restaurant.CannotReserveWhenTimeOfReservationIsOutOfSchedule";

        Assert.True(isErrorCannotReserveWhenTimeOfReservationIsOutOfSchedule);
    }

    [Fact]
    public async void Request_Should_ReturnAnError_WhenTimeOfReservationRequestedIsAfterRestaurantHasClosedOutOfItsRegularSchedule()
    {
        //Restaurant schedule goes since now up to 8 hours after now.
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var menu = Menu.Publish(MenuId.CreateUnique(),
            restaurant.Id,
            MenuDetails.Create("Title",
                "Description",
                MenuType.Lunch,
                new Price(10.23m, "USD"),
                0.0m,
                false,
                "Chef name",
                false),
            DishSpecification.Create(),
            new List<string>(),
            new List<string>(),
            new List<string>(),
            new List<MenuSchedule>(),
            DateTime.UtcNow);

        restaurant.Close(restaurant.RestaurantAdministrations.First().AdministratorId); //Closing out of its regular schedule

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        var command = new RequestReservationCommand(ReservedTable: restaurant.RestaurantTables.First().Number,
            Start: DateTime.Now.AddHours(2).TimeOfDay.ToString(), //restaurant will be closed
            End: DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay.ToString(),
            ReservationDateTime: DateTime.Now.AddHours(2),
            RestaurantId: restaurant.Id.Value,
            "Customer name",
            NumberOfAttendees: 3,
            new List<Guid>() { menu.Id.Value });

        var handler = new RequestReservationCommandHandler(_reservationRepository,
            _restaurantRepository,
            _executionContextAccessorMock,
            _menuRepository);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotReserveWhenRestaurantHasClosedOutOfSchedule = result
            .FirstError
            .Code == "Restaurant.CannotReserveWhenRestaurantHasClosedOutOfSchedule";

        Assert.True(isErrorCannotReserveWhenRestaurantHasClosedOutOfSchedule);
    }

    [Fact]
    public async void Request_Should_ReturnAnError_WhenTableWillBeOccuppiedInTheReservationTimeRequested()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var menu = Menu.Publish(MenuId.CreateUnique(),
            restaurant.Id,
            MenuDetails.Create("Title",
                "Description",
                MenuType.Lunch,
                new Price(10.23m, "USD"),
                0.0m,
                false,
                "Chef name",
                false),
            DishSpecification.Create(),
            new List<string>(),
            new List<string>(),
            new List<string>(),
            new List<MenuSchedule>(),
            DateTime.UtcNow);

        restaurant.ReserveTable(1,
            new Domain.Common.TimeRange(DateTime.Now.AddHours(2).TimeOfDay, 
                DateTime.Now.AddHours(2).AddMinutes(30).TimeOfDay),
            DateTime.Now.AddHours(2));

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        var command = new RequestReservationCommand(ReservedTable: restaurant.RestaurantTables.First().Number,
            Start: DateTime.Now.AddHours(2).TimeOfDay.ToString(), //restaurant will be occupied at this hour
            End: DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay.ToString(),
            ReservationDateTime: DateTime.Now.AddHours(2),
            RestaurantId: restaurant.Id.Value,
            "Customer name",
            NumberOfAttendees: 3,
            new List<Guid>() { menu.Id.Value });

        var handler = new RequestReservationCommandHandler(_reservationRepository,
            _restaurantRepository,
            _executionContextAccessorMock,
            _menuRepository);

        var result = await handler.Handle(command, CancellationToken.None);

        bool isErrorCannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTime = result
            .FirstError
            .Code == "Table.CannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTime";

        Assert.True(isErrorCannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTime);
    }

    [Fact]
    public async void Request_Should_ReturnAReservationId_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var menu = Menu.Publish(MenuId.CreateUnique(),
            restaurant.Id,
            MenuDetails.Create("Title",
                "Description",
                MenuType.Lunch,
                new Price(10.23m, "USD"),
                0.0m,
                false,
                "Chef name",
                false),
            DishSpecification.Create(),
            new List<string>(),
            new List<string>(),
            new List<string>(),
            new List<MenuSchedule>(),
            DateTime.UtcNow);

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        var command = new RequestReservationCommand(ReservedTable: restaurant.RestaurantTables.First().Number,
            Start: DateTime.Now.AddHours(2).TimeOfDay.ToString(),
            End: DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay.ToString(),
            ReservationDateTime: DateTime.Now.AddHours(2),
            RestaurantId: restaurant.Id.Value,
            "Customer name",
            NumberOfAttendees: 3,
            new List<Guid>() { menu.Id.Value });

        var handler = new RequestReservationCommandHandler(_reservationRepository,
            _restaurantRepository,
            _executionContextAccessorMock,
            _menuRepository);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.IsType<Guid>(result.Value);
    }

    [Fact]
    public async void Request_Should_AddReservationToTheDatabase_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var menu = Menu.Publish(MenuId.CreateUnique(),
            restaurant.Id,
            MenuDetails.Create("Title",
                "Description",
                MenuType.Lunch,
                new Price(10.23m, "USD"),
                0.0m,
                false,
                "Chef name",
                false),
            DishSpecification.Create(),
            new List<string>(),
            new List<string>(),
            new List<string>(),
            new List<MenuSchedule>(),
            DateTime.UtcNow);

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        var command = new RequestReservationCommand(ReservedTable: restaurant.RestaurantTables.First().Number,
            Start: DateTime.Now.AddHours(2).TimeOfDay.ToString(),
            End: DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay.ToString(),
            ReservationDateTime: DateTime.Now.AddHours(2),
            RestaurantId: restaurant.Id.Value,
            "Customer name",
            NumberOfAttendees: 3,
            new List<Guid>() { menu.Id.Value });

        var handler = new RequestReservationCommandHandler(_reservationRepository,
            _restaurantRepository,
            _executionContextAccessorMock,
            _menuRepository);

        await handler.Handle(command, CancellationToken.None);

        Reservation? getReservation = await DbContext
            .Reservations
            .Where(r => r.RestaurantId == restaurant.Id)
            .FirstOrDefaultAsync();

        bool isReservationAddedToDatabase = DbContext
            .Reservations
            .Any();

        Assert.True(isReservationAddedToDatabase);
    }

    [Fact]
    public async void Request_Should_AddRestaurantReservationToTheDatabase_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        var menu = Menu.Publish(MenuId.CreateUnique(),
            restaurant.Id,
            MenuDetails.Create("Title",
                "Description",
                MenuType.Lunch,
                new Price(10.23m, "USD"),
                0.0m,
                false,
                "Chef name",
                false),
            DishSpecification.Create(),
            new List<string>(),
            new List<string>(),
            new List<string>(),
            new List<MenuSchedule>(),
            DateTime.UtcNow);

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        await DbContext.Menus.AddAsync(menu);
        await DbContext.SaveChangesAsync();

        var command = new RequestReservationCommand(1,
            Start: DateTime.Now.AddHours(2).TimeOfDay.ToString(),
            End: DateTime.Now.AddHours(2).AddMinutes(45).TimeOfDay.ToString(),
            ReservationDateTime: DateTime.Now.AddHours(2),
            RestaurantId: restaurant.Id.Value,
            "Customer name",
            NumberOfAttendees: 3,
            new List<Guid>() { menu.Id.Value });

        var handler = new RequestReservationCommandHandler(_reservationRepository,
            _restaurantRepository,
            _executionContextAccessorMock,
            _menuRepository);

        await handler.Handle(command, CancellationToken.None);

        var getRestaurant = await DbContext
            .Restaurants
            .Where(r => r.Id == restaurant.Id)
            .SingleOrDefaultAsync();

        bool isReservedHourStored = getRestaurant!.RestaurantTables.Where(r => r.Number == 1).Single().ReservedHours.Any();

        Assert.True(isReservedHourStored);
    }
}
