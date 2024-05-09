using ErrorOr;
using MediatR;

namespace Users.Application.Common;

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : IErrorOr
{
}
