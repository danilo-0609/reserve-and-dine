using FluentValidation;

namespace Dinners.Application.Restaurants.RestaurantImages.Remove;

internal sealed class RemoveRestaurantImageCommandValidator : AbstractValidator<RemoveRestaurantImageCommand>
{
    public RemoveRestaurantImageCommandValidator()
    {
        RuleFor(r => r.Id)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.imageId)
            .NotNull()
            .NotEmpty();
    }
}
