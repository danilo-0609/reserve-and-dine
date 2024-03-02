using FluentValidation;

namespace Dinners.Application.Menus.UpdateReview.Comments;

internal sealed class UpdateReviewCommentCommandValidator : AbstractValidator<UpdateReviewCommentCommand>
{
    public UpdateReviewCommentCommandValidator()
    {
        RuleFor(r => r.MenuReviewId)
            .NotNull().NotEmpty();

        RuleFor(r => r.Comment)
            .NotEmpty().NotNull();
    }
}
