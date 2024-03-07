using FluentValidation;

namespace Dinners.Application.Restaurants.GetRestaurantAdministrationById;

internal sealed class GetRestaurantAdministrationByIdQueryValidator : AbstractValidator<GetRestaurantAdministrationByIdQuery>
{
    public GetRestaurantAdministrationByIdQueryValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty().NotNull();
    }
}
