using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Users;

namespace Users.Infrastructure.Domain.Users.UserRoles;

internal sealed class UserRolesConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles", "users");
        builder.HasKey(r => new { r.UserId, r.RoleId });

        builder.Property(r => r.UserId)
            .HasConversion(
                userId => userId.Value,
                value => UserId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("UserId");

        builder.Property(r => r.RoleId)
            .HasColumnName("RoleId");
    }
}
