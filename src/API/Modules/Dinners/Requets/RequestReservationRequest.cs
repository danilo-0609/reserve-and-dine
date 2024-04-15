namespace API.Modules.Dinners.Requets;

public sealed record RequestReservationRequest(int ReservedTable,
        DateTime StartReservationDateTime,
        DateTime EndReservationDateTime,
        Guid RestaurantId,
        string Name,
        int NumberOfAttendees,
        List<Guid> MenuIds);