using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Dinners.Infrastructure.Domain.ReservationsMenus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dinners.Infrastructure.Domain.Reservations;

internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations", "dinners");

        builder.HasKey(x => x.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                reservationId => reservationId.Value,
                value => ReservationId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("ReservationId");

        builder.ComplexProperty(r => r.ReservationInformation, x =>
        {
            x.Property(p => p.ReservedTable).ValueGeneratedNever().HasColumnName("ReservedTable");

            x.ComplexProperty(r => r.TimeOfReservation, g =>
            {
                g.Property(x => x.Start).HasColumnName("ReservationStartTime");
                g.Property(x => x.End).HasColumnName("ReservationEndTime");
            });

            x.Property(r => r.ReservationDateTime)
                .HasColumnName("ReservationDateTime");
        });

        builder.HasMany(r => r.MenuIds)
            .WithMany()
           .UsingEntity<ReservationMenus>();

        builder.Property(r => r.RestaurantId)
            .HasConversion(
                reservationId => reservationId.Value,
                value => RestaurantId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("RestaurantId");

        builder.ComplexProperty(r => r.ReservationAttendees, x =>
        {
            x.Property(g => g.ClientId).HasColumnName("ClientId");
            x.Property(g => g.Name).HasColumnName("Name");
            x.Property(g => g.NumberOfAttendees).HasColumnName("NumberOfAttendees");
        });

        builder.ComplexProperty(r => r.ReservationStatus, x =>
        {
            x.Property(r => r.Value).HasColumnName("ReservationStatus");
        });

        builder.Property(r => r.CancelledAt)
            .IsRequired(false)
            .HasColumnName("CancelledAt");
    }
}
