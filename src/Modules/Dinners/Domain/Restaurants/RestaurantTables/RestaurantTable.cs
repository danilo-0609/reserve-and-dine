using Dinners.Domain.Common;

namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed record RestaurantTable
{
    private readonly Dictionary<DateTime, TimeRange> _reservedHours = new();

    public int Number { get; private set; }

    public int Seats { get; private set; }

    public bool IsPremium { get; private set; }

    public bool IsOccuppied { get; private set; }

    public IReadOnlyDictionary<DateTime, TimeRange> ReservedHours => _reservedHours.AsReadOnly();

    public static RestaurantTable Create(int number,
        int seats,
        bool isPremium,
        Dictionary<DateTime, TimeRange> reservedHours)
    {
        return new RestaurantTable(number, seats, isPremium, reservedHours);
    }

    public RestaurantTable Upgrade(int number,
        int seats,
        bool isPremium)
    {
        return new RestaurantTable(number, seats, isPremium, _reservedHours);
    }

    public void CancelReservation(DateTime reservedTime)
    {
        _reservedHours.Remove(reservedTime);
    }

    public void Reserve(DateTime reservedTime, TimeRange reservationTimeRange)
    {
        _reservedHours.Add(reservedTime, reservationTimeRange);
    }

    public void OccupyTable()
    {
        IsOccuppied = true;
    }

    public void FreeTable()
    {
        IsOccuppied = false;
    }


    private RestaurantTable(int number, int seats, bool isPremium, Dictionary<DateTime, TimeRange> reservedHours)
    {
        Number = number;
        Seats = seats;
        IsPremium = isPremium;
        IsOccuppied = false;

        _reservedHours = reservedHours;
    }

    private RestaurantTable() { }
}
