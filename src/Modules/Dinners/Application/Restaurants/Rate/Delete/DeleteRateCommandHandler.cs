using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants.RestaurantRatings;
using Dinners.Domain.Restaurants.RestaurantRatings.Events;
using ErrorOr;
using MediatR;

namespace Dinners.Application.Restaurants.Rate.Delete;

internal sealed class DeleteRateCommandHandler : ICommandHandler<DeleteRateCommand, ErrorOr<Unit>>
{
    private readonly IRestaurantRatingRepository _restaurantRatingRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public DeleteRateCommandHandler(IRestaurantRatingRepository restaurantRatingRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRatingRepository = restaurantRatingRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteRateCommand request, CancellationToken cancellationToken)
    {
        RestaurantRating? rating = await _restaurantRatingRepository.GetByIdAsync(RestaurantRatingId.Create(request.RatingId), cancellationToken);
    
        if (rating is null)
        {
            return RestaurantRatingErrorsCodes.NotFound;
        }

        if (_executionContextAccessor.UserId != rating.ClientId)
        {
            return RestaurantRatingErrorsCodes.CannotDeleteRating;
        }

        await _restaurantRatingRepository.DeleteAsync(rating.Id, cancellationToken);

        return Unit.Value;
    }
}
