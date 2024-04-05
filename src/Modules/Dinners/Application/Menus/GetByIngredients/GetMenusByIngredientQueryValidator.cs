using FluentValidation;

namespace Dinners.Application.Menus.GetByIngredients;

internal sealed class GetMenusByIngredientQueryValidator : AbstractValidator<GetMenusByIngredientQuery>
{
    public GetMenusByIngredientQueryValidator()
    {
        RuleFor(r => r.Ingredient)
            .NotEmpty()
            .NotNull();
    }
}
