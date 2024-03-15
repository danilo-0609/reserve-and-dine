using FluentValidation;

namespace Dinners.Application.Menus.MenuImages.Add;

internal sealed class AddMenuImageCommandValidator : AbstractValidator<AddMenuImageCommand>
{
    public AddMenuImageCommandValidator()
    {
        RuleFor(r => r.Id)
            .NotNull();

        RuleFor(r => r.FilePath)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.FormFile)
            .NotNull();
    }
}
