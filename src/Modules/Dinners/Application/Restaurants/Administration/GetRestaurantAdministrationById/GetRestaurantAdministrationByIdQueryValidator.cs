using FluentValidation;

namespace Dinners.Application.Restaurants.Administration.GetRestaurantAdministrationById;

internal sealed class GetRestaurantAdministrationByIdQueryValidator : AbstractValidator<GetRestaurantAdministrationByIdQuery>
{
    public GetRestaurantAdministrationByIdQueryValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty().NotNull();
    }
}
