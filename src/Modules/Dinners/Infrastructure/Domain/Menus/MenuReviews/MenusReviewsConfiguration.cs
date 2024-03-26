using Dinners.Domain.Menus;
using Dinners.Domain.Menus.MenuReviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dinners.Infrastructure.Domain.Menus.MenuReviews;

internal sealed class MenusReviewsConfiguration : IEntityTypeConfiguration<MenusReviews>
{
    public void Configure(EntityTypeBuilder<MenusReviews> builder)
    {
        builder.ToTable("MenusReviews", "dinners");

        builder.HasKey(r => new { r.MenuId, r.MenuReviewId });
    
        builder.Property(r => r.MenuId)
            .HasConversion(
                menuId => menuId.Value,
                value => MenuId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("MenuId");

        builder.Property(r => r.MenuReviewId)
            .HasConversion(
                reviewId => reviewId.Value,
                value => MenuReviewId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("MenuReviewId");
    }
}
