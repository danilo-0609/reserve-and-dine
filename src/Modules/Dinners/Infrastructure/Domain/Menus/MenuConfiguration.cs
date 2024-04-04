using Dinners.Domain.Menus;
using Dinners.Domain.Restaurants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dinners.Infrastructure.Domain.Menus.MenuReviews;
using Dinners.Domain.Menus.Dishes;
using Dinners.Domain.Menus.Details;
using Dinners.Domain.Menus.Schedules;

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

        builder.ComplexProperty<MenuDetails>("MenuDetails", x =>
        {
            x.Property(p => p.Title).HasColumnName("Title");
            x.Property(p => p.Description).HasColumnName("Description");
            x.ComplexProperty(p => p.MenuType).Property(p => p.Value).HasColumnName("MenuType");
            x.ComplexProperty(p => p.Price, t =>
            {
                t.Property(f => f.Amount).HasColumnName("PriceAmount").HasColumnType("decimal").HasPrecision(10, 2); ;
                t.Property(f => f.Currency).HasColumnName("PriceCurrency");
            });
            x.Property(p => p.Discount).HasColumnName("Discount").HasColumnType("decimal").HasPrecision(10, 2);
            x.Property(p => p.DiscountTerms).HasColumnName("DiscountTerms");

            x.Property(p => p.IsVegetarian).HasColumnName("IsVegetarian");
            x.Property(p => p.PrimaryChefName).HasColumnName("PrimaryChefName");
            x.Property(p => p.HasAlcohol).HasColumnName("HasAlcohol");
        });

        builder.ComplexProperty<DishSpecification>("DishSpecification", x =>
        {
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
            .WithMany()
            .UsingEntity<MenusReviews>();

        builder.OwnsMany(r => r.MenuImagesUrl, x =>
        {
            x.WithOwner().HasForeignKey("MenuId");
            x.HasKey(x => x.Id);
            x.ToTable("MenuImagesUrl", "dinners");

            x.Property(r => r.Id)
                .HasConversion(
                    menuImageUrlId => menuImageUrlId.Value,
                    value => MenuImageUrlId.Create(value))
                .HasColumnName("MenuImageUrlId");


            x.Property(r => r.Value).HasColumnName("MenuImageUrl");
            x.Property(r => r.MenuId).HasColumnName("MenuId")
                .HasConversion(
                    menuId => menuId.Value,
                    value => MenuId.Create(value))
                .ValueGeneratedNever();
        });

        builder.OwnsMany(r => r.Tags, x =>
        {
            x.WithOwner().HasForeignKey("MenuId");
            x.HasKey(x => x.Id);
            x.ToTable("Tags", "dinners");

            x.Property(r => r.Id)
                .HasConversion(
                    tagId => tagId.Value,
                    value => TagId.Create(value))
                .HasColumnName("TagId");

            x.Property(r => r.Value).HasColumnName("Tag");
            x.Property(r => r.MenuId).HasColumnName("MenuId")
                .HasConversion(
                    menuId => menuId.Value,
                    value => MenuId.Create(value))
                .ValueGeneratedNever(); ;
        });

        builder.OwnsMany(r => r.Ingredients, x =>
        {
            x.WithOwner().HasForeignKey("MenuId");
            x.HasKey(x => x.Id);
            x.ToTable("Ingredients", "dinners");

            x.Property(r => r.Id)
                .HasConversion(
                    ingredientId => ingredientId.Value,
                    value => IngredientId.Create(value))
                .HasColumnName("IngredientId");

            x.Property(r => r.Value).HasColumnName("Ingredient"); 
            x.Property(r => r.MenuId).HasColumnName("MenuId")
                .HasConversion(
                    menuId => menuId.Value,
                    value => MenuId.Create(value))
                .ValueGeneratedNever(); ;
        });

        builder.OwnsMany(r => r.MenuSchedules, x =>
        {
            x.WithOwner().HasForeignKey("MenuId");
            x.HasKey(x => x.Id);

            x.Property(r => r.Id)
                .HasConversion(
                    menuScheduleId => menuScheduleId.Value,
                    value => MenuScheduleId.Create(value))
                .HasColumnName("MenuScheduleId");

            x.Property(x => x.MenuId).HasColumnName("MenuId")
                .HasConversion(
                    menuId => menuId.Value,
                    value => MenuId.Create(value))
                .ValueGeneratedNever();

            x.Property(r => r.Day).HasColumnName("DayOfWeek");
            x.Property(r => r.StartTimeSpan).HasColumnName("StartTimeSpan");
            x.Property(r => r.EndTimeSpan).HasColumnName("EndTimeSpan");
        });

        builder.OwnsMany(r => r.MenuConsumers, x =>
        {
            x.WithOwner().HasForeignKey("MenuId");
            x.HasKey(x => x.MenuId);
            x.ToTable("MenuConsumers", "dinners");

            x.Property(r => r.MenuId).HasColumnName("MenuId")
                .HasConversion(
                    menuId => menuId.Value,
                    value => MenuId.Create(value))
                .ValueGeneratedNever();

            x.Property(r => r.ClientId).HasColumnName("ClientId");
        });

        builder.Property(r => r.CreatedOn).HasColumnName("CreatedOn");
        builder.Property(r => r.UpdatedOn).HasColumnName("UpdatedOn");
    }
}
