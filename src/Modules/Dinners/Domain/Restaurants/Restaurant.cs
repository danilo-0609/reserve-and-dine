using BuildingBlocks.Domain.AggregateRoots;
using Dinners.Domain.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.Events;
using Dinners.Domain.Restaurants.RestaurantInformations;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using Dinners.Domain.Restaurants.RestaurantTables;
using Dinners.Domain.Restaurants.RestaurantUsers;
using Dinners.Domain.Restaurants.Rules;
using ErrorOr;
using MediatR;
using System.Data;

namespace Domain.Restaurants;

public sealed class Restaurant : AggregateRoot<RestaurantId, Guid>
{
    private readonly List<RestaurantRatingId> _ratingIds = new();
    private readonly List<RestaurantClient> _restaurantClients = new();
    private readonly List<RestaurantTable> _restaurantTables = new();
    private readonly List<RestaurantAdministration> _restaurantAdministrations = new();

    public new RestaurantId Id { get; private set; }

    public int NumberOfTables { get; private set; }

    public AvailableTablesStatus AvailableTablesStatus { get; private set; }

    public RestaurantInformation RestaurantInformation { get; private set; }

    public RestaurantLocalization RestaurantLocalization { get; private set; }

    public RestaurantScheduleStatus RestaurantScheduleStatus { get; private set; }

    public RestaurantSchedule RestaurantSchedule { get; private set; }

    public RestaurantContact RestaurantContact { get; private set; }

    public IReadOnlyList<RestaurantRatingId> RestaurantRatingIds => _ratingIds.AsReadOnly();

    public IReadOnlyList<RestaurantClient> RestaurantClients => _restaurantClients.AsReadOnly();

    public IReadOnlyList<RestaurantTable> RestaurantTables => _restaurantTables.AsReadOnly();

    public IReadOnlyList<RestaurantAdministration> RestaurantAdministrations => _restaurantAdministrations.AsReadOnly();

    public DateTime PostedAt { get; private set; }


    public static Restaurant Post(RestaurantInformation restaurantInformation,
        RestaurantLocalization restaurantLocalization,
        RestaurantSchedule restaurantSchedule,
        RestaurantContact restaurantContact,
        List<RestaurantTable> restaurantTables,
        List<RestaurantAdministration> restaurantAdministrations,
        DateTime postedAt)
    {
        var restaurant = new Restaurant(RestaurantId.CreateUnique(),
            restaurantTables.Count,
            restaurantInformation,
            restaurantLocalization,
            restaurantSchedule,
            restaurantContact,
            restaurantTables,
            restaurantAdministrations,
            new List<RestaurantClient>(),
            postedAt);

        restaurant.AddDomainEvent(new NewRestaurantPostedDomainEvent(
            Guid.NewGuid(),
            restaurant.Id,
            restaurantAdministrations.First().AdministratorId,
            DateTime.UtcNow));

        return restaurant;
    }

    public Restaurant Update(int numberOfTables,
        AvailableTablesStatus availableTablesStatus,
        RestaurantInformation restaurantInformation,
        RestaurantLocalization restaurantLocalization,
        RestaurantScheduleStatus restaurantScheduleStatus,
        RestaurantSchedule restaurantSchedule,
        RestaurantContact restaurantContact,
        List<RestaurantRatingId> ratingIds,
        List<RestaurantClient> restaurantClients,
        List<RestaurantTable> restaurantTables,
        List<RestaurantAdministration> restaurantAdministrations)
    {
        return new Restaurant(Id,
            numberOfTables,
            availableTablesStatus,
            restaurantInformation,
            restaurantLocalization,
            restaurantScheduleStatus,
            restaurantSchedule,
            restaurantContact,
            ratingIds,
            restaurantClients,
            restaurantTables,
            restaurantAdministrations,
            PostedAt);
    }

    public ErrorOr<RestaurantRating> Rate(int stars,
        Guid clientId,
        string comment = "")
    {
        var mustNotBeAnAdministrator = CheckRule(new CannotRateWhenUserIsAdministratorRule(clientId, _restaurantAdministrations));

        if (mustNotBeAnAdministrator.IsError)
        {
            return mustNotBeAnAdministrator.FirstError;
        }

        ErrorOr<RestaurantRating> ratingOperation = RestaurantRating.GiveRating(Id,
            RestaurantInformation.Title,
            stars,
            clientId,
            RestaurantClients
                .Select(g => g.ClientId == clientId)
                .FirstOrDefault(),
            DateTime.UtcNow,
            comment);

        if (ratingOperation.IsError)
        {
            return ratingOperation.FirstError;
        }

        return ratingOperation.Value;
    }

    public ErrorOr<RestaurantScheduleStatus> Close(
        Guid userId)
    {
        var canChangeRestaurantScheduleStatusRule = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(RestaurantAdministrations.ToList(), userId));

        if (canChangeRestaurantScheduleStatusRule.IsError)
        {
            return canChangeRestaurantScheduleStatusRule.FirstError;
        }

        var mustNotBeClosedRule = CheckRule(new CannotCloseWhenRestaurantScheduleStatusIsClosedRule(RestaurantScheduleStatus));

        if (mustNotBeClosedRule.IsError)
        {
            return mustNotBeClosedRule.FirstError;
        }

        RestaurantScheduleStatus = RestaurantScheduleStatus.Closed;

        return RestaurantScheduleStatus.Closed;
    }

    public ErrorOr<Unit> Open(
        Guid userId)
    {
        var canOpenRule = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(RestaurantAdministrations.ToList(), userId));

        if (canOpenRule.IsError)
        {
            return canOpenRule.FirstError;
        }

        var mustNotBeOpenedRule = CheckRule(new CannotOpenWhenRestaurantScheduleStatusIsOpenedRule(RestaurantScheduleStatus));

        if (mustNotBeOpenedRule.IsError)
        {
            return mustNotBeOpenedRule.FirstError;
        }

        RestaurantScheduleStatus = RestaurantScheduleStatus.Opened;

        return Unit.Value;
    }

    public ErrorOr<Unit> AddTable(Guid userId,
        int number,
        int seats,
        bool isPremium)
    {
        var canAddTable = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, userId));
    
        if (canAddTable.IsError)
        {
            return canAddTable.FirstError;
        }

        if (_restaurantTables.Any(g => g.Number == number))
        {
            return RestaurantErrorCodes.CannotAddTableWithDuplicateNumber;
        }

        RestaurantTable restaurantTable = RestaurantTable.Create(number, 
            seats, 
            isPremium, 
            new Dictionary<DateTime, TimeRange>());

        _restaurantTables.Add(restaurantTable);

        return Unit.Value;
    }

    public ErrorOr<Unit> DeleteTable(Guid userId,
        int number)
    {
        var canDeleteTable = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, userId));
    
        if (canDeleteTable.IsError)
        {
            return canDeleteTable.FirstError;
        }

        if (!_restaurantTables.Any(g => g.Number == number))
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        RestaurantTable? table = _restaurantTables.Where(r => r.Number == number).SingleOrDefault();

        _restaurantTables.Remove(table!);

        return Unit.Value;
    }

    public ErrorOr<Unit> UpgradeTable(Guid userId,
        int number,
        int seats,
        bool isPremium)
    {
        var canUpgradeTable = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, userId));

        if (canUpgradeTable.IsError)
        {
            return canUpgradeTable.FirstError;
        }

        if (!_restaurantTables.Any(r => r.Number == number))
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        RestaurantTable? table = _restaurantTables.Where(r => r.Number == number)
            .SingleOrDefault()!
            .Upgrade(number, seats, isPremium);

        return Unit.Value;
    }

    public ErrorOr<RestaurantTable> ReserveTable(int tableNumber,
        TimeRange reservationTimeRange,
        DateTime reservationDateTimeRequested)
    {
        var isRestaurantOpenToReserve = CheckRule(new TableCannotReserveWhenRestaurantIsClosedRule(RestaurantSchedule, reservationTimeRange, reservationDateTimeRequested));

        if (isRestaurantOpenToReserve.IsError)
        {
            return isRestaurantOpenToReserve.FirstError;
        }

        var cannotBeReservedWhenItWillBeOcuppiedRule = CheckRule(new TableCannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTimeNowRule(_restaurantTables, reservationDateTimeRequested, reservationTimeRange));

        if (cannotBeReservedWhenItWillBeOcuppiedRule.IsError)
        {
            return cannotBeReservedWhenItWillBeOcuppiedRule.FirstError;
        }

        RestaurantTable? table = _restaurantTables
            .Where(r => r.Number == tableNumber)
            .SingleOrDefault();

        if (table is null)
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        table.Reserve(reservationDateTimeRequested, reservationTimeRange);

        return table;
    }

    public ErrorOr<RestaurantTable> CancelReservation(int tableNumber, DateTime reservationDateTime)
    {
        RestaurantTable? table = _restaurantTables
            .Where(r => r.Number == tableNumber)
            .SingleOrDefault();

        if (table is null)
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        table.CancelReservation(reservationDateTime);

        return table;
    }

    public ErrorOr<RestaurantTable> OccupyTable(int tableNumber)
    {
        RestaurantTable? table = _restaurantTables
           .Where(r => r.Number == tableNumber)
           .SingleOrDefault();

        if (table is null)
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        var isTableFree = CheckRule(new TableMustNotBeOccuppiedToAssistRule(table.IsOccuppied));

        if (isTableFree.IsError)
        {
            return isTableFree.FirstError;
        }

        table.OccupyTable();

        return table;
    }

    public ErrorOr<RestaurantTable> FreeTable(int tableNumber)
    {
        RestaurantTable? table = _restaurantTables
            .Where(r => r.Number == tableNumber)
            .SingleOrDefault();

        if (table is null)
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        table.FreeTable();

        return table;
    }

    public RestaurantClient AddRestaurantClient(Guid clientId)
    {
        if (!RestaurantClients.Any(r => r.ClientId == clientId))
        {
            var client = RestaurantClient.Create(Id, clientId, 1);

            _restaurantClients.Add(client);

            return client;
        }

        RestaurantClient? restaurantClient = RestaurantClients.SingleOrDefault(f => f.ClientId == clientId);

        restaurantClient!.AddVisit();

        return restaurantClient;
    }

    public ErrorOr<AvailableTablesStatus> ModifyAvailableTableStatus(AvailableTablesStatus availableTablesStatus)
    {
        if (AvailableTablesStatus == availableTablesStatus)
        {
            return RestaurantErrorCodes.EqualAvailableTableStatus;
        }

        return availableTablesStatus;
    }

    public ErrorOr<Unit> UpdateInformation(Guid userId,
        string title,
        string description,
        string type,
        List<string> chefs,
        List<string> specialties,
        List<Uri> imagesUrl)
    {
        var canUpdateInformation = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, userId));
        
        if (canUpdateInformation.IsError)
        {
            return canUpdateInformation.FirstError;
        }

        RestaurantInformation restaurantInformation = RestaurantInformation
            .Create(title, description, type, chefs, specialties, imagesUrl);

        RestaurantInformation = restaurantInformation;

        return Unit.Value;
    }

    public ErrorOr<Unit> ChangeLocalization(Guid adminId,
        string country,
        string city,
        string region,
        string neighborhood,
        string address,
        string localizationDetails = "")
    {
        var canChangeLocalization = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, adminId));

        if (canChangeLocalization.IsError)
        {
            return canChangeLocalization.FirstError;
        }

        var restaurantLocalization = RestaurantLocalization.Create(
            country, city, region, neighborhood, address, localizationDetails);

        RestaurantLocalization = restaurantLocalization;

        return Unit.Value;
    }

    public ErrorOr<Unit> ModifySchedule(Guid userId,
        List<DayOfWeek> days,
        TimeSpan start,
        TimeSpan end)
    {
        var canChangeProperty = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, userId));

        if (canChangeProperty.IsError)
        {
            return canChangeProperty.FirstError;
        }

        RestaurantSchedule schedule = RestaurantSchedule.Create(days, start, end);
    
        RestaurantSchedule = schedule;

        return Unit.Value;
    }

    public ErrorOr<Unit> UpdateContact(Guid adminId,
        string email = "",
        string whatsapp = "",
        string facebook = "",
        string phoneNumber = "",
        string instagram = "",
        string twitter = "",
        string tikTok = "",
        string website = "")
    {
        var canChangeRestaurantContact = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, adminId));

        if (canChangeRestaurantContact.IsError)
        {
            return canChangeRestaurantContact.FirstError;
        }

        var restaurantContact = RestaurantContact.Create(email,
            whatsapp,
            facebook,
            phoneNumber,
            instagram,
            twitter,
            tikTok,
            website);

        RestaurantContact = restaurantContact;

        return Unit.Value;
    }

    public ErrorOr<RestaurantAdministration> AddAdministration(string name,
        Guid newAdministratorId,
        string administratorTitle,
        Guid adminId)
    {
        var canAddNewAdmin = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, adminId));

        if (canAddNewAdmin.IsError)
        {
            return canAddNewAdmin.FirstError;
        }

        if (_restaurantAdministrations.Any(r => r.AdministratorId == newAdministratorId))
        {
            return Error.Validation("RestaurantAdministration.AdministratorExists", "The administrator already exists");
        }

        var restaurantAdministration = RestaurantAdministration.Create(
            name,
            newAdministratorId,
            administratorTitle);

        _restaurantAdministrations.Add(restaurantAdministration);

        return restaurantAdministration;
    }

    public ErrorOr<Unit> DeleteAdministrator(Guid administratorId, Guid adminId)
    {
        var canDeleteAdmin = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, adminId));

        if (canDeleteAdmin.IsError)
        {
            return canDeleteAdmin.FirstError;
        }

        if (_restaurantAdministrations.Any(t => t.AdministratorId == administratorId))
        {
            return Error.NotFound("RestaurantAdministration.NotFound", "Restaurant administrator was not found");
        }

        RestaurantAdministration? restaurantAdministrator = _restaurantAdministrations
            .Where(r => r.AdministratorId == administratorId)
            .SingleOrDefault();

        _restaurantAdministrations.Remove(restaurantAdministrator!);

        return Unit.Value;
    }

    public ErrorOr<Unit> UpdateAdministrator(Guid administratorId,
        string name,
        string administratorTitle,
        Guid adminId)
    {
        var canUpdateAdmin = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, adminId));

        if (canUpdateAdmin.IsError)
        {
            return canUpdateAdmin.FirstError;
        }

        if (_restaurantAdministrations.Any(t => t.AdministratorId == administratorId))
        {
            return Error.NotFound("RestaurantAdministration.NotFound", "Restaurant administrator was not found");
        }

            _restaurantAdministrations
                .Where(r => r.AdministratorId == administratorId)
                .SingleOrDefault()!
                .Update(name, administratorId, administratorTitle);

        return Unit.Value;
    }

    private Restaurant(
        RestaurantId id, 
        int numberOfTables, 
        AvailableTablesStatus availableTablesStatus, 
        RestaurantInformation restaurantInformation, 
        RestaurantLocalization restaurantLocalization, 
        RestaurantScheduleStatus restaurantScheduleStatus, 
        RestaurantSchedule restaurantSchedule, 
        RestaurantContact restaurantContact,
        List<RestaurantRatingId> ratingIds,
        List<RestaurantClient> restaurantClients,
        List<RestaurantTable> restaurantTables,
        List<RestaurantAdministration> restaurantAdministrations,
        DateTime postedAt)
    {
        Id = id;
        NumberOfTables = numberOfTables;
        AvailableTablesStatus = availableTablesStatus;
        RestaurantInformation = restaurantInformation;
        RestaurantLocalization = restaurantLocalization;
        RestaurantScheduleStatus = restaurantScheduleStatus;
        RestaurantSchedule = restaurantSchedule;
        RestaurantContact = restaurantContact;
        PostedAt = postedAt;

        _ratingIds = ratingIds;
        _restaurantClients = restaurantClients;
        _restaurantTables = restaurantTables;
        _restaurantAdministrations = restaurantAdministrations;
    }

    private Restaurant(RestaurantId id,
        int numberOfTables,
        RestaurantInformation restaurantInformation,
        RestaurantLocalization restaurantLocalization,
        RestaurantSchedule restaurantSchedule,
        RestaurantContact restaurantContact,
        List<RestaurantTable> restaurantTables,
        List<RestaurantAdministration> restaurantAdministrations,
        List<RestaurantClient> restaurantClients,
        DateTime postedAt)
    {
        Id = id;
        NumberOfTables = numberOfTables;
        RestaurantInformation = restaurantInformation;
        RestaurantLocalization = restaurantLocalization;
        RestaurantSchedule = restaurantSchedule;
        RestaurantContact = restaurantContact;

        _restaurantClients = restaurantClients;
        _restaurantTables = restaurantTables;
        _restaurantAdministrations = restaurantAdministrations;

        RestaurantScheduleStatus = SetRestaurantScheduleStatus();
        AvailableTablesStatus = UpdateAvailableTablesStatus();

        PostedAt = postedAt;
    }

    private AvailableTablesStatus UpdateAvailableTablesStatus()
    {
        int reservedTables = _restaurantTables
            .Where(r => r.IsOccuppied == true)
            .Count();

        int percentageOfReservedTables = reservedTables % 100;

        if (percentageOfReservedTables >= _restaurantTables.Count % 60)
        {
            return AvailableTablesStatus.Few;
        }

        if (reservedTables == _restaurantTables.Count)
        {
            return AvailableTablesStatus.NoAvailables;
        }

        return AvailableTablesStatus.Availables;
    }

    private RestaurantScheduleStatus SetRestaurantScheduleStatus()
    {
        var startHour = RestaurantSchedule.HoursOfOperation.Start.Hours;

        var endHour = RestaurantSchedule.HoursOfOperation.End.Hours;

        if (DateTime.UtcNow.Hour > startHour && DateTime.UtcNow.Hour < endHour)
        {
            return RestaurantScheduleStatus.Opened;
        }

        return RestaurantScheduleStatus.Closed;
    }

    private Restaurant() { }
}
