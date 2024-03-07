using FluentValidation;

namespace Dinners.Application.Restaurants.Administration.Update;

internal sealed class UpdateAdministrationCommandValidator : AbstractValidator<UpdateAdministrationCommand>
{
    public UpdateAdministrationCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.AdministratorTitle)
            .NotEmpty()
            .NotNull();
    
        RuleFor(r => r.Name)
            .NotEmpty()
            .NotNull();
    }
}
