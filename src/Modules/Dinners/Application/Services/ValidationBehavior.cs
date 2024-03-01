using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Services;

internal sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators is null)
        {
            return await next();
        }

        foreach (IValidator<TRequest> validator in _validators)
        {
            ValidationResult? validationResult = await validator.ValidateAsync(request, cancellationToken);
        
            if (validationResult.IsValid)
            {
                return await next();
            }

            List<Error> errors = validationResult
                .Errors
                .ConvertAll(error => Error.Validation(error.ErrorCode, error.ErrorMessage));

            return (dynamic)errors;
        }

        return await next();
    }
}    