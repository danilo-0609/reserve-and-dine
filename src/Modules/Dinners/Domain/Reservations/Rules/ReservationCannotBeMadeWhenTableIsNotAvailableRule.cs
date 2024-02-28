using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Reservations.Errors;
using ErrorOr;

namespace Dinners.Domain.Reservations.Rules;

internal sealed class ReservationCannotBeMadeWhenTableIsNotAvailableRule : IBusinessRule
{
    private readonly List<int> _availableTables;
    private readonly int _requestedTable;

    public ReservationCannotBeMadeWhenTableIsNotAvailableRule(List<int> availableTables, int requestedTable)
    {
        _availableTables = availableTables;
        _requestedTable = requestedTable;
    }

    public Error Error => ReservationErrorsCodes.CannotBeMadeWhenTableIsNotAvailable;

    public bool IsBroken() => !_availableTables.Any(r => r == _requestedTable);

    public static string Message => "Reservation cannot be made when table is not available";
}
