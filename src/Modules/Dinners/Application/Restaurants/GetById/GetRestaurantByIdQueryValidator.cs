using FluentValidation;

namespace Dinners.Application.Restaurants.GetById;

internal sealed class GetRestaurantByIdQueryValidator : AbstractValidator<GetRestaurantByIdQuery>
{
    public GetRestaurantByIdQueryValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty().NotNull();
    }
}
