using ErrorOr;
using MediatR;

namespace Dinners.Application.Common;

internal interface ICommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    where TResponse : IErrorOr
{
}
