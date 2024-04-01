using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dinners.Infrastructure.Domain.Restaurants.RestaurantsRatings;

internal sealed class RestaurantRatingsConfiguration : IEntityTypeConfiguration<RestaurantRatings>
{
    public void Configure(EntityTypeBuilder<RestaurantRatings> builder)
    {
        builder.ToTable("RestaurantRatings", "dinners");

        builder.HasKey(r => new { r.RestaurantId, r.RestaurantRatingId });

        builder.Property(r => r.RestaurantId)
            .HasConversion(
                restaurantId => restaurantId.Value,
                value => RestaurantId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("RestaurantId");

        builder.Property(r => r.RestaurantRatingId)
            .HasConversion(
                restaurantRatingId => restaurantRatingId.Value,
                value => RestaurantRatingId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("RatingId");
    }
}
