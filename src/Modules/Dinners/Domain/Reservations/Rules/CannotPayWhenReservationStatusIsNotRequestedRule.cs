using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class CannotPayWhenReservationStatusIsNotRequestedRule : IBusinessRule
{
    private readonly ReservationStatus _reservationStatus;

    public CannotPayWhenReservationStatusIsNotRequestedRule(ReservationStatus reservationStatus)
    {
        _reservationStatus = reservationStatus;
    }

    public Error Error => ReservationErrorsCodes.CannotPayWhenReservationStatusIsNotRequested;

    public bool IsBroken() => _reservationStatus != ReservationStatus.Requested;

    public static string Message => "Cannot pay when reservation status is not requested";
}
