using Dinners.Domain.Common;

namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed record RestaurantTable
{
    private readonly Dictionary<DateTime, TimeRange> _reservedHours = new();

    public int Number { get; private set; }

    public int Seats { get; private set; }

    public bool IsPremium { get; private set; }

    public bool IsReserved { get; private set; }

    public IReadOnlyDictionary<DateTime, TimeRange> ReservedHours => _reservedHours.AsReadOnly();

    public static RestaurantTable Create(int number,
        int seats,
        bool isPremium,
        bool isReserved,
        Dictionary<DateTime, TimeRange> reservedHours)
    {
        return new RestaurantTable(number, seats, isPremium, isReserved, reservedHours);
    }

    public void Reserve(DateTime reservedTime, TimeRange reservationTimeRange)
    {
        _reservedHours.Add(reservedTime, reservationTimeRange);

        IsReserved = true;
    }

    public void FreeTable()
    {
        IsReserved = false;
    }


    private RestaurantTable(int number, int seats, bool isPremium, bool isReserved, Dictionary<DateTime, TimeRange> reservedHours)
    {
        Number = number;
        Seats = seats;
        IsPremium = isPremium;
        IsReserved = isReserved;

        _reservedHours = reservedHours;
    }

    private RestaurantTable() { }
}
