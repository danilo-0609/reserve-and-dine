using FluentValidation;

namespace Dinners.Application.Restaurants.Tables.Update;

internal sealed class UpgradeTableCommandValidator : AbstractValidator<UpgradeTableCommand>
{
    public UpgradeTableCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty().NotNull();

        RuleFor(r => r.Number)
            .NotEmpty().NotNull();

        RuleFor(r => r.Seats)
            .NotEmpty().NotNull();


        RuleFor(r => r.IsPremium)
            .NotEmpty().NotNull();
    }
}
