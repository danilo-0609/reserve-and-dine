using Dinners.Domain.Common;

namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed record RestaurantTable
{
    private readonly List<ReservedHour> _reservedHours = new();

    public RestaurantId RestaurantId { get; private set; }

    public int Number { get; private set; }

    public int Seats { get; private set; }

    public bool IsPremium { get; private set; }

    public bool IsOccupied { get; private set; }

    public IReadOnlyList<ReservedHour> ReservedHours => _reservedHours.AsReadOnly();

    public static RestaurantTable Create(RestaurantId restaurantId,
        int number,
        int seats,
        bool isPremium,
        List<ReservedHour> reservedHours)
    {
        return new RestaurantTable(restaurantId, number, seats, isPremium, reservedHours);
    }

    public RestaurantTable Upgrade(int number,
        int seats,
        bool isPremium)
    {
        return new RestaurantTable(RestaurantId, number, seats, isPremium, _reservedHours);
    }

    public void CancelReservation(DateTime reservedTime)
    {
        var reservedHour = _reservedHours.Where(r => r.ReservationDateTime == reservedTime).Single();

        _reservedHours.Remove(reservedHour); 
    }

    public void Reserve(DateTime reservedTime, TimeRange reservationTimeRange)
    {
        _reservedHours.Add(new ReservedHour(reservedTime, reservationTimeRange, Number));
    }

    public void OccupyTable()
    {
        IsOccupied = true;
    }

    public void FreeTable()
    {
        IsOccupied = false;
    }


    private RestaurantTable(
        RestaurantId restaurantId,
        int number, 
        int seats, 
        bool isPremium, 
        List<ReservedHour> reservedHours)
    {
        RestaurantId = restaurantId;
        Number = number;
        Seats = seats;
        IsPremium = isPremium;
        IsOccupied = false;

        _reservedHours = reservedHours;
    }

    private RestaurantTable() { }
}
