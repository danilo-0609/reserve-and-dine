using Dinners.Domain.Reservations.Rules;
using ErrorOr;

namespace Dinners.Domain.Reservations.Errors;

public static class ReservationErrorsCodes
{
    public static Error NotFound =>
        Error.Validation("Reservation.NotFound", "Reservation was not found");

    public static Error CannotFinishIfStatusIsNotAsisting =>
        Error.Validation("Reservation.CannotFinishIfStatusIsNotAsisting", CannotFinishAReservationWhenReservationStatusIsNotVisitedRule.Message);
    
    public static Error CannotBeMadeWhenTableIsNotAvailable =>
        Error.Validation("Reservation.CannotBeMadeWhenTableIsNotAvailable", ReservationCannotBeMadeWhenTableIsNotAvailableRule.Message);

    public static Error ReservedWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved =>
        Error.Validation("Reservation.CannotReserveWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved", CannotReservedWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReservedRule.Message);

    public static Error CancelWhenReservationStatusIsNotPaidOrRequested =>
        Error.Validation("Reservation.CancelWhenReservationStatusIsNotPaidOrRequested", CannotCancelWhenReservationStatusIsNotPayedOrRequesteddRule.Message);

    public static Error AssistWhenReservationStatusIsNotPaid =>
        Error.Validation("Reservation.AssistWhenReservationStatusIsNotPaid", CannotAssistWhenReservationStatusIsNotPaidRule.Message);

    public static Error MenuNotFound =>
        Error.NotFound("Reservation.MenuNotFound", "The menu in the reservation was not found");

    public static Error CannotPayWhenReservationStatusIsNotRequested =>
        Error.Validation("Reservation.CannotPayWhenReservationStatusIsNotRequested", CannotPayWhenReservationStatusIsNotRequestedRule.Message);
}
