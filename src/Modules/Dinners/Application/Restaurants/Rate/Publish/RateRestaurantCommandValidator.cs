using FluentValidation;

namespace Dinners.Application.Restaurants.Rate.Publish;

internal sealed class RateRestaurantCommandValidator : AbstractValidator<RateRestaurantCommand>
{
    public RateRestaurantCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty().NotNull();

        RuleFor(r => r.Stars)
            .NotEmpty().NotNull();

        RuleFor(r => r.Comment)
            .NotNull();
    }
}
