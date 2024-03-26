using BuildingBlocks.Application;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace API.Configuration;

public sealed class ExecutionContextAccessor : IExecutionContextAccessor
{
    private readonly IHttpContextAccessor _contextAccessor;

    public ExecutionContextAccessor(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Guid UserId
    {
        get
        {
            if (_contextAccessor.HttpContext is not null &&
                _contextAccessor.HttpContext.User is not null &&
                _contextAccessor.HttpContext.User.Claims is not null)
            {
                string? subClaim = _contextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (subClaim is null)
                {
                    throw new ApplicationException("User context is not available");
                }

                Match match = Regex.Match(subClaim, @"\b([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\b");

                if (Guid.TryParse(match.Value, out var userId))
                {
                    return userId;
                }
            }

            throw new ApplicationException("User context is not available");
        }
    }
}
