using Dinners.Domain.Menus;
using Dinners.Domain.Restaurants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dinners.Infrastructure.Domain.Menus.MenuReviews;

namespace Dinners.Infrastructure.Domain.Menus;

internal sealed class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menus", "dinners");
        
        builder.HasKey(k => k.Id);
    
        builder.Property(p => p.Id)
            .HasConversion(
                menuId => menuId.Value,
                value => MenuId.Create(value))
            .ValueGeneratedNever()
            .HasColumnName("MenuId");

        builder.Property(p => p.RestaurantId)
            .HasConversion(
                menuId => menuId.Value,
                value => RestaurantId.Create(value))
            .HasColumnName("RestaurantId");

        builder.OwnsOne<MenuDetails>("MenuDetails", x =>
        {
            x.Property(p => p.Title).HasColumnName("Title");
            x.Property(p => p.Description).HasColumnName("Description");
            x.OwnsOne(p => p.MenuType).Property(p => p.Value).HasColumnName("MenuType");
            x.OwnsOne(p => p.Price, t =>
            {
                t.Property(f => f.Amount).HasColumnName("PriceAmount");
                t.Property(f => f.Currency).HasColumnName("PriceCurrency");
            });
            x.Property(p => p.Discount).HasColumnName("Discount");
            x.Property(p => p.DiscountTerms).HasColumnName("DiscountTerms");

            x.OwnsMany(p => p.MenuImagesUrl.Select(r => r.AbsoluteUri), x =>
            {
                x.WithOwner().HasForeignKey("MenuId");
                x.ToTable("MenuImagesUrl", "dinners");
            });

            x.OwnsMany(p => p.Tags, x =>
            {
                x.WithOwner().HasForeignKey("MenuId");
                x.ToTable("Tags", "dinners");
            });

            x.Property(p => p.IsVegetarian).HasColumnName("IsVegetarian");
            x.Property(p => p.PrimaryChefName).HasColumnName("PrimaryChefName");
            x.Property(p => p.HasAlcohol).HasColumnName("HasAlcohol");
        });

        builder.OwnsOne<DishSpecification>("DishSpecification", x =>
        {
            x.OwnsMany(p => p.Ingredients, x =>
            {
                x.WithOwner().HasForeignKey("MenuId");
                x.ToTable("Ingredients", "dinners");
            });

            x.Property(p => p.MainCourse).HasColumnName("MainCourse");
            x.Property(p => p.SideDishes).HasColumnName("SideDishes");
            x.Property(p => p.Appetizers).HasColumnName("Appetizers");
            x.Property(p => p.Beverages).HasColumnName("Beverages");
            x.Property(p => p.Desserts).HasColumnName("Desserts");
            x.Property(p => p.Sauces).HasColumnName("Sauces");
            x.Property(p => p.Condiments).HasColumnName("Condiments");
            x.Property(p => p.Coffee).HasColumnName("Coffee");
        });

        builder.HasMany(r => r.MenuReviewIds)
            .WithMany().UsingEntity<MenusReviews>();

        builder.OwnsOne<MenuSchedule>("MenuSchedule", x =>
        {
            x.OwnsMany(r => r.Days.Select(r => r.ToString()).ToList(), t =>
            {
                t.WithOwner().HasForeignKey("MenuId");
                t.ToTable("DaysAvailable", "dinners");
            });

            x.OwnsOne(p => p.AvailableMenuHours, x =>
            {
                x.Property(r => r.Start).HasColumnName("Open");
                x.Property(r => r.End).HasColumnName("Close");
            });
        });

        builder.OwnsMany(r => r.MenuConsumers, x =>
        {
            x.WithOwner().HasForeignKey("MenuId");
            x.ToTable("MenuConsumers", "dinners");
        });

        builder.Property(r => r.CreatedOn).HasColumnName("CreatedOn");
        builder.Property(r => r.UpdatedOn).HasColumnName("UpdatedOn");
    }
}
