using BuildingBlocks.Domain.AggregateRoots;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Events;
using Dinners.Domain.Restaurants.RestaurantRatings;
using ErrorOr;

namespace Domain.Restaurants;

public sealed class Restaurant : AggregateRoot<RestaurantId, Guid>
{
    private readonly List<RestaurantRatingId> _ratingIds = new();
    private readonly List<RestaurantClient> _restaurantClients = new();
    private readonly List<RestaurantTable> _restaurantTables = new();
    
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

    public DateTime PostedAt { get; private set; }

    public static ErrorOr<Restaurant> Post(RestaurantInformation restaurantInformation,
        RestaurantLocalization restaurantLocalization,
        RestaurantSchedule restaurantSchedule,
        RestaurantContact restaurantContact,
        List<RestaurantRatingId> ratingIds,
        List<RestaurantClient> restaurantClients,
        List<RestaurantTable> restaurantTables,
        DateTime postedAt)
    {
        var restaurant = new Restaurant(RestaurantId.CreateUnique(),
            restaurantTables.Count,
            restaurantInformation,
            restaurantLocalization,
            restaurantSchedule,
            restaurantContact,
            ratingIds,
            restaurantClients,
            restaurantTables,
            postedAt);

        restaurant.AddDomainEvent(new NewRestaurantPostedDomainEvent(
            Guid.NewGuid(), 
            restaurant.Id, 
            DateTime.UtcNow));
    
        return restaurant;
    }

    //Update();

    //Rate();

    //Close();

    //Open();

    //ReserveTable();

    //FreeTable();

    private Restaurant(RestaurantId id, 
        int numberOfTables, 
        RestaurantInformation restaurantInformation, 
        RestaurantLocalization restaurantLocalization,
        RestaurantSchedule restaurantSchedule,
        RestaurantContact restaurantContact,
        List<RestaurantRatingId> ratingIds, 
        List<RestaurantClient> restaurantClients,
        List<RestaurantTable> restaurantTables,
        DateTime postedAt)
    {
        Id = id;
        NumberOfTables = numberOfTables;
        RestaurantInformation = restaurantInformation;
        RestaurantLocalization = restaurantLocalization;
        RestaurantSchedule = restaurantSchedule;
        RestaurantContact = restaurantContact;

        PostedAt = postedAt;

        _ratingIds = ratingIds;
        _restaurantClients = restaurantClients;
        _restaurantTables = restaurantTables;

        RestaurantScheduleStatus = SetRestaurantScheduleStatus();
        AvailableTablesStatus = UpdateAvailableTablesStatus();
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
