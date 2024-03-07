using FluentValidation;

namespace Dinners.Application.Restaurants.Tables.Get;

internal sealed class GetTablesByRestaurantIdQueryValidator : AbstractValidator<GetTablesByRestaurantIdQuery>
{
    public GetTablesByRestaurantIdQueryValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotNull().NotEmpty();
    }
}
