using ErrorOr;
using MediatR;

namespace Users.Application.Common;


internal interface IQueryHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
    where TResponse : IErrorOr
{
}
