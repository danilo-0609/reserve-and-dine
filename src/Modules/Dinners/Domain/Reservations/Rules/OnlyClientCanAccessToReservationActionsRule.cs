using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class OnlyClientCanAccessToReservationActionsRule : IBusinessRule
{
    private readonly Guid _userId;
    private readonly Guid _clientId;

    public OnlyClientCanAccessToReservationActionsRule(Guid clientId, Guid userId)
    {
        _clientId = clientId;
        _userId = userId;
    }

    public Error Error => ReservationErrorsCodes.UserIsNotAllowedToGetOrAccessContent;

    public bool IsBroken()
    {
        if (_clientId  == _userId)
        {
            return false;
        }        

        return true;
    }
}
