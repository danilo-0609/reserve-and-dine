using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantInformations;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using Dinners.Domain.Restaurants.RestaurantTables;
using Dinners.Domain.Restaurants.RestaurantUsers;
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

        builder.OwnsMany(r => r.RestaurantSchedules, x =>
        {
            x.WithOwner().HasForeignKey("RestaurantId");
            x.HasKey(k => k.Id);
            x.ToTable("RestaurantSchedules", "dinners");

            x.Property(p => p.Id)
                .HasConversion(
                    restaurantScheduleId => restaurantScheduleId.Value,
                    value => RestaurantScheduleId.Create(value))
                .HasColumnName("RestaurantScheduleId");

            x.Property(r => r.RestaurantId)
            .HasConversion(
                restaurantId => restaurantId.Value,
                value => RestaurantId.Create(value))
            .HasColumnName("RestaurantId");

            x.OwnsOne(r => r.Day, g =>
            {
                g.Property(r => r.DayOfWeek).HasColumnName("DayOfWeek");
            });

            x.OwnsOne(r => r.HoursOfOperation, s =>
            {
                s.Property(r => r.Start).HasColumnName("OpenTime");
                s.Property(r => r.End).HasColumnName("CloseTime");
            });

            x.Property(t => t.ReopeningTime)
                .HasColumnName("ReopeningTime")
                .IsRequired(false);
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
            x.HasKey(k => k.Id);
            x.ToTable("RestaurantClients", "dinners");

            x.Property(p => p.Id)
                .HasConversion(
                    restaurantClientId => restaurantClientId.Value,
                    value => RestaurantClientId.Create(value))
                .HasColumnName("RestaurantClientId");

            x.Property(r => r.RestaurantId)
            .HasConversion(
                restaurantId => restaurantId.Value,
                value => RestaurantId.Create(value))
            .HasColumnName("RestaurantId");
            
            x.Property(r => r.ClientId).HasColumnName("ClientId");
            x.Property(r => r.NumberOfVisits).HasColumnName("NumberOfVisits");
        });

        builder.OwnsMany(r => r.RestaurantTables, x =>
        {
            x.WithOwner().HasForeignKey("RestaurantId");
            x.HasKey(x => x.Id);
            x.ToTable("RestaurantTables", "dinners");

            x.Property<RestaurantTableId>(p => p.Id)
                .HasConversion(
                       restaurantTableId => restaurantTableId.Value,
                       value => RestaurantTableId.Create(value))
                .HasColumnName("RestaurantTableId");

            x.Property(p => p.RestaurantId)
                .HasConversion(restaurantId => restaurantId.Value,
                               value => RestaurantId.Create(value))
                .HasColumnName("RestaurantId");

            x.Property(r => r.Number).HasColumnName("NumberOfTable").ValueGeneratedNever();
            x.Property(r => r.Seats).HasColumnName("Seats");
            x.Property(r => r.IsPremium).HasColumnName("IsPremium");
            x.Property(r => r.IsOccupied).HasColumnName("IsOccupied");

            x.OwnsMany(r => r.ReservedHours, t =>
            {
                t.WithOwner().HasForeignKey("RestaurantTableId");
                t.HasKey(k => k.Id);
                t.ToTable("ReservedHours", "dinners");

                t.Property(r => r.Id)
                    .HasConversion(
                        reservedHourId => reservedHourId.Value,
                        value => ReservedHourId.Create(value))
                    .HasColumnName("ReservedHourId");

                t.Property<RestaurantTableId>(p => p.RestaurantTableId)
                .HasConversion(
                        restaurantTableId => restaurantTableId.Value,
                        value => RestaurantTableId.Create(value))
                .HasColumnName("RestaurantTableId");

                t.Property(p => p.RestaurantId)
                    .HasConversion(restaurantId => restaurantId.Value,
                                   value => RestaurantId.Create(value))
                    .HasColumnName("RestaurantId");

                t.Property(p => p.ReservationDateTime).HasColumnName("ReservationDateTime");
                t.OwnsOne(o => o.ReservationTimeRange, k =>
                {
                    k.Property(r => r.Start).HasColumnName("StartReservationTimeRange");
                    k.Property(r => r.End).HasColumnName("EndReservationTimeRange");
                });
            });
        });

        builder.OwnsMany(r => r.RestaurantAdministrations, x =>
        {
            x.WithOwner().HasForeignKey("RestaurantId");
            x.HasKey(k => k.Id);
            x.ToTable("RestaurantAdministrations", "dinners");

            x.Property(r => r.Id)
                .HasConversion(
                    restaurantAdministrationId => restaurantAdministrationId.Value,
                    value => RestaurantAdministrationId.Create(value))
                .HasColumnName("RestaurantAdministrationId");

            x.Property(p => p.RestaurantId)
                .HasConversion(
                    restaurantId => restaurantId.Value,
                    value => RestaurantId.Create(value))
                .HasColumnName("RestaurantId");

            x.Property(r => r.Name).HasColumnName("Name");
            x.Property(r => r.AdministratorId).HasColumnName("AdministratorId");
            x.Property(r => r.AdministratorTitle).HasColumnName("AdministratorTitle");
        });

        builder.OwnsMany(p => p.Chefs, x =>
        {
            x.WithOwner().HasForeignKey("RestaurantId");
            x.HasKey(x => new { x.Id, x.RestaurantId });
            x.ToTable("Chefs", "dinners");

            x.Property(r => r.Id)
                .HasConversion(
                    chefId => chefId.Value,
                    value => ChefId.Create(value))
                .HasColumnName("ChefId");

            x.Property(r => r.RestaurantId)
            .HasConversion(
                restaurantId => restaurantId.Value,
                value => RestaurantId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("RestaurantId");
            x.Property(r => r.Value).HasColumnName("Chef");
        });

        builder.OwnsMany(p => p.Specialities, x =>
        {
            x.WithOwner().HasForeignKey("RestaurantId");
            x.HasKey(x => x.Id);
            x.ToTable("Specialties", "dinners");

            x.Property(r => r.Id)
                .HasConversion(
                    specialityId => specialityId.Value,
                    value => SpecialityId.Create(value))
                .HasColumnName("SpecialityId");

            x.Property(r => r.RestaurantId)
            .HasConversion(
                restaurantId => restaurantId.Value,
                value => RestaurantId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("RestaurantId");

            x.Property(r => r.Value).HasColumnName("Speciality");
        });

        builder.OwnsMany(p => p.RestaurantImagesUrl, c =>
        {
            c.WithOwner().HasForeignKey("RestaurantId");
            c.HasKey(x => x.Id);
            c.ToTable("RestaurantImagesUrl", "dinners");

            c.Property(r => r.Id)
                .HasConversion(
                    restaurantImageUrlId => restaurantImageUrlId.Value,
                    value => RestaurantImageUrlId.Create(value))
                .HasColumnName("RestaurantImageUrlId");

            c.Property(r => r.RestaurantId)
            .HasConversion(
                restaurantId => restaurantId.Value,
                value => RestaurantId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("RestaurantId");

            c.Property(f => f.Value).HasColumnName("RestaurantImageUrl");
        });

        builder.Property(r => r.PostedAt)
            .HasColumnName("PostedAt");
    }
}
