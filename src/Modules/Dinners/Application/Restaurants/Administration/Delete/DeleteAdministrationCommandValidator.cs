using FluentValidation;

namespace Dinners.Application.Restaurants.Administration.Delete;

internal sealed class DeleteAdministrationCommandValidator : AbstractValidator<DeleteAdministrationCommand>
{
    public DeleteAdministrationCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty().NotNull();
    }
}
