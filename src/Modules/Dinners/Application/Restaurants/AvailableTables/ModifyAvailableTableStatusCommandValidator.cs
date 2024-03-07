using FluentValidation;

namespace Dinners.Application.Restaurants.AvailableTables;

internal sealed class ModifyAvailableTableStatusCommandValidator : AbstractValidator<ModifyAvailableTableStatusCommand>
{
    public ModifyAvailableTableStatusCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotNull().NotEmpty();

        RuleFor(r => r.AvailableTableStatus)
            .NotNull().NotEmpty()
            .Equal("Availables")
            .Equal("Few")
            .Equal("NoAvailables");
    }
}
