using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dinners.Application.Services;

internal sealed class DinnersLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly ILogger<DinnersLoggingBehavior<TRequest, TResponse>> _logger;

    public DinnersLoggingBehavior(ILogger<DinnersLoggingBehavior<TRequest, TResponse>> logger)
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

        _logger.LogInformation("Completed request: {@RequestnName}. At {@DateTime}",
            typeof(TRequest).Name,
            DateTime.UtcNow);

        return result;
    }
}
