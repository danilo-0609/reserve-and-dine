using FluentValidation;

namespace Dinners.Application.Reservations.Visit;

internal sealed class VisitReservationCommandValidator : AbstractValidator<VisitReservationCommand>
{
    public VisitReservationCommandValidator()
    {
        RuleFor(r => r.ReservationId)
            .NotEmpty().NotNull();
    }
}
