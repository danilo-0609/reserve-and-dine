using FluentValidation;

namespace Dinners.Application.Restaurants.ModifySchedule;

internal sealed class ModifyRestaurantScheduleCommandValidator : AbstractValidator<ModifyRestaurantScheduleCommand>
{
    public ModifyRestaurantScheduleCommandValidator()
    {
        RuleFor(r => r.RestaurantId)
            .NotEmpty().NotNull();

        RuleFor(r => r.Days)
            .NotEmpty().NotNull();

        RuleFor(r => r.Start)
            .NotEmpty().NotNull();

        RuleFor(r => r.End)
            .NotEmpty().NotNull();
    }
}
