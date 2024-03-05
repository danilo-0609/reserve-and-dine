using ErrorOr;

namespace BuildingBlocks.Application;

public sealed class DomainEventHandlerException : Exception
{
    public Error Error { get; private set; }

    public DateTime OcurredOn { get; private set; }

    public DomainEventHandlerException(Error error,
        DateTime ocurredOn) : base(error.Description)
    {
        Error = error;
        OcurredOn = ocurredOn;
    }
}
