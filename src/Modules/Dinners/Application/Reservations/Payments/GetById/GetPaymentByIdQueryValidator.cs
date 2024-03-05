using FluentValidation;

namespace Dinners.Application.Reservations.Payments.GetById;

internal sealed class GetPaymentByIdQueryValidator : AbstractValidator<GetPaymentByIdQuery>
{
    public GetPaymentByIdQueryValidator()
    {
        RuleFor(r => r.PaymentId)
            .NotNull().NotEmpty();
    }
}
