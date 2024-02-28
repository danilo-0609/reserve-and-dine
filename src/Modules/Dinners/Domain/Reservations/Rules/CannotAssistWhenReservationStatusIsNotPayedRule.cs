using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class CannotAssistWhenReservationStatusIsNotPayedRule : IBusinessRule
{
    private readonly ReservationStatus _reservationStatus;

    public CannotAssistWhenReservationStatusIsNotPayedRule(ReservationStatus reservationStatus)
    {
        _reservationStatus = reservationStatus;
    }

    public Error Error => ReservationErrorsCodes.AssistWhenReservationStatusIsNotPayed;

    public bool IsBroken() => _reservationStatus != ReservationStatus.Payed;

    public static string Message => "Cannot assit when reservation status is not payed";
}
