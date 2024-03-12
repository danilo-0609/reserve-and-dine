using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Dinners.Domain.Restaurants.RestaurantRatings.Events;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Rate.Upgrade;

internal sealed class UpgradeRateCommandHandler : ICommandHandler<UpgradeRateCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public UpgradeRateCommandHandler(IRestaurantRatingRepository restaurantRatingRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Unit>> Handle(UpgradeRateCommand request, CancellationToken cancellationToken)
    {
        RestaurantRating? rating = await _restaurantRatingRepository.GetByIdAsync(RestaurantRatingId.Create(request.RateId), cancellationToken);

        if (rating is null)
        {
            return RestaurantRatingErrorsCodes.NotFound;
        }

        ErrorOr<RestaurantRating> updateRating = rating.Update(rating.Id,
            request.Stars,
            _executionContextAccessor.UserId,
            rating.RatedAt,
            DateTime.UtcNow,
            request.Comment);
    
        if (updateRating.IsError)
        {
            return updateRating.FirstError;
        }

        await _restaurantRatingRepository.UpdateAsync(updateRating.Value, cancellationToken);

        return Unit.Value;
    }
}
