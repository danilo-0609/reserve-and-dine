using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class CannotAssistWhenReservationStatusIsNotPaidRule : IBusinessRule
{
    private readonly ReservationStatus _reservationStatus;

    public CannotAssistWhenReservationStatusIsNotPaidRule(ReservationStatus reservationStatus)
    {
        _reservationStatus = reservationStatus;
    }

    public Error Error => ReservationErrorsCodes.AssistWhenReservationStatusIsNotPaid;

    public bool IsBroken() => _reservationStatus != ReservationStatus.Paid;

    public static string Message => "Cannot assit when reservation status is not paid";
}
