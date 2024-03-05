using FluentValidation;

namespace Dinners.Application.Reservations.Cancel;

internal sealed class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand>
{
    public CancelReservationCommandValidator()
    {
        RuleFor(r => r.ReservationId)
            .NotEmpty().NotNull();
    }
}
