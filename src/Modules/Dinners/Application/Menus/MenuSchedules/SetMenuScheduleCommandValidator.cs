using FluentValidation;

namespace Dinners.Application.Menus.MenuSchedules;

internal sealed class SetMenuScheduleCommandValidator : AbstractValidator<SetMenuScheduleCommand>
{
    public SetMenuScheduleCommandValidator()
    {
        RuleFor(r => r.DayOfWeeks)
            .NotNull().NotEmpty();

        RuleFor(r => r.Open)
            .NotEmpty().NotNull();

        RuleFor(r => r.Close)
            .NotEmpty().NotNull();
    }
}
