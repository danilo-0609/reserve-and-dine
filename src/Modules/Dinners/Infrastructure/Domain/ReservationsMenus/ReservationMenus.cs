using Dinners.Domain.Menus;
using Dinners.Domain.Reservations;

namespace Dinners.Infrastructure.Domain.ReservationsMenus;

public sealed class ReservationMenus 
{
    public ReservationId ReservationId { get; set; }

    public MenuId MenuId { get; set; }
}
