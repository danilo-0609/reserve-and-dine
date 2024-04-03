using BuildingBlocks.Domain.AggregateRoots;
using BuildingBlocks.Domain.Results;
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
using System.Data;

namespace Domain.Restaurants;

public sealed class Restaurant : AggregateRoot<RestaurantId, Guid>
{
    private readonly List<RestaurantRatingId> _ratingIds = new();
    private readonly List<RestaurantClient> _restaurantClients = new();
    private readonly List<RestaurantTable> _restaurantTables = new();
    private readonly List<RestaurantAdministration> _restaurantAdministrations = new();
    private readonly List<RestaurantSchedule> _restaurantSchedules = new();
    private readonly List<Chef> _chefs = new();
    private readonly List<Speciality> _specialties = new();
    private readonly List<RestaurantImageUrl> _restaurantImagesUrl = new();


    public new RestaurantId Id { get; private set; }

    public int NumberOfTables { get; private set; }

    public AvailableTablesStatus AvailableTablesStatus { get; private set; }

    public RestaurantInformation RestaurantInformation { get; private set; }

    public RestaurantLocalization RestaurantLocalization { get; private set; }

    public RestaurantScheduleStatus RestaurantScheduleStatus { get; private set; }

    public RestaurantContact RestaurantContact { get; private set; }

    public List<RestaurantSchedule> RestaurantSchedules => _restaurantSchedules;

    public List<RestaurantRatingId> RestaurantRatingIds => _ratingIds;

    public List<RestaurantClient> RestaurantClients => _restaurantClients;

    public List<RestaurantTable> RestaurantTables => _restaurantTables;
        
    public List<RestaurantAdministration> RestaurantAdministrations => _restaurantAdministrations;

    public List<Speciality> Specialities => _specialties;

    public List<Chef> Chefs => _chefs;

    public List<RestaurantImageUrl> RestaurantImagesUrl => _restaurantImagesUrl;

    public DateTime PostedAt { get; private set; }


    public static Restaurant Post(RestaurantId restaurantId,
        RestaurantInformation restaurantInformation,
        RestaurantLocalization restaurantLocalization,
        List<RestaurantSchedule> restaurantSchedules,
        RestaurantContact restaurantContact,
        List<RestaurantTable> restaurantTables,
        List<RestaurantAdministration> restaurantAdministrations,
        List<string> specialities,
        List<string> chefs,
        DateTime postedAt)
    {
        var restaurant = new Restaurant(restaurantId,
            restaurantTables.Count,
            restaurantInformation,
            restaurantLocalization,
            restaurantSchedules,
            restaurantContact,
            restaurantTables,
            restaurantAdministrations,
            new List<RestaurantClient>(),
            specialities.ConvertAll(speciality => new Speciality(SpecialityId.CreateUnique(), restaurantId, speciality)),
            chefs.ConvertAll(chef => new Chef(ChefId.CreateUnique(), restaurantId, chef)),
            new List<RestaurantImageUrl>(), 
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
        List<RestaurantSchedule> restaurantSchedules,
        RestaurantContact restaurantContact,
        List<RestaurantRatingId> ratingIds,
        List<RestaurantClient> restaurantClients,
        List<RestaurantTable> restaurantTables,
        List<RestaurantAdministration> restaurantAdministrations,
        List<Speciality> specialities,
        List<Chef> chefs,
        List<RestaurantImageUrl> restaurantImageUrls)
    {
        return new Restaurant(Id,
            numberOfTables,
            availableTablesStatus,
            restaurantInformation,
            restaurantLocalization,
            restaurantScheduleStatus,
            restaurantSchedules,
            restaurantContact,
            ratingIds,
            restaurantClients,
            restaurantTables,
            restaurantAdministrations,
            specialities,
            chefs,
            restaurantImageUrls,
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

    public ErrorOr<SuccessOperation> Close(Guid userId)
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

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> Open(Guid userId)
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

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> AddTable(Guid userId,
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

        RestaurantTable restaurantTable = RestaurantTable.Create(Id,
            number, 
            seats, 
            isPremium, 
            new List<ReservedHour>());

        _restaurantTables.Add(restaurantTable);

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> DeleteTable(Guid userId, int number)
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

        _restaurantTables.RemoveAll(table => table.Number == number);

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> UpgradeTable(Guid userId,
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

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> ReserveTable(int tableNumber, TimeRange reservationTimeRange)
    {
        RestaurantTable? table = _restaurantTables
            .Where(r => r.Number == tableNumber)
            .SingleOrDefault();

        if (table is null)
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        var currentDaySchedule = RestaurantSchedules
            .Where(r => r.Day.DayOfWeek == DateTime.Now.DayOfWeek)
            .Single();

        var isRequestedTimeInRestaurantSchedule = CheckRule(new CannotReserveWhenTimeOfReservationIsOutOfScheduleRule(currentDaySchedule, reservationTimeRange));

        if (isRequestedTimeInRestaurantSchedule.IsError)
        {
            return isRequestedTimeInRestaurantSchedule.FirstError;
        }

        var isRestaurantCloseOutOfNormalSchedule = CheckRule(new CannotReserveWhenRestaurantHasClosedOutOfScheduleRule(currentDaySchedule, RestaurantScheduleStatus, reservationTimeRange));

        if (isRestaurantCloseOutOfNormalSchedule.IsError)
        {
            return isRestaurantCloseOutOfNormalSchedule.FirstError;
        }

        var cannotBeReservedWhenItWillBeOcuppiedRule = CheckRule(new TableCannotBeReservedAtCertainTimeWhenTableIsReservedAtThatTimeNowRule(_restaurantTables, reservationTimeRange));

        if (cannotBeReservedWhenItWillBeOcuppiedRule.IsError)
        {
            return cannotBeReservedWhenItWillBeOcuppiedRule.FirstError;
        }

        table.Reserve(reservationTimeRange.Start, reservationTimeRange);

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> CancelReservation(int tableNumber, DateTime reservationDateTime)
    {
        RestaurantTable? table = _restaurantTables
            .Where(r => r.Number == tableNumber)
            .SingleOrDefault();

        if (table is null)
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        table.CancelReservation(reservationDateTime);

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> OccupyTable(int tableNumber)
    {
        RestaurantTable? table = _restaurantTables
           .Where(r => r.Number == tableNumber)
           .SingleOrDefault();

        if (table is null)
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        var isTableFree = CheckRule(new TableMustNotBeOccupiedToAssistRule(table.IsOccupied));

        if (isTableFree.IsError)
        {
            return isTableFree.FirstError;
        }

        table.OccupyTable();

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> FreeTable(int tableNumber)
    {
        RestaurantTable? table = _restaurantTables
            .Where(r => r.Number == tableNumber)
            .SingleOrDefault();

        if (table is null)
        {
            return RestaurantErrorCodes.TableDoesNotExist;
        }

        var cannotFreeTableWhenTableIsNotOccupied = CheckRule(new CannotFreeTableWhenTableIsNotOccupiedRule(table.IsOccupied));

        if (cannotFreeTableWhenTableIsNotOccupied.IsError)
        {
            return cannotFreeTableWhenTableIsNotOccupied.FirstError;
        }

        table.FreeTable();

        return SuccessOperation.Code;
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

    public ErrorOr<SuccessOperation> ModifyAvailableTablesStatus(AvailableTablesStatus availableTablesStatus)
    {
        if (AvailableTablesStatus == availableTablesStatus)
        {
            return RestaurantErrorCodes.EqualAvailableTableStatus;
        }

        AvailableTablesStatus = availableTablesStatus;

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> UpdateInformation(Guid userId,
        string title,
        string description,
        string type)
    {
        var canUpdateInformation = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, userId));
        
        if (canUpdateInformation.IsError)
        {
            return canUpdateInformation.FirstError;
        }

        RestaurantInformation restaurantInformation = RestaurantInformation.Create(title, description, type);

        RestaurantInformation = restaurantInformation;

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> ChangeLocalization(Guid adminId,
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

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> ModifySchedule(Guid userId,
        DayOfWeek day,
        DateTime start,
        DateTime end)
    {
        var canChangeProperty = CheckRule(new CannotChangeRestaurantPropertiesWhenUserIsNotAdministratorRule(_restaurantAdministrations, userId));

        if (canChangeProperty.IsError)
        {
            return canChangeProperty.FirstError;
        }

        RestaurantSchedule schedule = RestaurantSchedule.Create(Id, day, start, end);

        var restaurantSchedule = _restaurantSchedules.Where(r => r.Day.DayOfWeek == day).Single();

        restaurantSchedule = schedule;

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> UpdateContact(Guid adminId,
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

        return SuccessOperation.Code;
    }

    public ErrorOr<RestaurantAdministration> AddAdministrator(string name,
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

        var restaurantAdministration = RestaurantAdministration.Create(Id,
            name,
            newAdministratorId,
            administratorTitle);

        _restaurantAdministrations.Add(restaurantAdministration);

        return restaurantAdministration;
    }

    public ErrorOr<SuccessOperation> DeleteAdministrator(Guid administratorId, Guid adminId)
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

        return SuccessOperation.Code;
    }

    public ErrorOr<SuccessOperation> UpdateAdministrator(Guid administratorId,
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

        return SuccessOperation.Code;
    }

    public void AddImage(string imageUrl, RestaurantImageUrlId id)
    {
        _restaurantImagesUrl.Add(new RestaurantImageUrl(id, Id, imageUrl));
    }

    public void RemoveImage(string imageUrl, RestaurantImageUrlId id)
    {
        _restaurantImagesUrl.Remove(new RestaurantImageUrl(id, Id, imageUrl));
    }

    private Restaurant(
        RestaurantId id, 
        int numberOfTables, 
        AvailableTablesStatus availableTablesStatus, 
        RestaurantInformation restaurantInformation, 
        RestaurantLocalization restaurantLocalization, 
        RestaurantScheduleStatus restaurantScheduleStatus, 
        List<RestaurantSchedule> restaurantSchedule, 
        RestaurantContact restaurantContact,
        List<RestaurantRatingId> ratingIds,
        List<RestaurantClient> restaurantClients,
        List<RestaurantTable> restaurantTables,
        List<RestaurantAdministration> restaurantAdministrations,
        List<Speciality> specialities,
        List<Chef> chefs,
        List<RestaurantImageUrl> restaurantImagesUrl,
        DateTime postedAt) : base(id)
    {
        Id = id;
        NumberOfTables = numberOfTables;
        AvailableTablesStatus = availableTablesStatus;
        RestaurantInformation = restaurantInformation;
        RestaurantLocalization = restaurantLocalization;
        RestaurantScheduleStatus = restaurantScheduleStatus;
        _restaurantSchedules = restaurantSchedule;
        RestaurantContact = restaurantContact;
        PostedAt = postedAt;

        _ratingIds = ratingIds;
        _restaurantClients = restaurantClients;
        _restaurantTables = restaurantTables;
        _restaurantAdministrations = restaurantAdministrations;
        _specialties = specialities;
        _chefs = chefs;
        _restaurantImagesUrl = restaurantImagesUrl;
    }

    private Restaurant(RestaurantId id,
        int numberOfTables,
        RestaurantInformation restaurantInformation,
        RestaurantLocalization restaurantLocalization,
        List<RestaurantSchedule> restaurantSchedules,
        RestaurantContact restaurantContact,
        List<RestaurantTable> restaurantTables,
        List<RestaurantAdministration> restaurantAdministrations,
        List<RestaurantClient> restaurantClients,
        List<Speciality> specialities,
        List<Chef> chefs,
        List<RestaurantImageUrl> restaurantImagesUrl,
        DateTime postedAt) : base(id)
    {
        Id = id;
        NumberOfTables = numberOfTables;
        RestaurantInformation = restaurantInformation;
        RestaurantLocalization = restaurantLocalization;
        RestaurantContact = restaurantContact;

        _restaurantSchedules = restaurantSchedules;
        _restaurantClients = restaurantClients;
        _restaurantTables = restaurantTables;
        _restaurantAdministrations = restaurantAdministrations;
        _specialties = specialities;
        _chefs = chefs;
        _restaurantImagesUrl = restaurantImagesUrl;

        RestaurantScheduleStatus = SetRestaurantScheduleStatus();
        AvailableTablesStatus = UpdateAvailableTablesStatus();

        PostedAt = postedAt;
    }

    private AvailableTablesStatus UpdateAvailableTablesStatus()
    {
        int reservedTables = _restaurantTables
            .Where(r => r.IsOccupied == true)
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
        var currentDaySchedule = RestaurantSchedules.Where(r => r.Day.DayOfWeek == DateTime.Now.DayOfWeek)
            .Single();

        var startHour = currentDaySchedule.HoursOfOperation.Start;

        var endHour = currentDaySchedule.HoursOfOperation.End;

        if (DateTime.Now > startHour && DateTime.Now < endHour)
        {
            return RestaurantScheduleStatus.Opened;
        }

        return RestaurantScheduleStatus.Closed;
    }

    private Restaurant() { }
}
