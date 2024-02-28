using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.ReservationsPayments.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.ReservationsPayments.Rules;

internal sealed class PaymentCannotBeMadeWhenReservationStatusIsNotRequestedRule : IBusinessRule
{
    private readonly ReservationStatus _reservationStatus;

    public PaymentCannotBeMadeWhenReservationStatusIsNotRequestedRule(ReservationStatus reservationStatus)
    {
        _reservationStatus = reservationStatus;
    }

    public Error Error => ReservationPaymentsErrorsCodes.CannotPayWhenStatusIsNotRequested;

    public bool IsBroken() => _reservationStatus != ReservationStatus.Requested;

    public static string Message => "Payment cannot be made when reservation status is not requested";
}
