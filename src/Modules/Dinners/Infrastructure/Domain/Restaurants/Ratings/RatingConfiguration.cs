using Dinners.Domain.Restaurants.RestaurantRatings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dinners.Infrastructure.Domain.Restaurants.Ratings;

internal sealed class RatingConfiguration : IEntityTypeConfiguration<RestaurantRating>
{
    public void Configure(EntityTypeBuilder<RestaurantRating> builder)
    {
        builder.ToTable("Ratings", "dinners");

        builder.HasKey(x => x.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                ratingId => ratingId.Value,
                value => RestaurantRatingId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("RatingId");

        builder.Property(r => r.ClientId)
            .HasColumnName("ClientId");

        builder.Property(r => r.Stars)
            .HasColumnName("Stars");

        builder.Property(r => r.Comment).HasColumnName("Comment");

        builder.Property(r => r.RatedAt).HasColumnName("RatedAt");

        builder.Property(r => r.UpdatedAt).HasColumnName("UpdatedAt")
            .IsRequired(false);
    }
}
