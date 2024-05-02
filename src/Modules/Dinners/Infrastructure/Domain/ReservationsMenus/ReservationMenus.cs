using Dinners.Domain.Menus;
using Dinners.Domain.Reservations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dinners.Infrastructure.Domain.ReservationsMenus;

public sealed class ReservationMenus 
{
    public ReservationId ReservationId { get; set; }

    public MenuId MenuId { get; set; }

    public ReservationMenus(ReservationId reservationId, MenuId menuId)
    {
        ReservationId = reservationId;
        MenuId = menuId;
    }  

    [ForeignKey("ReservationId")]
    public Reservation Reservation { get; set; }

    [ForeignKey("MenuId")]
    public Menu Menu { get; set; }
}
