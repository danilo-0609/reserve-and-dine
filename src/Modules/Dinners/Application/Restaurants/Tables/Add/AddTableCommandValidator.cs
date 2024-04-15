using FluentValidation;

namespace Dinners.Application.Restaurants.Tables.Add;

internal sealed class AddTableCommandValidator : AbstractValidator<AddTableCommand>
{
    public AddTableCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
           .NotEmpty().NotNull();

        RuleFor(r => r.Number)
            .NotEmpty().NotNull();

        RuleFor(r => r.Seats)
            .NotEmpty().NotNull();


        RuleFor(r => r.IsPremium)
            .NotEmpty().NotNull();

        RuleFor(r => r.Price)
            .LessThanOrEqualTo(2_000_000);

        RuleFor(r => r.Currency)
            .NotEmpty()
            .Must(value => Enum.TryParse(value, out Currency currency))
            .WithMessage("Currency must be COP or USD");
    }

    private enum MenuType
    {
        Breakfast,
        Lunch,
        Dinner
    }

    private enum Currency
    {
        USD,
        COP
    }
}
