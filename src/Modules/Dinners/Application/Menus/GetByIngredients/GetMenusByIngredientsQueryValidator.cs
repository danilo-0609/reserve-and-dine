using FluentValidation;

namespace Dinners.Application.Menus.GetByIngredients;

internal sealed class GetMenusByIngredientsQueryValidator : AbstractValidator<GetMenusByIngredientsQuery>
{
    public GetMenusByIngredientsQueryValidator()
    {
        RuleFor(r => r.Ingredients)
            .NotEmpty().NotNull();
    }
}
