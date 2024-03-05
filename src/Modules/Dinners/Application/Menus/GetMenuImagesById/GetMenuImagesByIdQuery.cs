using BuildingBlocks.Application;
using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.GetMenuImagesById;

internal sealed record GetMenuImagesByIdQuery(Guid MenuId) : IQuery<ErrorOr<List<BlobObject>>>;
