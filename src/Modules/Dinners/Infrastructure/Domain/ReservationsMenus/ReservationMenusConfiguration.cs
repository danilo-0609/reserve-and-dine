using Dinners.Domain.Menus;
using Dinners.Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dinners.Infrastructure.Domain.ReservationsMenus;

internal sealed class ReservationMenusConfiguration : IEntityTypeConfiguration<ReservationMenus>
{
    public void Configure(EntityTypeBuilder<ReservationMenus> builder)
    {
        builder.ToTable("ReservationMenus", "dinners");

        builder.HasKey(x => new { x.ReservationId, x.MenuId });

        builder.Property(p => p.MenuId)
            .HasConversion(
                menuId => menuId.Value,
                value => MenuId.Create(value))
            .HasColumnName("MenuId");

        builder.Property(p => p.ReservationId)
            .HasConversion(
                reservationId => reservationId.Value,
                value => ReservationId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("ReservationId");
    }
}
