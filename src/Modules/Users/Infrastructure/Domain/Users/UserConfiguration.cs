using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Common;
using Users.Domain.Users;

namespace Users.Infrastructure.Domain.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "users");

        builder.HasKey(x => x.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                userId => userId.Value,
                value => UserId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("UserId");

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

        builder.Property(r => r.ProfileImageUrl)
            .HasColumnName("ProfileImageUrl");

        builder.Property(r => r.CreatedDateTime)
            .HasColumnName("CreatedDateTime");

        builder.Property(r => r.UpdatedDateTime)
            .HasColumnName("UpdatedDateTime");
    }
}