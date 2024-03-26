using Dinners.Application.Restaurants.Post.Requests;
using FluentValidation;

namespace Dinners.Application.Restaurants.Post;

internal sealed class RestaurantScheduleRequestValidator : AbstractValidator<RestaurantScheduleRequest>
{
    public RestaurantScheduleRequestValidator()
    {
        RuleFor(r => r.Day)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.Start)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.End)
            .NotEmpty()
            .NotNull();
    }
}
