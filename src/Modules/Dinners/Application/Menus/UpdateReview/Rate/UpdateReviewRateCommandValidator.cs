using FluentValidation;

namespace Dinners.Application.Menus.UpdateReview.Rate;

internal sealed class UpdateReviewRateCommandValidator : AbstractValidator<UpdateReviewRateCommand>
{
    public UpdateReviewRateCommandValidator()
    {
        RuleFor(r => r.MenuReviewId)
            .NotEmpty().NotNull();

        RuleFor(r => r.Rate)
            .NotEmpty().NotNull();
    }
}
