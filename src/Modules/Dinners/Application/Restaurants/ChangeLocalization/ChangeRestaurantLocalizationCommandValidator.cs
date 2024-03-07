using FluentValidation;

namespace Dinners.Application.Restaurants.ChangeLocalization;

internal sealed class ChangeRestaurantLocalizationCommandValidator : AbstractValidator<ChangeRestaurantLocalizationCommand>
{
    public ChangeRestaurantLocalizationCommandValidator()
    {
        RuleFor(r => r.Country)
            .NotEmpty().NotNull();

        RuleFor(r => r.City)
            .NotEmpty().NotNull();

        RuleFor(r => r.Region)
            .NotEmpty().NotNull();

        RuleFor(r => r.Neighborhood)
            .NotEmpty().NotNull();

        RuleFor(r => r.Address)
            .NotEmpty().NotNull();

        RuleFor(r => r.Country)
            .NotNull();
    }
}
