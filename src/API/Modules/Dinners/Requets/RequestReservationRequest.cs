namespace API.Modules.Dinners.Requets;

public sealed record RequestReservationRequest(int ReservedTable,
        decimal Price,
        string Currency,
        DateTime StartReservationDateTime,
        DateTime EndReservationDateTime,
        Guid RestaurantId,
        string Name,
        int NumberOfAttendees,
        List<Guid> MenuIds);