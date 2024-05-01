using FluentValidation;

namespace Dinners.Application.Restaurants.Rate.Upgrade;

internal sealed class UpgradeRateCommandValidator : AbstractValidator<UpgradeRateCommand>
{
    public UpgradeRateCommandValidator()
    {
        RuleFor(r => r.RateId)
            .NotNull();

        RuleFor(r => r.Stars)
            .NotEmpty()
            .NotNull()
            .LessThan(6)
            .GreaterThan(0);

        RuleFor(r => r.Comment)
            .NotNull();
    }
}
