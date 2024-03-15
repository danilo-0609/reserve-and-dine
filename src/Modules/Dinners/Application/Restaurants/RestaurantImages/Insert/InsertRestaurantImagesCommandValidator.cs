using FluentValidation;

namespace Dinners.Application.Restaurants.RestaurantImages.Insert;

internal sealed class InsertRestaurantImagesCommandValidator : AbstractValidator<InsertRestaurantImagesCommand>
{
    public InsertRestaurantImagesCommandValidator()
    {
        RuleFor(r => r.Id)
            .NotNull();

        RuleFor(r => r.FilePath)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.FormFile)
            .NotNull();
    }
}
