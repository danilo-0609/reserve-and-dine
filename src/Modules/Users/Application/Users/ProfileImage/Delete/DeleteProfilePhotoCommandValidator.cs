using FluentValidation;

namespace Users.Application.Users.ProfileImage.Delete;

internal sealed class DeleteProfilePhotoCommandValidator : AbstractValidator<DeleteProfilePhotoCommand>
{
    public DeleteProfilePhotoCommandValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .NotNull();
    }
}
