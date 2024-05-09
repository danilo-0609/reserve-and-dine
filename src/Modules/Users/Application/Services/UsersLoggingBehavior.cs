using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Users.Application.Services;

internal sealed class UsersLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly ILogger<UsersLoggingBehavior<TRequest, TResponse>> _logger;

    public UsersLoggingBehavior(ILogger<UsersLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting request: {@RequestName}, at {@DateTime}",
            typeof(TRequest).Name,
            DateTime.UtcNow);

        var result = await next();

        if (result.IsError)
        {
            _logger.LogError("Request {@RequestName} failed. At {@DateTime}. Error: {@Error}",
                typeof(TRequest).Name,
                DateTime.UtcNow,
                result.Errors);
        }

        _logger.LogInformation("Completed request: {@RequestName}. At {@DateTime}",
            typeof(TRequest).Name,
            DateTime.UtcNow);

        return result;
    }
}