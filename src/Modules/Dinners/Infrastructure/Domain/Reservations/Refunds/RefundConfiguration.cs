using Dinners.Domain.Reservations.Refunds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dinners.Infrastructure.Domain.Reservations.Refunds;

internal sealed class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("Refunds", "dinners");
    
        builder.HasKey(r => r.Id);
    
        builder.Property(r => r.Id)
            .HasConversion(
                refundId => refundId.Value,
                value => RefundId.Create(value))
            .HasColumnName("RefundId");

        builder.Property(r => r.ClientId)
            .HasColumnName("ClientId");

        builder.OwnsOne(r => r.RefundedMoney, x =>
        {
            x.Property(r => r.Amount).HasColumnName("MoneyRefunded").HasColumnType("decimal").HasPrecision(10, 2); ;
            x.Property(r => r.Currency).HasColumnName("MoneyCurrency");
        });

        builder.Property(r => r.RefundedAt)
            .HasColumnName("RefundedAt");
    }
}
