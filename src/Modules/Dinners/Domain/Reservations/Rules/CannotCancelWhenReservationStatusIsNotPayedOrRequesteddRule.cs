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

    public Error Error => ReservationErrorsCodes.CancelWhenReservationStatusIsAsisted;

    public bool IsBroken() => _reservationStatus != ReservationStatus.Payed && _reservationStatus != ReservationStatus.Requested;

    public static string Message => "Cannot cancel when reservation status is asisted";
}
