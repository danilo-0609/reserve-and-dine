using Dinners.Domain.Reservations.ReservationsPayments.Rules;
using ErrorOr;

namespace Dinners.Domain.Reservations.ReservationsPayments.Errors;

public static class ReservationPaymentsErrorsCodes
{
    public static Error NotFound =>
        Error.NotFound("ReservationPayment.NotFound", "Reservation payment was not found");

    public static Error CannotPayWhenStatusIsNotRequested =>
        Error.Validation("Payment.CannotPayWhenStatusIsNotConfirmed", PaymentCannotBeMadeWhenReservationStatusIsNotRequestedRule.Message);
}
