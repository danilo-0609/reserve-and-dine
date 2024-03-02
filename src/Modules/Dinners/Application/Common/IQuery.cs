using ErrorOr;
using MediatR;

namespace Dinners.Application.Common;

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : IErrorOr
{
}
