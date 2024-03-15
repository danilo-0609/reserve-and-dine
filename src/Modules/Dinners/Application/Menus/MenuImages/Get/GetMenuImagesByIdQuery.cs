using BuildingBlocks.Application;
using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.MenuImages.Get;

internal sealed record GetMenuImagesByIdQuery(Guid MenuId) : IQuery<ErrorOr<List<BlobObject>>>;
