using FluentValidation;
using System;
namespace Dinners.Application.Restaurants.Rate.Upgrade;

internal sealed class UpgradeRateCommandValidator : AbstractValidator<UpgradeRateCommand>
{
    public UpgradeRateCommandValidator()
    {
        RuleFor(r => r.RateId)
            .NotNull();

        RuleFor(r => r.Stars)
            .NotEmpty().NotNull();

        RuleFor(r => r.Comment)
            .NotNull();
    }
}
