using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Refunds;
using Dinners.Domain.Reservations.ReservationsPayments;
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

        builder.OwnsOne(r => r.ReservationInformation, x =>
        {
            x.Property(p => p.ReservedTable).ValueGeneratedNever().HasColumnName("ReservedTable");
            x.OwnsOne(p => p.ReservationPrice, r =>
            {
                r.Property(t => t.Amount).HasColumnName("MoneyAmount").HasColumnType("decimal").HasPrecision(10, 2); ;
                r.Property(t => t.Currency).HasColumnName("MoneyCurrency");
            });

            x.OwnsOne(r => r.TimeOfReservation, g =>
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

        builder.OwnsOne(r => r.ReservationAttendees, x =>
        {
            x.Property(g => g.ClientId).HasColumnName("ClientId");
            x.Property(g => g.Name).HasColumnName("Name");
            x.Property(g => g.NumberOfAttendees).HasColumnName("NumberOfAttendees");
        });

        builder.OwnsOne(r => r.ReservationStatus, x =>
        {
            x.Property(r => r.Value).HasColumnName("ReservationStatus");
        });

        builder.Property(r => r.ReservationPaymentId)
            .IsRequired(false)
            .HasDefaultValue(null)
            .HasConversion(
                paymentId => paymentId!.Value,
                value => ReservationPaymentId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("ReservationPaymentId");

        builder.Property(r => r.RefundId)
            .IsRequired(false)
            .HasDefaultValue(null)
            .HasConversion(
                refundId => refundId!.Value,
                value => RefundId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("RefundId");

        builder.Property(r => r.RequestedAt)
            .HasColumnName("RequestedAt");

        builder.Property(r => r.PaidAt)
            .IsRequired(false)
            .HasColumnName("PaidAt");

        builder.Property(r => r.CancelledAt)
            .IsRequired(false)
            .HasColumnName("CancelledAt");
    }
}
