using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Domain.Restaurants;
using ErrorOr;

namespace Dinners.Application.Restaurants.Rate.Publish;

internal sealed class RateRestaurantCommandHandler : ICommandHandler<RateRestaurantCommand, ErrorOr<Guid>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public RateRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IRestaurantRatingRepository restaurantRatingRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _restaurantRatingRepository = restaurantRatingRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Guid>> Handle(RateRestaurantCommand request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));

        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        ErrorOr<RestaurantRating> rating = restaurant.Rate(request.Stars,
            _executionContextAccessor.UserId,
            request.Comment);
    
        if (rating.IsError)
        {
            return rating.FirstError;
        }

        await _restaurantRatingRepository.AddAsync(rating.Value, cancellationToken);

        return rating.Value.Id.Value;
    }
}
