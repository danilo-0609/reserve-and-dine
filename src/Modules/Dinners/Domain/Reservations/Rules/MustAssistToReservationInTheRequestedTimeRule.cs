using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class MustAssistToReservationInTheRequestedTimeRule : IBusinessRule
{
    private readonly DateTime _requestedTime;
    private readonly DateTime _currentTime;

    public MustAssistToReservationInTheRequestedTimeRule(DateTime currentTime, DateTime requestedTime)
    {
        _currentTime = currentTime;
        _requestedTime = requestedTime;
    }

    public Error Error => ReservationErrorsCodes.CannotAssistToReservationOutOfTheRequestedTime;

    public bool IsBroken() => _currentTime < _requestedTime;

    public static string Message => "Must assist to reservation in the requested time";    
}
