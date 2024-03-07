using FluentValidation;

namespace Dinners.Application.Restaurants.Open;

internal sealed class OpenRestaurantCommandValidator : AbstractValidator<OpenRestaurantCommand>
{
    public OpenRestaurantCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty().NotNull();
    }
}
