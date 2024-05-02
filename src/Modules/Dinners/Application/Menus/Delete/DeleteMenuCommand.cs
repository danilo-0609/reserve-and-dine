using Dinners.Application.Common;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Menus.Delete;

public sealed record DeleteMenuCommand(Guid Id) : ICommand<ErrorOr<Unit>>;
