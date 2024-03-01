using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.Review;

public sealed record ReviewMenuCommand(
    Guid MenuId,
    decimal Rate,
    string Comment) : ICommand<ErrorOr<Guid>>;
