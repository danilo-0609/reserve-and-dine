using FluentValidation;

namespace Users.Application.Users.ProfileImage.Set;

internal sealed class SetProfilePhotoCommandValidator : AbstractValidator<SetProfilePhotoCommand>
{
    public SetProfilePhotoCommandValidator()
    {
        RuleFor(r => r.Id)
            .NotNull()
            .NotEmpty();
    
        RuleFor(r => r.FilePath)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.FormFile)
            .NotEmpty()
            .NotNull();

    }
}
