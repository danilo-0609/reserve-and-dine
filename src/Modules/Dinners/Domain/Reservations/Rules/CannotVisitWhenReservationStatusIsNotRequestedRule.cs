using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class CannotVisitWhenReservationStatusIsNotRequestedRule : IBusinessRule
{
    private readonly ReservationStatus _reservationStatus;

    public CannotVisitWhenReservationStatusIsNotRequestedRule(ReservationStatus reservationStatus)
    {
        _reservationStatus = reservationStatus;
    }

    public Error Error => ReservationErrorsCodes.CannotVisitWhenReservationStatusIsNotRequested;

    public bool IsBroken() => _reservationStatus != ReservationStatus.Requested;

    public static string Message => "Cannot visit when reservation status is not requested";
}