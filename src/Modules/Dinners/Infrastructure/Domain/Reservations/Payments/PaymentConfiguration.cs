using Dinners.Domain.Reservations.ReservationsPayments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dinners.Infrastructure.Domain.Reservations.Payments;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<ReservationPayment>
{
    public void Configure(EntityTypeBuilder<ReservationPayment> builder)
    {
        builder.ToTable("ReservationPayments", "dinners");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                reservationPaymentId => reservationPaymentId.Value,
                value => ReservationPaymentId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("ReservationPaymentId");

        builder.Property(r => r.PayerId)
            .HasColumnName("PayerId");

        builder.OwnsOne(r => r.Price, x =>
        {
            x.Property(r => r.Amount).HasColumnName("MoneyPaid");
            x.Property(r => r.Currency).HasColumnName("MoneyCurrency");
        });

        builder.Property(r => r.PayedAt)
            .HasColumnName("PayedAt");
    }
}
