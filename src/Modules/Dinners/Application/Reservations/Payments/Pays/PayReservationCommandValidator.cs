using FluentValidation;

namespace Dinners.Application.Reservations.Payments.Pays;

internal sealed class PayReservationCommandValidator : AbstractValidator<PayReservationCommand>
{
    public PayReservationCommandValidator()
    {
        RuleFor(r => r.ReservationId)
            .NotEmpty().NotNull();
    }
}
