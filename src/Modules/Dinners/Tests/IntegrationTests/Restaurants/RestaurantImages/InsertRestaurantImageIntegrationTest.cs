using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Application.Restaurants.RestaurantImages.Insert;
using Dinners.Domain.Restaurants;
using Dinners.Tests.UnitTests.Restaurants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Dinners.Tests.IntegrationTests.Restaurants.RestaurantImages;

public sealed class InsertRestaurantImageIntegrationTest : BaseIntegrationTest
{
    private const string ImagePath = "../../../../../../test_image.png";

    public InsertRestaurantImageIntegrationTest(IntegrationTestWebAppFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async void InsertRestaurantImage_Should_ReturnAnError_WhenRestaurantDoesNotExist()
    {   
        var command = new InsertRestaurantImagesCommand(
            Id: Guid.NewGuid(),
            new FormFile(File.OpenRead(ImagePath), 0, File.OpenRead(ImagePath).Length, "test_image.png", "test_image.png"),
            ImagePath);

        var result = await Sender.Send(command);

        bool isErrorRestaurantDoesNotExist = result
            .FirstError
            .Code == "Restaurant.NotFound";

        Assert.True(isErrorRestaurantDoesNotExist);
    }

    [Fact]
    public async void InsertRestaurantImage_Should_AddImageOnBlobStorageAndRestaurantImageInDatabase_WhenSuccessful()
    {
        var restaurant = new RestaurantTests().CreateRestaurant(RestaurantId.CreateUnique());

        await DbContext.Restaurants.AddAsync(restaurant);
        await DbContext.SaveChangesAsync();

        var command = new InsertRestaurantImagesCommand(
            Id: restaurant.Id.Value,
            new FormFile(File.OpenRead(ImagePath), 0, File.OpenRead(ImagePath).Length, "test_image.png", "test_image.png"),
            ImagePath);

        await Sender.Send(command);
        
        var getRestaurant = await DbContext
            .Restaurants
            .Where(r => r.Id ==  restaurant.Id)
            .SingleAsync();

        var restaurantImage = getRestaurant.RestaurantImagesUrl.SingleOrDefault();

        BlobObject? blobUploaded = await RestaurantBlobService
            .GetBlobAsync(restaurantImage!.Value);

        bool isBlobUploadedOnBlobStorageAndRestaurantImageAddedInDatabase =
            getRestaurant.RestaurantImagesUrl.Any() &&
            blobUploaded is not null;

        await RestaurantBlobService.DeleteBlobAsync(ImagePath);

        Assert.True(isBlobUploadedOnBlobStorageAndRestaurantImageAddedInDatabase);
    }
}
