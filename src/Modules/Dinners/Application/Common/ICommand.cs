using ErrorOr;
using MediatR;

namespace Dinners.Application.Common;

public interface ICommand<out TResponse> : IRequest<TResponse> 
    where TResponse : IErrorOr
{
}
