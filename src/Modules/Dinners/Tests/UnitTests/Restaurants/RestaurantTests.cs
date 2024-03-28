using Dinners.Domain.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantInformations;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using Dinners.Domain.Restaurants.RestaurantTables;
using Dinners.Domain.Restaurants.RestaurantUsers;
using Domain.Restaurants;

namespace Dinners.Tests.UnitTests.Restaurants;

public sealed class RestaurantTests
{

    public static RestaurantId RestaurantId { get; set; } = RestaurantId.CreateUnique();

    private RestaurantInformation RestaurantInformation = RestaurantInformation.Create("Restaurant name",
        "Restaurant description",
        "Fish restaurant",
        new List<string>() { "Juan Carlos González" },
        new List<string>() { "Bagre", "Sancocho de pescado", "Tilapia" },
        new List<string>());


    private RestaurantLocalization RestaurantLocalization = RestaurantLocalization.Create("Colombia",
        "Medellin",
        "Antioquia",
        "La Milagrosa",
        "Carrera 1 N° 1 - 1",
        "Restaurant localization details");

    public List<RestaurantSchedule> RestaurantSchedules()
    {
        var startDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);

        var restaurantSchedules = new List<RestaurantSchedule>
        {
            RestaurantSchedule.Create(
                DayOfWeek.Monday,
                startDateTime,
                startDateTime.AddHours(8)),

            RestaurantSchedule.Create(
                DayOfWeek.Tuesday,
                startDateTime,
                startDateTime.AddHours(8)),

            RestaurantSchedule.Create(
                DayOfWeek.Wednesday,
                startDateTime,
                startDateTime.AddHours(8)),

            RestaurantSchedule.Create(
                DayOfWeek.Thursday,
                startDateTime,
                startDateTime.AddHours(8)),

            RestaurantSchedule.Create(
                DayOfWeek.Friday,
                startDateTime,
                startDateTime.AddHours(8)),

            RestaurantSchedule.Create(
                DayOfWeek.Saturday,
                startDateTime,
                startDateTime.AddHours(8)),

            RestaurantSchedule.Create(
                DayOfWeek.Sunday,
                startDateTime,
                startDateTime.AddHours(8))
        };

        return restaurantSchedules;
    }

    private RestaurantContact RestaurantContact => RestaurantContact.Create("lasazondelanegra@gmail.com",
        "whatsapp del restaurant",
        "link de facebook del restaurante",
        "número de teléfono del restaurante",
        "link del instagram del restaurante",
        "link del twitter del restaurante",
        "link del tiktok del restaurante",
        "link del sitio web del restaurante");

    private List<RestaurantTable> RestaurantTables = new List<RestaurantTable>()
    {
        RestaurantTable.Create(RestaurantId, 1, 4, false, new List<ReservedHour>()),
        RestaurantTable.Create(RestaurantId, 2, 5, false, new List<ReservedHour>())
    };

    private List<RestaurantAdministration> RestaurantAdministrations = new List<RestaurantAdministration>()
    {
        RestaurantAdministration.Create("Juan Camilo Orozco", Guid.NewGuid(), "Director de ventas"),
        RestaurantAdministration.Create("Ana María Soto", Guid.NewGuid(), "Director de marketing"),
    };

    public Restaurant CreateRestaurant()
    {
        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            RestaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        return restaurant;
    }

    [Fact]
    public void AddRestaurantClient_Should_AddANewClient_WhenClientIsNew()
    {
        Restaurant restaurant = CreateRestaurant();

        var clientId = Guid.NewGuid();

        restaurant.AddRestaurantClient(clientId);

        bool clientIsAddedToRestaurant = restaurant.RestaurantClients.Any(r => r.ClientId == clientId);

        Assert.True(clientIsAddedToRestaurant);
    }

    [Fact]
    public void Reserve_Table_Should_ReturnAnError_WhenTimeOfReservationRequestedInterferesWithOtherReservationMade()
    {
        var restaurant = CreateRestaurant();

        var reservationDateTime = DateTime.Now.AddMinutes(20);

        restaurant.ReserveTable(1, new TimeRange(reservationDateTime, reservationDateTime.AddMinutes(45)));

        var secondReservationDateTime = DateTime.Now.AddMinutes(10);

        var reservationInterfering = restaurant.ReserveTable(
            1,
            new TimeRange(secondReservationDateTime, secondReservationDateTime.AddMinutes(30)));

        bool reservationIsError = reservationInterfering.FirstError.Code == "Table.CannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTime";

        Assert.True(reservationIsError);
    }

    [Fact]
    public void Reserve_Table_Should_ReturnAnError_WhenTimeOfReservationIsOutOfSchedule()
    {
        var startDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);

        List<RestaurantSchedule> restaurantSchedules = new()
        {
            RestaurantSchedule.Create(DateTime.Now.DayOfWeek,
                startDateTime,
                startDateTime.AddMinutes(30)),

            RestaurantSchedule.Create(DateTime.Now.AddDays(1).DayOfWeek,
                startDateTime,
                startDateTime.AddMinutes(30))
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            restaurantSchedules,
            RestaurantContact,
            RestaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        var reservationTable = restaurant.ReserveTable(1,
            new TimeRange(DateTime.Now.AddHours(1), DateTime.Now.AddHours(1).AddMinutes(30)));

        bool isErrorTimeOfReservationIsOutOfSchedule = reservationTable.FirstError.Code == "Restaurant.CannotReserveWhenTimeOfReservationIsOutOfSchedule";

        Assert.True(isErrorTimeOfReservationIsOutOfSchedule);
    }

    [Fact]
    public void Reserve_Table_Should_ReturnAnError_WhenRestaurantHasClosedOutOfSchedule()
    {
        var startDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);

        List<RestaurantSchedule> restaurantSchedules = new()
        {
            RestaurantSchedule.Create(DateTime.Now.DayOfWeek,
                startDateTime,
                startDateTime.AddHours(8)),

            RestaurantSchedule.Create(DateTime.Now.AddDays(1).DayOfWeek,
                startDateTime,
                startDateTime.AddHours(8))
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            restaurantSchedules,
            RestaurantContact,
            RestaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        //Restaurant closed out of regular schedule
        restaurant.Close(restaurant.RestaurantAdministrations.First().AdministratorId);

        var reservationTable = restaurant.ReserveTable(1,
            new TimeRange(DateTime.Now.AddHours(1), DateTime.Now.AddHours(1).AddMinutes(30)));

        bool isErrorCannotReserveWhenRestaurantHasClosedOutOfSchedule =
            reservationTable
            .FirstError
            .Code == "Restaurant.CannotReserveWhenRestaurantHasClosedOutOfSchedule";

        Assert.True(isErrorCannotReserveWhenRestaurantHasClosedOutOfSchedule);
    }

    [Fact]
    public void ReserveTable_Should_SaveTheReservedHouInRestaurantTables_WhenSuccessful()
    {
        var restaurant = CreateRestaurant();

        var tableReservation = restaurant.ReserveTable(
            1,
            new TimeRange(DateTime.Now.AddHours(1), DateTime.Now.AddHours(1).AddMinutes(30)));

        bool isStoredInsideRestaurantTable = restaurant
            .RestaurantTables
            .Where(table => table.Number == 1)
            .Single()
            .ReservedHours
            .Any();

        Assert.True(isStoredInsideRestaurantTable);
    }

    [Fact]
    public void Close_Should_ReturnAnError_WhenUserIsNotAnAdministrator()
    {
        var restaurant = CreateRestaurant();

        var closeRestaurant = restaurant.Close(Guid.NewGuid());

        bool isErrorCannotChangeRestaurantProperties = closeRestaurant.FirstError.Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public void Close_Should_ReturnAnError_WhenRestaurantIsClosed()
    {
        var startDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);

        List<RestaurantSchedule> restaurantSchedules = new()
        {
            RestaurantSchedule.Create(DateTime.Now.DayOfWeek,
                startDateTime,
                startDateTime),

            RestaurantSchedule.Create(DateTime.Now.AddDays(1).DayOfWeek,
                startDateTime,
                startDateTime)
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            restaurantSchedules,
            RestaurantContact,
            RestaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        var restaurantClose = restaurant.Close(restaurant.RestaurantAdministrations.First().AdministratorId);

        bool isErrorCannotCloseTheRestaurant = restaurantClose.FirstError.Code == "Restaurant.CannotCloseTheRestaurant";

        Assert.True(isErrorCannotCloseTheRestaurant);
    }

    [Fact]
    public void Close_Should_TurnScheduleStatusToClose_WhenSuccessful()
    {
        var restaurant = CreateRestaurant();

        restaurant.Close(restaurant.RestaurantAdministrations.First().AdministratorId);

        bool isRestaurantScheduleStatusClosed = restaurant
            .RestaurantScheduleStatus == RestaurantScheduleStatus.Closed;

        Assert.True(isRestaurantScheduleStatusClosed);
    }

    [Fact]
    public void Open_Should_ReturnAnError_WhenUserIsNotAnAdministrator()
    {
        var restaurant = CreateRestaurant();

        var openRestaurant = restaurant.Open(Guid.NewGuid());

        bool isErrorCannotChangeRestaurantProperties = openRestaurant
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public void Open_Should_ReturnAnError_WhenRestaurantIsOpened()
    {
        var restaurant = CreateRestaurant();

        var openRestaurant = restaurant.Open(restaurant.RestaurantAdministrations.First().AdministratorId);

        bool isErrorCannotOpenTheRestaurant = openRestaurant
            .FirstError
            .Code == "Restaurant.CannotOpenTheRestaurant";

        Assert.True(isErrorCannotOpenTheRestaurant);
    }

    [Fact]
    public void Open_Should_TurnScheduleStatusToOpen_WhenSuccessful_()
    {
        var openingHour = DateTime.Now.AddHours(1).Hour;

        var startDateTime = new DateTime(DateTime.Now.Year,
            DateTime.Now.Month,
            DateTime.Now.Day,
            openingHour,
            0,
            0);

        List<RestaurantSchedule> restaurantSchedules = new()
        {
            RestaurantSchedule.Create(DateTime.Now.DayOfWeek,
                startDateTime,
                startDateTime.AddHours(8)),

            RestaurantSchedule.Create(DateTime.Now.AddDays(1).DayOfWeek,
                startDateTime,
                startDateTime.AddHours(8))
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            restaurantSchedules,
            RestaurantContact,
            RestaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        restaurant.Open(restaurant.RestaurantAdministrations.First().AdministratorId);

        bool isRestaurantOpened = restaurant.RestaurantScheduleStatus == RestaurantScheduleStatus.Opened;

        Assert.True(isRestaurantOpened);
    }

    [Fact]
    public void AddTable_Should_ReturnAnError_WhenUserIsNotAdministrator()
    {
        var restaurant = CreateRestaurant();

        var addTable = restaurant.AddTable(Guid.NewGuid(), 3, 4, true);

        bool isErrorCannotChangeRestaurantProperties = addTable
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public void AddTable_Should_ReturnAnError_WhenNumberOfTableRequestedAlreadyExists()
    {
        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, 2, 5, false, new List<ReservedHour>())
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        var addTable = restaurant.AddTable(restaurant.RestaurantAdministrations.First().AdministratorId,
            1,
            4,
            true);

        bool isErrorCannotAddTableWithDuplicateNumber = addTable
            .FirstError
            .Code == "Restaurant.CannotAddTableWithDuplicateNumber";

        Assert.True(isErrorCannotAddTableWithDuplicateNumber);
    }

    [Fact]
    public void AddTable_Should_StoreTheNewTableInsideRestaurantTables_WhenSuccessful()
    {
        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, 2, 5, false, new List<ReservedHour>())
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        var tableNumber = 3;

        restaurant.AddTable(restaurant.RestaurantAdministrations.First().AdministratorId, tableNumber, 4, true);

        bool isNewRestaurantTableStoredInRestaurantTables = restaurant
            .RestaurantTables
            .Any(table => table.Number == tableNumber);

        Assert.True(isNewRestaurantTableStoredInRestaurantTables);
    }

    [Fact]
    public void DeleteTable_Should_ReturnAnError_WhenUserIsNotAdministrator()
    {
        var restaurant = CreateRestaurant();

        var deleteTable = restaurant.DeleteTable(Guid.NewGuid(), 1);

        bool isErrorCannotChangeRestaurantProperties = deleteTable
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public void DeleteTable_ReturnAnError_WhenTableDoesNotExist()
    {
        var restaurant = CreateRestaurant();

        var tableNumber = 4;

        var deleteTable = restaurant.DeleteTable(restaurant.RestaurantAdministrations.First().AdministratorId,
            tableNumber);

        bool isErrorTableDoesNotExist = deleteTable
            .FirstError
            .Code == "Restaurant.TableDoesNotExist";

        Assert.True(isErrorTableDoesNotExist);
    }

    [Fact]
    public void DeleteTable_Should_RemoveTableFromRestaurantTables_WhenSuccessful()
    {
        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, 2, 5, false, new List<ReservedHour>())
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        var deleteTable = restaurant.DeleteTable(restaurant.RestaurantAdministrations.First().AdministratorId,
            1);

        bool isTableRemovedFromRestaurantTables = !restaurant
            .RestaurantTables
            .Any(r => r.Number == 1);

        Assert.True(isTableRemovedFromRestaurantTables);
    }

    [Fact]
    public void UpgradeTable_Should_ReturnAnError_WhenUserIsNotAdministrator()
    {
        var restaurant = CreateRestaurant();

        var upgradeTable = restaurant.UpgradeTable(Guid.NewGuid(), 1, 5, true);

        bool isErrorCannotChangeRestaurantProperties = upgradeTable
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public void UpgradeTable_Should_ReturnAnError_WhenTableDoesNotExist()
    {
        var restaurant = CreateRestaurant();

        var tableNumber = 4;

        var deleteTable = restaurant.UpgradeTable(restaurant.RestaurantAdministrations.First().AdministratorId,
            tableNumber,
            5,
            true);

        bool isErrorTableDoesNotExist = deleteTable
            .FirstError
            .Code == "Restaurant.TableDoesNotExist";

        Assert.True(isErrorTableDoesNotExist);
    }

    [Fact]
    public void UpgradeTable_Should_UpdateRestaurantTable_WhenSuccessful()
    {
        var firstRestaurantTable = RestaurantTable.Create(RestaurantId, 1, 4, false, new List<ReservedHour>());

        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, 2, 5, false, new List<ReservedHour>())
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        var newAmountOfSeats = 6;

        restaurant.UpgradeTable(restaurant.RestaurantAdministrations.First().AdministratorId,
            1,
            newAmountOfSeats,
            true);

        var restauranTableUpdated = restaurant
            .RestaurantTables
            .Where(r => r.Number == 1)
            .Single();

        bool hasRestaurantTableChanged = firstRestaurantTable != restauranTableUpdated;

        Assert.True(hasRestaurantTableChanged);
    }

    [Fact]
    public void CancelReservation_Should_ReturnAnError_WhenTableDoesNotExist()
    {
        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, number: 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, number: 2, 5, false, new List<ReservedHour>())
        };

        var tableNumber = 3;

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        var cancelTableReservation = restaurant.CancelReservation(tableNumber, DateTime.Now);

        bool isErrorTableDoesNotExist = cancelTableReservation
            .FirstError
            .Code == "Restaurant.TableDoesNotExist";

        Assert.True(isErrorTableDoesNotExist);
    }

    [Fact]
    public void CancelReservation_Should_RemoveReservationFromRestaurantTables_WhenSuccessful()
    {
        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, number: 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, number: 2, 5, false, new List<ReservedHour>())
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        var timeOfReservation = DateTime.Now.AddHours(1);

        restaurant.ReserveTable(1, new TimeRange(timeOfReservation, timeOfReservation.AddMinutes(30)));

        restaurant.CancelReservation(1, timeOfReservation);

        bool isReservationTimeRemovedFromRestaurantTables = !restaurant
            .RestaurantTables
            .Where(r => r.Number == 1)
            .Single()
            .ReservedHours
            .Any(r => r.ReservationDateTime == timeOfReservation);

        Assert.True(isReservationTimeRemovedFromRestaurantTables);
    }

    [Fact]
    public void OccupyTable_Should_ReturnAnError_WhenTableDoesNotExist()
    {
        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, number: 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, number: 2, 5, false, new List<ReservedHour>())
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        var tableNumber = 3;

        var occupyTable = restaurant.OccupyTable(tableNumber);

        bool isErrorTableDoesNotExist = occupyTable
            .FirstError
            .Code == "Restaurant.TableDoesNotExist";

        Assert.True(isErrorTableDoesNotExist);
    }

    [Fact]
    public void OccupyTable_Should_ReturnAnError_WhenTableIsOccupiedNow()
    {
        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, number: 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, number: 2, 5, false, new List<ReservedHour>())
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        restaurant.OccupyTable(1);

        var occupyTable = restaurant.OccupyTable(1);

        bool isErrorTableIsNotFree = occupyTable
            .FirstError
            .Code == "Restaurant.TableIsNotFree";

        Assert.True(isErrorTableIsNotFree);
    }

    [Fact]
    public void OccupyTable_Should_SetRestaurantTableAsOccupied_WhenSuccessful()
    {
        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, number: 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, number: 2, 5, false, new List<ReservedHour>())
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        restaurant.OccupyTable(1);

        bool isRestaurantTableOccupied = restaurant
            .RestaurantTables
            .Where(r => r.Number == 1)
            .Single()
            .IsOccupied == true;

        Assert.True(isRestaurantTableOccupied);
    }

    [Fact]
    public void FreeTable_Should_ReturnAnError_WhenTableDoesNotExist()
    {
        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, number: 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, number: 2, 5, false, new List<ReservedHour>())
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        var tableNumber = 3;

        var freeTable = restaurant.FreeTable(tableNumber);

        bool isErrorTableDoesNotExist = freeTable
            .FirstError
            .Code == "Restaurant.TableDoesNotExist";

        Assert.True(isErrorTableDoesNotExist);
    }

    [Fact]
    public void FreeTable_Should_ReturnAnError_WhenTableIsNotOccupied()
    {
        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, number: 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, number: 2, 5, false, new List<ReservedHour>())
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        var freeTable = restaurant.FreeTable(1);

        bool isErrorTableIsNotOccupied = freeTable
            .FirstError
            .Code == "Restaurant.CannotFreeTableWhenTableIsNotOccupied";

        Assert.True(isErrorTableIsNotOccupied);
    }

    [Fact]
    public void FreeTable_Should_TurnIsOccupiedPropertyRestaurantTableToFalse_WhenSuccessful()
    {
        List<RestaurantTable> restaurantTables = new()
        {
            RestaurantTable.Create(RestaurantId, number: 1, 4, false, new List<ReservedHour>()),

            RestaurantTable.Create(RestaurantId, number: 2, 5, false, new List<ReservedHour>())
        };

        var restaurant = Restaurant.Post(RestaurantId,
            RestaurantInformation,
            RestaurantLocalization,
            RestaurantSchedules(),
            RestaurantContact,
            restaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        restaurant.OccupyTable(1);

        restaurant.FreeTable(1);

        bool isOccupiedPropertyFalse = restaurant
            .RestaurantTables
            .Where(r => r.Number == 1)
            .Single()
            .IsOccupied == false;

        Assert.True(isOccupiedPropertyFalse);
    }

    [Fact]
    public void AddRestaurantClient_Should_CreateANewRestaurantClient_WhenClientHadNotVisitedTheRestaurant()
    {
        var restaurant = CreateRestaurant();

        var clientId = Guid.NewGuid();

        restaurant.AddRestaurantClient(clientId);

        bool isClientAddedToRestaurantClients = restaurant
            .RestaurantClients
            .Any(r => r.ClientId == clientId);

        Assert.True(isClientAddedToRestaurantClients);
    }

    [Fact]
    public void AddRestaurantClient_Should_AddANewVisitForTheClient_WhenClientHasVisitedTheRestaurantNow()
    {
        var restaurant = CreateRestaurant();

        var clientId = Guid.NewGuid();

        restaurant.AddRestaurantClient(clientId);

        //A new visit of the same user in the restaurant
        restaurant.AddRestaurantClient(clientId);

        bool isClientVisitAddedToRestaurantClients = restaurant
            .RestaurantClients
            .Where(r => r.ClientId == clientId)
            .Single()
            .NumberOfVisits > 1;

        Assert.True(isClientVisitAddedToRestaurantClients);
    }

    [Fact]
    public void ModifyAvailableTableStatus_Should_ReturnError_WhenAttemptingToModifyAvailableTableStatusWithNoChange()
    {
        var restaurant = CreateRestaurant();

        var modifyAvailableTableStatus = restaurant.ModifyAvailableTablesStatus(restaurant.AvailableTablesStatus);

        bool isErrorEqualAvailableTableStatus = modifyAvailableTableStatus
            .FirstError
            .Code == "Restaurant.EqualAvailableTableStatus";

        Assert.True(isErrorEqualAvailableTableStatus);
    }

    [Fact]
    public void ModifyAvailableTableStatus_Should_ChangeAvailableTablesStatusToTheStatusRequested_WhenSuccessful()
    {
        var restaurant = CreateRestaurant();

        //The default available tables status is "available" when creating a new restaurant instance 
        restaurant.ModifyAvailableTablesStatus(AvailableTablesStatus.NoAvailables);

        bool hasAvailableTablesStatusChangedToTheRequestedStatus = restaurant
            .AvailableTablesStatus == AvailableTablesStatus.NoAvailables;

        Assert.True(hasAvailableTablesStatusChangedToTheRequestedStatus);
    }

    [Fact]
    public void UpdateInformation_Should_ReturnAnError_WhenUserRequestingIsNotARestaurantAdministrator()
    {
        var restaurant = CreateRestaurant();

        var updateInformation = restaurant.UpdateInformation(Guid.NewGuid(),
            "Restaurant name",
            "Restaurant description",
            "Restaurant type",
            new List<string>(),
            new List<string>(),
            new List<string>());

        bool isErrorCannotChangeRestaurantProperties = updateInformation
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeRestaurantProperties);
    }

    [Fact]
    public void Rate_Should_ReturnAnError_WhenUserRatingIsAnAdministrator()
    {
        var restaurant = CreateRestaurant();

        var rating = restaurant.Rate(stars: 5,
            clientId: restaurant.RestaurantAdministrations.First().AdministratorId,
            "some comment");

        bool isErrorRateWhenUserIsAdministrator = rating
            .FirstError
            .Code == "Restaurant.RateWhenUserIsAdministrator";

        Assert.True(isErrorRateWhenUserIsAdministrator);
    }

    [Fact]
    public void Rate_Should_ReturnAnError_WhenClientHasNotVisitedTheRestaurantYet()
    {
        var restaurant = CreateRestaurant();

        var rating = restaurant.Rate(stars: 5,
            clientId: Guid.NewGuid(),
            "some comment");

        bool isErrorCannotRate = rating
            .FirstError
            .Code == "RestaurantRatings.CannotRate";

        Assert.True(isErrorCannotRate);
    }

    [Fact]
    public void Rate_Should_ReturnARestaurantRatingEntity_WhenSuccessful()
    {
        var restaurant = CreateRestaurant();

        var clientId = Guid.NewGuid();

        //Add restaurant client after a reservation
        restaurant.AddRestaurantClient(clientId);

        var rating = restaurant.Rate(stars: 5,
            clientId: clientId,
            "some comment");

        bool isReturningARestaurantRatingEntity = rating
            .Value
            .GetType() == typeof(RestaurantRating);

        Assert.True(isReturningARestaurantRatingEntity);
    }

    [Fact]
    public void ChangeLocalization_Should_ReturnAnError_WhenUserIsNotAdministrator()
    {
        var restaurant = CreateRestaurant();

        var changeLocalization = restaurant.ChangeLocalization(Guid.NewGuid(),
            "Restaurant country",
            "Restaurant city",
            "Restaurant region",
            "Restaurant neighborhood",
            "Restaurant address",
            "Restaurant localization details");

        bool isErrorCannotChangeWhenUserIsNotAdministrator = changeLocalization
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeWhenUserIsNotAdministrator);
    }

    [Fact]
    public void ModifySchedule_Should_ReturnAnError_WhenUserIsNotAdministrator()
    {
        var restaurant = CreateRestaurant();

        var modifySchedule = restaurant.ModifySchedule(Guid.NewGuid(),
            DateTime.Now.DayOfWeek,
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0),
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.AddHours(7).Hour, DateTime.Now.AddHours(7).Minute, 0));

        bool isErrorCannotChangeWhenUserIsNotAdministrator = modifySchedule
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeWhenUserIsNotAdministrator);
    }

    [Fact]
    public void UpdateContact_Should_ReturnAnError_WhenUserIsNotAdministrator()
    {
        var restaurant = CreateRestaurant();

        var updateContact = restaurant.UpdateContact(Guid.NewGuid(),
            "Restaurant email",
            "Restaurant whatsapp",
            "Restaurant facebook",
            "Restaurant phone number",
            "Restaurant instagram",
            "Restaurant twitter",
            "Restaurant tik tok",
            "Restaurant website");

        bool isErrorCannotChangeWhenUserIsNotAdministrator = updateContact
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeWhenUserIsNotAdministrator);
    }

    [Fact]
    public void AddAdministrator_Should_ReturnAnError_WhenUserIsNotAdministrator()
    {
        var restaurant = CreateRestaurant();

        var noAdminUser = Guid.NewGuid();

        var addAdministrator = restaurant.AddAdministrator("Admin name",
            noAdminUser,
            "Admin title",
            Guid.NewGuid());

        bool isErrorCannotChangeWhenUserIsNotAdministrator = addAdministrator
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeWhenUserIsNotAdministrator);
    }

    [Fact]
    public void UpdateAdministrator_Should_ReturnAnError_WhenUserIsNotAdministrator()
    {
        var restaurant = CreateRestaurant();

        var noAdminUser = Guid.NewGuid();

        var updateAdministrator = restaurant.UpdateAdministrator(noAdminUser,
            "Admin name",
            "Admin title",
            Guid.NewGuid());

        bool isErrorCannotChangeWhenUserIsNotAdministrator = updateAdministrator
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeWhenUserIsNotAdministrator);
    }

    [Fact]
    public void DeleteAdministrator_Should_ReturnAnError_WhenUserIsNotAdministrator()
    {
        var restaurant = CreateRestaurant();

        var noAdminUser = Guid.NewGuid();

        var updateAdministrator = restaurant.DeleteAdministrator(noAdminUser,
            Guid.NewGuid());

        bool isErrorCannotChangeWhenUserIsNotAdministrator = updateAdministrator
            .FirstError
            .Code == "Restaurant.CannotChangeRestaurantProperties";

        Assert.True(isErrorCannotChangeWhenUserIsNotAdministrator);
    }
}

