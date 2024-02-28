using BuildingBlocks.Domain.AggregateRoots;
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


    public static ErrorOr<Restaurant> Post(RestaurantInformation restaurantInformation,
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
            postedAt);

        restaurant.AddDomainEvent(new NewRestaurantPostedDomainEvent(
            Guid.NewGuid(),
            restaurant.Id,
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
        ErrorOr<RestaurantRating> ratingOperation = RestaurantRating.GiveRating(Id,
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
        var canChangeRestaurantScheduleStatusRule = CheckRule(new CannotChangeRestaurantScheduleStatusWhenUserIsNotAdministratorRule(RestaurantAdministrations.ToList(), userId));
    
        if (canChangeRestaurantScheduleStatusRule.IsError)
        {
            return canChangeRestaurantScheduleStatusRule.FirstError;
        }

        var mustNotBeClosedRule = CheckRule(new CannotCloseWhenRestaurantScheduleStatusIsClosedRule(RestaurantScheduleStatus));

        if (mustNotBeClosedRule.IsError)
        {
            return mustNotBeClosedRule.FirstError;
        }

        return RestaurantScheduleStatus.Closed;
    }

    public ErrorOr<RestaurantScheduleStatus> Open(
        Guid userId)
    {
        var canChangeRestaurantScheduleStatusRule = CheckRule(new CannotChangeRestaurantScheduleStatusWhenUserIsNotAdministratorRule(RestaurantAdministrations.ToList(), userId));

        if (canChangeRestaurantScheduleStatusRule.IsError)
        {
            return canChangeRestaurantScheduleStatusRule.FirstError;
        }

        var mustNotBeOpenedRule = CheckRule(new CannotOpenWhenRestaurantScheduleStatusIsOpenedRule(RestaurantScheduleStatus));

        if (mustNotBeOpenedRule.IsError)
        {
            return mustNotBeOpenedRule.FirstError;
        }

        return RestaurantScheduleStatus.Opened;
    }

    public ErrorOr<RestaurantTable> ReserveTable(int tableNumber)
    {
        RestaurantTable? table = _restaurantTables
            .Where(r => r.Number == tableNumber)
            .SingleOrDefault();

        if (table is null)
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        var canReserveRule = CheckRule(new TableCannotBeReservedWhenTableIsReservedNowRule(table));

        if (canReserveRule.IsError)
        {
            return canReserveRule.FirstError;
        }

        table.Reserve();

        AddDomainEvent(new TableReservedDomainEvent(Guid.NewGuid(),
            Id,
            tableNumber,
            DateTime.UtcNow));

        return table;
    }

    public ErrorOr<RestaurantTable> CancelReservation(int tableNumber)
    {
        RestaurantTable? table = _restaurantTables
            .Where(r => r.Number == tableNumber)
            .SingleOrDefault();

        if (table is null)
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        table.FreeTable();

        AddDomainEvent(new TableReservationCancelledDomainEvent(
            Guid.NewGuid(),
            Id,
            tableNumber,
            DateTime.UtcNow));

        return table;
    }

    public ErrorOr<RestaurantTable> FreeTable(int tableNumber, Guid clientId)
    {
        RestaurantTable? table = _restaurantTables
            .Where(r => r.Number == tableNumber)
            .SingleOrDefault();

        if (table is null)
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        ConfirmVisitation(clientId);

        AddDomainEvent(new TableFreedDomainEvent(Guid.NewGuid(),
            Id,
            tableNumber,
            DateTime.UtcNow));
        
        table.FreeTable();

        return table;
    }
    
    private void ConfirmVisitation(Guid clientId)
    {
        if (RestaurantClients.Any(r => r.ClientId != clientId))
        {
            var restaurantClient = RestaurantClient.Create(Id, clientId, 1);

            _restaurantClients.Add(restaurantClient);
        }
    }
    
    public ErrorOr<AvailableTablesStatus> ModifyAvailableTableStatus(AvailableTablesStatus availableTablesStatus)
    {
        if (AvailableTablesStatus == availableTablesStatus)
        {
            return RestaurantErrorCodes.EqualAvailableTableStatus;
        }

        return availableTablesStatus;        
    }

    public RestaurantInformation UpdateInformation(string title,
        string description,
        string type,
        List<string>? chefs,
        List<string>? specialties,
        List<string>? imagesUrl)
    {
        return RestaurantInformation
            .Create(title, description, type, chefs, specialties, imagesUrl);
    }
     
    public RestaurantLocalization ChangeLocalization(string country,
        string city,
        string region,
        string neighborhood,
        string address,
        string localizationDetails = "")
    {
        return RestaurantLocalization.Create(
            country, city, region, neighborhood, address, localizationDetails);
    }

    public RestaurantSchedule ModifySchedule(List<DayOfWeek> days,
        TimeSpan start,
        TimeSpan end)
    {
        return RestaurantSchedule.Create(days, start, end);
    }

    public RestaurantContact UpdateContact(string email = "",
        string whatsapp = "",
        string facebook = "",
        string phoneNumber = "",
        string instagram = "",
        string twitter = "",
        string tikTok = "",
        string website = "")
    {
        return RestaurantContact.Create(email,
            whatsapp,
            facebook,
            phoneNumber,
            instagram,
            twitter,
            tikTok,
            website);
    }

    public RestaurantAdministration AddAdministration(string name,
        Guid administratorId,
        RestaurantId restaurantId,
        string administratorTitle)
    {
        return RestaurantAdministration.Create(
            name, 
            administratorId,
            restaurantId,
            administratorTitle);
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
        DateTime postedAt)
    {
        Id = id;
        NumberOfTables = numberOfTables;
        RestaurantInformation = restaurantInformation;
        RestaurantLocalization = restaurantLocalization;
        RestaurantSchedule = restaurantSchedule;
        RestaurantContact = restaurantContact;

        _restaurantTables = restaurantTables;
        _restaurantAdministrations = restaurantAdministrations;

        RestaurantScheduleStatus = SetRestaurantScheduleStatus();
        AvailableTablesStatus = UpdateAvailableTablesStatus();

        PostedAt = postedAt;
    }

    private AvailableTablesStatus UpdateAvailableTablesStatus()
    {
        int reservedTables = _restaurantTables
            .Select(r => r.IsReserved == true)
            .Count();

        if (reservedTables < _restaurantTables.Count / 0.6)
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
