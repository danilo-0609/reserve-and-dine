using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Domain.Restaurants;
using ErrorOr;

namespace Dinners.Application.Restaurants.GetRestaurantAdministrationById;

internal sealed class GetRestaurantAdministrationByIdQueryHandler : IQueryHandler<GetRestaurantAdministrationByIdQuery, ErrorOr<IReadOnlyList<RestaurantAdministrationResponse>>>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public GetRestaurantAdministrationByIdQueryHandler(IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<IReadOnlyList<RestaurantAdministrationResponse>>> Handle(GetRestaurantAdministrationByIdQuery request, CancellationToken cancellationToken)
    {
        Restaurant? restaurant = await _restaurantRepository.GetRestaurantById(RestaurantId.Create(request.RestaurantId));
    
        if (restaurant is null)
        {
            return RestaurantErrorCodes.NotFound;
        }

        if (!restaurant.RestaurantAdministrations.Any(r => r.AdministratorId == _executionContextAccessor.UserId))
        {
            return RestaurantErrorCodes.CannotAccessToAdministrationContent;
        }

        return restaurant
            .RestaurantAdministrations
            .ToList()
            .ConvertAll(admin => 
                new RestaurantAdministrationResponse(admin.Name, 
                    admin.AdministratorId, 
                    admin.AdministratorTitle)).AsReadOnly();
    }
}
