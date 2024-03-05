using FluentValidation;

namespace Dinners.Application.Reservations.Finish;

internal sealed class FinishReservationCommandValidator : AbstractValidator<FinishReservationCommand>
{
    public FinishReservationCommandValidator()
    {
        RuleFor(r => r.ReservationId)
            .NotNull().NotEmpty();
    }
}
