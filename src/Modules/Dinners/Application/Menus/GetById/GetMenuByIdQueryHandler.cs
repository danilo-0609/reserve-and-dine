using Dinners.Application.Common;
using ErrorOr;

namespace Dinners.Application.Menus.GetById;

internal sealed class GetMenuByIdQueryHandler : IQueryHandler<GetMenuByIdQuery, ErrorOr<MenuResponse>>
{
    public Task<ErrorOr<MenuResponse>> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
