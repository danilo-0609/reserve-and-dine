using FluentValidation;

namespace Dinners.Application.Restaurants.GetByLocalization;

internal sealed class GetRestaurantsByLocalizationQueryValidator : AbstractValidator<GetRestaurantsByLocalizationQuery>
{
    public GetRestaurantsByLocalizationQueryValidator()
    {
        RuleFor(r => r.Country)
            .NotNull().NotEmpty();

        RuleFor(r => r.Region)
            .NotEmpty().NotNull();

        RuleFor(r => r.City)
            .NotEmpty().NotNull();  

        RuleFor(r => r.Neighborhood)
            .NotEmpty().NotNull();  
    }
}
