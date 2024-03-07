using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Dinners.Domain.Restaurants.RestaurantRatings.Events;
using ErrorOr;

namespace Dinners.Application.Restaurants.Rate.GetByRestaurantId;

internal sealed class GetRateByRestaurantIdQueryHandler : IQueryHandler<GetRateByRestaurantIdQuery, ErrorOr<List<RatingResponse>>>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;

    public GetRateByRestaurantIdQueryHandler(IRestaurantRatingRepository restaurantRatingRepository)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
    }

    public async Task<ErrorOr<List<RatingResponse>>> Handle(GetRateByRestaurantIdQuery request, CancellationToken cancellationToken)
    {
        List<RestaurantRating> ratings = await _restaurantRatingRepository.GetRatingsByRestaurantId(RestaurantId.Create(request.RestaurantId), cancellationToken);
    
        if (!ratings.Any())
        {
            return RestaurantRatingErrorsCodes.NotFoundByRestaurantId;
        }

        List<RatingResponse> ratingsResponse = ratings.ConvertAll(rating =>
        {
            return new RatingResponse(rating.Id.Value, 
                rating.Stars, 
                rating.Comment, 
                rating.RatedAt, 
                rating.UpdatedAt);
        });

        return ratingsResponse;
    }
}
