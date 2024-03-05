using FluentValidation;

namespace Dinners.Application.Reservations.GetById;

internal sealed class GetReservationByIdQueryValidator : AbstractValidator<GetReservationByIdQuery>
{
    public GetReservationByIdQueryValidator()
    {
        RuleFor(r => r.ReservationId)
            .NotNull().NotEmpty();
    }
}
