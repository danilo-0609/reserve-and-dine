using BuildingBlocks.Application;
using Dinners.Application.Common;
using Dinners.Domain.Menus;
using Dinners.Domain.Reservations;
using Dinners.Domain.Restaurants;
using Dinners.Domain.Restaurants.Errors;
using Dinners.Domain.Restaurants.RestaurantTables;
using ErrorOr;

namespace Dinners.Application.Reservations.Request;

internal sealed class RequestReservationCommandHandler : ICommandHandler<RequestReservationCommand, ErrorOr<Guid>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IExecutionContextAccessor _executionContextAccessor;

    public RequestReservationCommandHandler(IReservationRepository reservationRepository, IRestaurantRepository restaurantRepository, IExecutionContextAccessor executionContextAccessor)
    {
        _reservationRepository = reservationRepository;
        _restaurantRepository = restaurantRepository;
        _executionContextAccessor = executionContextAccessor;
    }

    public async Task<ErrorOr<Guid>> Handle(RequestReservationCommand request, CancellationToken cancellationToken)
    {
        bool restaurant = await _restaurantRepository.ExistsAsync(RestaurantId.Create(request.RestaurantId));

        if (restaurant is false)
        {
            return RestaurantErrorCodes.NotFound;
        }

        List<RestaurantTable> restaurantTables = await _restaurantRepository.GetRestaurantTablesById(RestaurantId.Create(request.RestaurantId), cancellationToken);

        var reservationInformation = ReservationInformation.Create(request.ReservedTable,
            request.Price,
            request.Currency,
            request.Start,
            request.End,
            request.ReservationDateTime);

        var reservationAttendees = ReservationAttendees.Create(_executionContextAccessor.UserId,
            request.Name,
            request.NumberOfAttendees);

        if (!restaurantTables.Any(r => r.Number == reservationInformation.ReservedTable))
        {
            return Error.NotFound("Reservation.TableNotFound", "The table was not found");
        }

        List<int> availableTables = restaurantTables
            .Where(r => !r.ReservedHours
                .ContainsKey(reservationInformation.ReservationDateTime)
                || r.ReservedHours
                    .Values
                    .Any(t => t.Start.Hours <= reservationInformation.ReservationDateTime.Hour
                     && t.End.Hours >= reservationInformation.ReservationDateTime.Hour))
            .Select(r => r.Number).ToList();

        var reservation = Reservation.Request(reservationInformation,
            availableTables,   
            restaurantTables.Where(g => g.Number == reservationInformation.ReservedTable)
                            .Select(g => g.Seats)
                            .SingleOrDefault(),
            RestaurantId.Create(request.RestaurantId),
            reservationAttendees,
            request.MenuIds.ConvertAll(menuId => MenuId.Create(menuId)));
    
        if (reservation.IsError)
        {
            return reservation.FirstError;
        }

        await _reservationRepository.AddAsync(reservation.Value, cancellationToken);

        return reservation.Value.Id.Value;
    }
}
