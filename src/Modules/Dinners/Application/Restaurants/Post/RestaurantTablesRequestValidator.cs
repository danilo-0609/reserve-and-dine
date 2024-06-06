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
    }

    private enum MenuType
    {
        Breakfast,
        Lunch,
        Dinner
    }
}
