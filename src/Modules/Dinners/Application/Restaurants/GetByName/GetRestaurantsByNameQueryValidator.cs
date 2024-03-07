using FluentValidation;

namespace Dinners.Application.Restaurants.GetByName;

internal sealed class GetRestaurantsByNameQueryValidator : AbstractValidator<GetRestaurantsByNameQuery>
{
    public GetRestaurantsByNameQueryValidator()
    {
        RuleFor(r => r.Name)
            .NotNull().NotEmpty();
    }
}
