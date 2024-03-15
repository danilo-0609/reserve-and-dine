using FluentValidation;

namespace Dinners.Application.Restaurants.RestaurantImages.Get;

internal sealed class GetRestaurantImagesQueryValidator : AbstractValidator<GetRestaurantImagesQuery>
{
    public GetRestaurantImagesQueryValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty();
    }
}
