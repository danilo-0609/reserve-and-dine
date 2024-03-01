using FluentValidation;

namespace Dinners.Application.Menus.Review;

internal sealed class ReviewMenuCommandValidator : AbstractValidator<ReviewMenuCommand>
{
    public ReviewMenuCommandValidator()
    {
        RuleFor(r => r.MenuId)
            .NotEmpty().NotNull();

        RuleFor(r => r.Rate)
            .NotEmpty().NotNull();

        RuleFor(r => r.MenuId)
            .NotNull();
    }
}
