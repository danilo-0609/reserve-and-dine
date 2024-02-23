using Dinners.Domain.Reservations.ReservationsPayments.Rules;
using ErrorOr;

namespace Dinners.Domain.Reservations.ReservationsPayments.Errors;

public static class ReservationPaymentsErrorsCodes
{
    public static Error CannotPayWhenStatusIsNotConfirmed =>
        Error.Validation("Payment.CannotPayWhenStatusIsNotConfirmed", PaymentCannotBeMadeWhenReservationStatusIsNotConfirmedRule.Message);
}
