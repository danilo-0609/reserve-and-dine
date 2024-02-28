using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class CannotReservedWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReservedRule : IBusinessRule
{
    private readonly int _numberOfAttendees;
    private readonly int _numberOfSeats;

    public CannotReservedWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReservedRule(int numberOfAttendees, int numberOfSeats)
    {
        _numberOfAttendees = numberOfAttendees;
        _numberOfSeats = numberOfSeats;
    }

    public Error Error => ReservationErrorsCodes.ReservedWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved;

    public bool IsBroken() => _numberOfAttendees > _numberOfSeats;

    public static string Message => "Cannot reserved when number of attendees is greater than seats of table reserved";
}
