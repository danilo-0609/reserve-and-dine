using BuildingBlocks.Domain.Entities;
using Dinners.Domain.Common;

namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed class ReservedHour : Entity<ReservedHourId, Guid>
{
    public new ReservedHourId Id { get; private set; }

    public RestaurantId RestaurantId { get; private set; }

    public RestaurantTableId RestaurantTableId { get; private set; }

    public DateTime ReservationDateTime { get; private set; }

    public TimeRange ReservationTimeRange { get; private set; }
       
    public ReservedHour(ReservedHourId id, RestaurantId restaurantId, RestaurantTableId restaurantTableId, DateTime reservationDateTime, TimeRange reservationTimeRange)
    {
        Id = id;
        RestaurantId = restaurantId;
        RestaurantTableId = restaurantTableId;
        ReservationDateTime = reservationDateTime;
        ReservationTimeRange = reservationTimeRange;
    }

    private ReservedHour() { }  
}
