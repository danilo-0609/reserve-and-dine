using BuildingBlocks.Application;
using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.MenuImages.Get;

public sealed record GetMenuImageByIdQuery(Guid MenuId, Guid ImageId) : IQuery<ErrorOr<BlobObject>>;
