using Dinners.Application.Common;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Dinners.Application.Menus.MenuImages.Add;

public sealed record AddMenuImageCommand(Guid Id, IFormFile FormFile, string FilePath) : ICommand<ErrorOr<Unit>>;
