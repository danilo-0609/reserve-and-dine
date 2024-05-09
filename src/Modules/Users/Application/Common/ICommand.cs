using ErrorOr;
using MediatR;

namespace Users.Application.Common;

public interface ICommand<out TResponse> : IRequest<TResponse>
    where TResponse : IErrorOr
{
}
