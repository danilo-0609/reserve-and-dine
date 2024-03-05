using FluentValidation;

namespace Dinners.Application.Reservations.Refunds;

internal sealed class GetRefundByIdQueryValidator : AbstractValidator<GetRefundByIdQuery>
{
    public GetRefundByIdQueryValidator()
    {
        RuleFor(r => r.RefundId)
            .NotEmpty().NotNull();
    }
}
