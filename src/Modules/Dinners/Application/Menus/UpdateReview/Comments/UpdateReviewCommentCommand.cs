using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.UpdateReview.Comments;

public sealed record UpdateReviewCommentCommand(Guid MenuReviewId,
    string Comment) : ICommand<ErrorOr<Unit>>;
