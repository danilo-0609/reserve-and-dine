using FluentValidation;

namespace Dinners.Application.Restaurants.Post;

internal sealed class PostRestaurantCommandValidator : AbstractValidator<PostRestaurantCommand>
{
    public PostRestaurantCommandValidator()
    {
        RuleFor(r => r.RestaurantInformation)
            .NotEmpty().NotNull();

        RuleFor(r => r.RestaurantLocalization)
            .NotEmpty().NotNull();

        RuleFor(r => r.RestaurantSchedule)
            .NotEmpty().NotNull();

        RuleFor(r => r.RestaurantTables)
            .NotEmpty().NotNull();
        
        RuleFor(r => r.RestaurantAdministrations)
            .NotEmpty().NotNull();  

        RuleFor(r => r.RestaurantContact)
            .NotEmpty().NotNull();
    }
}
