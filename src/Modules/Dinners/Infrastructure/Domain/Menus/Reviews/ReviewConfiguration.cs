using Dinners.Domain.Menus.MenuReviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dinners.Infrastructure.Domain.Menus.Reviews;

internal sealed class ReviewConfiguration : IEntityTypeConfiguration<MenuReview>
{
    public void Configure(EntityTypeBuilder<MenuReview> builder)
    {
        builder.ToTable("Reviews", "dinners");

        builder.HasKey(x => x.Id);
    
        builder.Property(r => r.Id)
            .HasConversion(
                reviewId => reviewId.Value,
                value => MenuReviewId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("MenuReviewId");

        builder.Property(p => p.Rate)
            .HasColumnName("Rate")
            .HasColumnType("decimal")
            .HasPrecision(5, 2); ;

        builder.Property(p => p.ClientId)
            .HasColumnName("ClientId");

        builder.Property(p => p.Comment)
            .HasColumnName("Comment");

        builder.Property(p => p.ReviewedAt)
            .HasColumnName("ReviewedAt");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt");
    }
}
