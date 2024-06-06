using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class CannotCancelWhenReservationStatusIsNotRequestedRule : IBusinessRule
{
    private readonly ReservationStatus _reservationStatus;

    public CannotCancelWhenReservationStatusIsNotRequestedRule(ReservationStatus reservationStatus)
    {
        _reservationStatus = reservationStatus;
    }

    public Error Error => ReservationErrorsCodes.CancelWhenReservationStatusIsNotPaidOrRequested;

    public bool IsBroken() => !CanCancelReservation(_reservationStatus);

    private bool CanCancelReservation(ReservationStatus reservationStatus)
    {
        if ( reservationStatus == ReservationStatus.Requested)
        {
            return true;
        }

        return false;
    }

    public static string Message => "Cannot cancel when reservation status is assisted";
}
