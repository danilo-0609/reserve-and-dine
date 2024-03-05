using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class CannotFinishAReservationWhenReservationStatusIsNotVisitedRule : IBusinessRule
{
    private readonly ReservationStatus _reservationStatus;

    public CannotFinishAReservationWhenReservationStatusIsNotVisitedRule(ReservationStatus reservationStatus)
    {
        _reservationStatus = reservationStatus;
    }

    public Error Error => ReservationErrorsCodes.CannotFinishIfStatusIsNotAsisting;

    public bool IsBroken() => _reservationStatus != ReservationStatus.Visiting;

    public static string Message => "Cannot finish a reservation when reservation status is not asisting";
}
