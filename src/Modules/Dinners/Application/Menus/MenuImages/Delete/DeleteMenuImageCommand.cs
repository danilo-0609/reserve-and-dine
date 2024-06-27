using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.MenuImages.Delete;

public sealed record DeleteMenuImageCommand(Guid Id, Guid ImageId) : ICommand<ErrorOr<Success>>;
