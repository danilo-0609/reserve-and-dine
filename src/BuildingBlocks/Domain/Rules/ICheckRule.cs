using ErrorOr;
using MediatR;

namespace BuildingBlocks.Domain.Rules;

public interface ICheckRule
{
    ErrorOr<Success> CheckRule(IBusinessRule rule);
}
