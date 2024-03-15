using Dinners.Domain.Restaurants;
using Dinners.Infrastructure.Domain.Restaurants.RestaurantsRatings;
using Domain.Restaurants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dinners.Infrastructure.Domain.Restaurants;

internal sealed class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("Restaurants", "dinners");
    
        builder.HasKey(x => x.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                restaurantId => restaurantId.Value,
                value => RestaurantId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("RestaurantId");

        builder.Property(r => r.NumberOfTables)
            .HasColumnName("NumberOfTables");

        builder.OwnsOne(r => r.AvailableTablesStatus, x =>
        {
            x.Property(r => r.Value).HasColumnName("AvailableTableStatus");
        });

        builder.OwnsOne(r => r.RestaurantInformation, x =>
        {
            x.Property(t => t.Title).HasColumnName("Title");
            x.Property(t => t.Description).HasColumnName("Description");
            x.Property(t => t.Type).HasColumnName("Type");

            x.OwnsMany(p => p.Chefs, x =>
            {
                x.WithOwner().HasForeignKey("RestaurantId");
                x.ToTable("Chefs", "dinners");
            });

            x.OwnsMany(p => p.Specialties, x =>
            {
                x.WithOwner().HasForeignKey("RestaurantId");
                x.ToTable("Specialties", "dinners");
            });

            x.OwnsMany(p => p.RestaurantImagesUrl.Select(r => r.AbsoluteUri), x =>
            {
                x.WithOwner().HasForeignKey("RestaurantId");
                x.ToTable("RestaurantImagesUrl", "dinners");
            });
        });

        builder.OwnsOne(r => r.RestaurantLocalization, x =>
        {
            x.Property(r => r.Country).HasColumnName("Country");
            x.Property(r => r.City).HasColumnName("City");
            x.Property(r => r.Region).HasColumnName("Region");
            x.Property(r => r.Neighborhood).HasColumnName("Neighborhood");
            x.Property(r => r.Addresss).HasColumnName("Address");
            x.Property(r => r.LocalizationDetails).HasColumnName("LocalizationDetails");
        });

        builder.OwnsOne(r => r.RestaurantScheduleStatus, x =>
        {
            x.Property(r => r.Value).HasColumnName("RestaurantScheduleStatus");
        });

        builder.OwnsOne(r => r.RestaurantSchedule, x =>
        {
            x.OwnsMany(r => r.Days.Select(r => r.ToString()).ToList(), t =>
            {
                t.WithOwner().HasForeignKey("RestaurantId");
                t.ToTable("DaysOfService", "dinners");
            });

            x.OwnsOne(p => p.HoursOfOperation, x =>
            {
                x.Property(r => r.Start).HasColumnName("OpeningTime");
                x.Property(r => r.End).HasColumnName("ClosingTime");
            });
        });

        builder.OwnsOne(r => r.RestaurantContact, x =>
        {
            x.Property(r => r.Email).HasColumnName("Email");
            x.Property(r => r.Whatsapp).HasColumnName("Whatsapp");
            x.Property(r => r.Facebook).HasColumnName("Facebook");
            x.Property(r => r.PhoneNumber).HasColumnName("PhoneNumber");
            x.Property(r => r.Instagram).HasColumnName("Instagram");
            x.Property(r => r.Twitter).HasColumnName("Twitter");
            x.Property(r => r.TikTok).HasColumnName("TikTok");
            x.Property(r => r.Website).HasColumnName("Website");
        });


        builder.HasMany(r => r.RestaurantRatingIds)
            .WithMany().UsingEntity<RestaurantRatings>();

        builder.OwnsMany(r => r.RestaurantClients, x =>
        {
            x.WithOwner().HasForeignKey("RestaurantId");
            x.ToTable("RestaurantClients", "dinners");

            x.Property(r => r.ClientId).HasColumnName("ClientId");
            x.Property(r => r.NumberOfVisits).HasColumnName("NumberOfVisits");
        });

        builder.OwnsMany(r => r.RestaurantTables, x =>
        {
            x.WithOwner().HasForeignKey("RestaurantId");
            x.ToTable("RestaurantTables", "dinners");

            x.Property(r => r.Number).HasColumnName("Number").ValueGeneratedNever();
            x.Property(r => r.Seats).HasColumnName("Seats");
            x.Property(r => r.IsPremium).HasColumnName("IsPremium");
            x.Property(r => r.IsOccuppied).HasColumnName("IsOccuppied");


            x.OwnsMany(r => r.ReservedHours.ToDictionary().Select(r => new { r.Value, r.Key }), x =>
            {
                x.WithOwner().HasForeignKey("RestaurantId");
                x.ToTable("ReservedHours", "dinners");

                x.Property(r => r.Key).HasColumnName("ReservedHourDateTime");

                x.OwnsOne(p => p.Value, x =>
                {
                    x.Property(r => r.Start).HasColumnName("ReservationStartTime");
                    x.Property(r => r.End).HasColumnName("ReservationEndTime");
                });
            });
        });

        builder.OwnsMany(r => r.RestaurantAdministrations, x =>
        {
            x.WithOwner().HasForeignKey("RestaurantId");
            x.ToTable("RestaurantAdministrations", "dinners");

            x.Property(r => r.Name).HasColumnName("Name");
            x.Property(r => r.AdministratorId).HasColumnName("AdministratorId");
            x.Property(r => r.AdministratorTitle).HasColumnName("AdministratorTitle");
        });

        builder.Property(r => r.PostedAt)
            .HasColumnName("PostedAt");
    }
}
