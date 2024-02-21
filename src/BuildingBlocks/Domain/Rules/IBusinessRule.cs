using ErrorOr;

namespace BuildingBlocks.Domain.Rules;

public interface IBusinessRule
{
    bool IsBroken();

    static string Message { get; } = string.Empty;

    Error Error { get; }
}
