using ErrorOr;
using MediatR;

namespace BuildingBlocks.Domain.Rules;

public interface ICheckRule
{
    ErrorOr<Unit> CheckRule(IBusinessRule rule);
}
