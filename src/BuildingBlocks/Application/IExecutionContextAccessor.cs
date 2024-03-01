namespace BuildingBlocks.Application;

public interface IExecutionContextAccessor
{
    Guid UserId { get; }
}
