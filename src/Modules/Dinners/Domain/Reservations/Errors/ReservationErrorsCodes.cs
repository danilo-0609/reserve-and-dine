using Dinners.Domain.Reservations.Rules;
using ErrorOr;

namespace Dinners.Domain.Reservations.Errors;

public static class ReservationErrorsCodes
{
    public static Error NotFound =>
        Error.NotFound("Reservation.NotFound", "Reservation was not found");

    public static Error CannotFinishIfStatusIsNotAssisting =>
        Error.Validation("Reservation.CannotFinishIfStatusIsNotAssisting", CannotFinishAReservationWhenReservationStatusIsNotVisitedRule.Message);
    
    public static Error CannotBeMadeWhenTableIsNotAvailable =>
        Error.Validation("Reservation.CannotBeMadeWhenTableIsNotAvailable", ReservationCannotBeMadeWhenTableIsNotAvailableRule.Message);

    public static Error ReservedWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved =>
        Error.Validation("Reservation.CannotReserveWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReserved", CannotReservedWhenNumberOfAttendeesIsGreaterThanSeatsOfTableReservedRule.Message);

    public static Error CancelWhenReservationStatusIsNotPaidOrRequested =>
        Error.Validation("Reservation.CancelWhenReservationStatusIsNotPaidOrRequested", CannotCancelWhenReservationStatusIsNotRequestedRule.Message);

    public static Error CannotAssistToReservationOutOfTheRequestedTime =>
        Error.Validation("Reservation.MustAssistToReservationInTheRequestedTime", MustAssistToReservationInTheRequestedTimeRule.Message);

    public static Error CannotVisitWhenReservationStatusIsNotRequested =>
        Error.Validation("Reservation.CannotVisitWhenReservationStatusIsNotRequested", CannotVisitWhenReservationStatusIsNotRequestedRule.Message);

    public static Error MenuNotFound =>
        Error.NotFound("Reservation.MenuNotFound", "The menu in the reservation was not found");
}
