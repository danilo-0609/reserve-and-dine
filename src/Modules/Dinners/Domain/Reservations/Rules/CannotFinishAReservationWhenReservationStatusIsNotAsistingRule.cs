using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class CannotFinishAReservationWhenReservationStatusIsNotAsistingRule : IBusinessRule
{
    private readonly ReservationStatus _reservationStatus;

    public CannotFinishAReservationWhenReservationStatusIsNotAsistingRule(ReservationStatus reservationStatus)
    {
        _reservationStatus = reservationStatus;
    }

    public Error Error => ReservationErrorsCodes.CannotFinishIfStatusIsNotAsisting;

    public bool IsBroken() => _reservationStatus != ReservationStatus.Asisting;

    public static string Message => "Cannot finish a reservation when reservation status is not asisting";
}
