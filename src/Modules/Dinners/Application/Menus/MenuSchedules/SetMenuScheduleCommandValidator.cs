using FluentValidation;

namespace Dinners.Application.Menus.MenuSchedules;

internal sealed class SetMenuScheduleCommandValidator : AbstractValidator<SetMenuScheduleCommand>
{
    public SetMenuScheduleCommandValidator()
    {
        RuleFor(r => r.Day)
            .NotNull().NotEmpty();

        RuleFor(r => r.Start)
            .NotEmpty().NotNull();

        RuleFor(r => r.End)
            .NotEmpty().NotNull();
    }
}
