using Dinners.Application.Restaurants.Post.Requests;
using FluentValidation;

namespace Dinners.Application.Restaurants.Post;

internal sealed class RestaurantTablesRequestValidator : AbstractValidator<RestaurantTableRequest>
{
    public RestaurantTablesRequestValidator()
    {
        RuleFor(r => r.Number)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.Seats)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.IsPremium)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.Price)
            .NotEmpty()
            .NotNull();

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
