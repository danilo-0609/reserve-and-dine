using FluentValidation;

namespace Dinners.Application.Restaurants.RestaurantImages.Get;

internal sealed class GetRestaurantImageQueryValidator : AbstractValidator<GetRestaurantImageQuery>
{
    public GetRestaurantImageQueryValidator()
    {
        RuleFor(r => r.Id)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.imageId)
            .NotNull()
            .NotEmpty();
    }
}
