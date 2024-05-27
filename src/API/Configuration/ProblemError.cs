using API.Common.Http;
using ErrorOr;

namespace API.Configuration;

public sealed class ProblemError 
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProblemError(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IResult Errors(List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Results.Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        _httpContextAccessor.HttpContext!.Items[HttpContextItemKeys.Errors] = errors;

        return Problem(errors[0]);
    }

    private IResult Problem(Error error)
    {
        int statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(statusCode: statusCode, title: error.Description);
    }

    private IResult ValidationProblem(List<Error> errors)
    {
        Dictionary<string, string[]> modelStateDictionary = new();

        foreach (var error in errors)
        {
            modelStateDictionary.Add(error.Code, new string[] { error.Description });
        }

        return Results.ValidationProblem(modelStateDictionary);
    }
}
