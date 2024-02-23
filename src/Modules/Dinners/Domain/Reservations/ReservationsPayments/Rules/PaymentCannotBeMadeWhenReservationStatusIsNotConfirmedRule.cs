using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.ReservationsPayments.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.ReservationsPayments.Rules;

internal sealed class PaymentCannotBeMadeWhenReservationStatusIsNotConfirmedRule : IBusinessRule
{
    private readonly ReservationStatus _reservationStatus;

    public PaymentCannotBeMadeWhenReservationStatusIsNotConfirmedRule(ReservationStatus reservationStatus)
    {
        _reservationStatus = reservationStatus;
    }

    public Error Error => ReservationPaymentsErrorsCodes.CannotPayWhenStatusIsNotConfirmed;

    public bool IsBroken() => _reservationStatus != ReservationStatus.Confirmed;

    public static string Message => "Payment cannot be made when reservation status is not confirmed";
}
