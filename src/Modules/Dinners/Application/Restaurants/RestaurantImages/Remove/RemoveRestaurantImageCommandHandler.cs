﻿using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using ErrorOr;
using MediatR;
using Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;

namespace Dinners.Application.Restaurants.RestaurantImages.Remove;

internal sealed class RemoveRestaurantImageCommandHandler : ICommandHandler<RemoveRestaurantImageCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IRestaurantBlobService _blobService;

    public RemoveRestaurantImageCommandHandler(IRestaurantRepository restaurantRepository, IRestaurantBlobService blobService)
    {
        _restaurantRepository = restaurantRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<Unit>> Handle(RemoveRestaurantImageCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.Id));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        if (!restaurant.RestaurantImagesUrl.Any(r => r.Value == request.RestaurantImageUrl))
        {
            return Error.NotFound("Restaurant.ImageNotFound", "Restaurant was not found");
        }

        var restaurantImageUrl = restaurant.RestaurantImagesUrl
            .Where(r => r.Value == request.RestaurantImageUrl)
            .Single();

        await _blobService.DeleteBlobAsync(request.RestaurantImageUrl);

        restaurant.RemoveImage(request.RestaurantImageUrl, restaurantImageUrl.Id);

        await _restaurantRepository.UpdateAsync(restaurant);

        return Unit.Value;
    }
}
