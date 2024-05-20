using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Common;
using Users.Domain.UserRegistrations;

namespace Users.Infrastructure.Domain.UserRegistrations;

internal sealed class UserRegistrationConfiguration : IEntityTypeConfiguration<UserRegistration>
{
    public void Configure(EntityTypeBuilder<UserRegistration> builder)
    {
        builder.ToTable("UserRegistrations", "users");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                userRegistrationId => userRegistrationId.Value,
                value => UserRegistrationId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("UserRegistrationId");

        builder.Property(r => r.Login)
            .HasColumnName("Login");

        builder.OwnsOne<Password>("Password", p =>
        {
            p.Property(p => p.Value)
             .HasColumnName("Password")
             .HasMaxLength(150);
        });

        builder.Property(r => r.Email)
            .HasColumnName("Email");

        builder.ComplexProperty(r => r.Status, x =>
        {
            x.Property(r => r.Value)
                .HasColumnName("UserRegistrationStatus");
        });

        builder.Property(r => r.RegisteredDate)
            .HasColumnName("RegisteredDate");

        builder.Property(r => r.ConfirmedDate)
            .HasColumnName("ConfirmedDate")
            .IsRequired(false);
    }
}
