namespace API.Modules.Dinners.Requets;

public sealed record RequestReservationRequest(int ReservedTable,
        string Start,
        string End,
        DateTime ReservationDateTime,
        Guid RestaurantId,
        string Name,
        int NumberOfAttendees,
        List<Guid> MenuIds);