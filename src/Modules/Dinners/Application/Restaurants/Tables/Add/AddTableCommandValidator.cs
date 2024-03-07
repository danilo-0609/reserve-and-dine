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
    }
}
