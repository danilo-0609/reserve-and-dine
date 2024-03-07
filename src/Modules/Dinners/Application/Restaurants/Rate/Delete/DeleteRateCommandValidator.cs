using FluentValidation;

namespace Dinners.Application.Restaurants.Rate.Delete;

internal sealed class DeleteRateCommandValidator : AbstractValidator<DeleteRateCommand>
{
    public DeleteRateCommandValidator()
    {
        RuleFor(r => r.RatingId)
            .NotNull();
    }
}
