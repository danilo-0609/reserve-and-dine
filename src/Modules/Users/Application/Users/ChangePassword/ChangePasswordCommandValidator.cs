using FluentValidation;

namespace Users.Application.Users.ChangePassword;

internal sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.OldPassword)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.NewPassword)
            .NotEmpty()
            .NotNull()
            .MinimumLength(20);
    }
}
