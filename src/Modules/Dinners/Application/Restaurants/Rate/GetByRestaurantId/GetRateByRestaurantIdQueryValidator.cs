using FluentValidation;

namespace Dinners.Application.Restaurants.Rate.GetByRestaurantId;

internal sealed class GetRateByRestaurantIdQueryValidator : AbstractValidator<GetRateByRestaurantIdQuery>
{
    public GetRateByRestaurantIdQueryValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotNull();    
    }
}
