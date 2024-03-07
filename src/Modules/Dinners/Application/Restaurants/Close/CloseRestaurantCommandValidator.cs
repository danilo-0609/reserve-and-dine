using FluentValidation;

namespace Dinners.Application.Restaurants.Close;

internal sealed class CloseRestaurantCommandValidator : AbstractValidator<CloseRestaurantCommand>
{
    public CloseRestaurantCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty().NotNull();
    }
}
