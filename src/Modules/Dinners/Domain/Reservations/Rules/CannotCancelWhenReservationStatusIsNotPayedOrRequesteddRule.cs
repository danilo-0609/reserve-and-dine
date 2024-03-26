using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class CannotCancelWhenReservationStatusIsNotPayedOrRequesteddRule : IBusinessRule
{
    private readonly ReservationStatus _reservationStatus;

    public CannotCancelWhenReservationStatusIsNotPayedOrRequesteddRule(ReservationStatus reservationStatus)
    {
        _reservationStatus = reservationStatus;
    }

    public Error Error => ReservationErrorsCodes.CancelWhenReservationStatusIsAssisted;

    public bool IsBroken() => !CanCancelReservation(_reservationStatus);

    private bool CanCancelReservation(ReservationStatus reservationStatus)
    {
        if (reservationStatus == ReservationStatus.Paid || reservationStatus == ReservationStatus.Requested)
        {
            return true;
        }

        return false;
    }

    public static string Message => "Cannot cancel when reservation status is assisted";
}
