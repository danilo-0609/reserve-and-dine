using BuildingBlocks.Domain.Entities;
using Dinners.Domain.Common;

namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed class RestaurantTable : Entity<RestaurantTableId, Guid>
{
    private List<ReservedHour> _reservedHours = new();

    public new RestaurantTableId Id { get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public int Number { get; private set; }

    public int Seats { get; private set; }

    public Price Price { get; private set; }

    public bool IsPremium { get; private set; }

    public bool IsOccupied { get; private set; }

    public IReadOnlyList<ReservedHour> ReservedHours => _reservedHours.AsReadOnly();

    public static RestaurantTable Create(RestaurantId restaurantId,
        int number,
        int seats,
        bool isPremium,
        Price price,
        List<ReservedHour> reservedHours)
    {
        return new RestaurantTable(RestaurantTableId.CreateUnique(), 
            restaurantId, 
            number, 
            seats, 
            isPremium, 
            price,
            reservedHours);
    }

    public RestaurantTable Upgrade(int number,
        int seats,
        bool isPremium,
        Price price)
    {
        return new RestaurantTable(base.Id, RestaurantId, number, seats, isPremium, price, _reservedHours);
    }

    public void CancelReservation(DateTime reservedTime)
    {
        _reservedHours.RemoveAll(r => r.ReservationDateTime == reservedTime); 
    }

    public void Reserve(DateTime reservedTime, TimeRange reservationTimeRange)
    {
        _reservedHours.Add(new ReservedHour(ReservedHourId.CreateUnique(), RestaurantId, base.Id, reservedTime, reservationTimeRange));
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
        RestaurantTableId restaurantTableId,
        RestaurantId restaurantId,
        int number, 
        int seats, 
        bool isPremium, 
        Price price,
        List<ReservedHour> reservedHours)
    {
        Id = restaurantTableId;
        RestaurantId = restaurantId;
        Number = number;
        Seats = seats;
        IsPremium = isPremium;
        IsOccupied = false;

        Price = price;

        _reservedHours = reservedHours;
    }

    private RestaurantTable() { }
}
