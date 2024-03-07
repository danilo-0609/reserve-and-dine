using FluentValidation;

namespace Dinners.Application.Restaurants.Administration.Add;

internal sealed class AddAdministrationCommandValidator : AbstractValidator<AddAdministrationCommand>
{
    public AddAdministrationCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty().NotNull();

        RuleFor(r => r.AdministratorTitle)
            .NotEmpty().NotNull();
    
        RuleFor(r => r.Name)
            .NotEmpty().NotNull();
    }
}
