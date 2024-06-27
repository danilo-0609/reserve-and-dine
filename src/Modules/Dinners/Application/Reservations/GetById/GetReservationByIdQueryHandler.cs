using Dinners.Application.Authorization;
using Dinners.Application.Common;
using Dinners.Domain.Reservations;
using Dinners.Domain.Reservations.Errors;
using Dinners.Domain.Restaurants;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Dinners.Application.Reservations.GetById;

internal sealed class GetReservationByIdQueryHandler : IQueryHandler<GetReservationByIdQuery, ErrorOr<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetReservationByIdQueryHandler(IReservationRepository reservationRepository, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
    {
        _reservationRepository = reservationRepository;
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ErrorOr<ReservationResponse>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        Reservation? reservation = await _reservationRepository.GetByIdAsync(ReservationId.Create(request.ReservationId), cancellationToken);
    
        if (reservation is null)
        {
            return ReservationErrorsCodes.NotFound;
        }

        var onlyReservationClientAndRestaurantAdministratorCanGetTheReservationRequirement = await _authorizationService
            .AuthorizeAsync(
                _httpContextAccessor.HttpContext!.User,
                new Tuple<RestaurantId, Guid>(
                    reservation.RestaurantId, reservation.ReservationAttendees.ClientId),
                Policy.CanGetReservation);

        if (onlyReservationClientAndRestaurantAdministratorCanGetTheReservationRequirement.Succeeded is false)
        {
            return ReservationErrorsCodes.UserIsNotAllowedToGetOrAccessContent;
        }

        ReservationInformationResponse reservationInformation = new(reservation.ReservationInformation.ReservedTable,
            reservation.ReservationInformation.TimeOfReservation,
            reservation.ReservationInformation.ReservationDateTime);

        ReservationAttendeesResponse reservationAttendees = new(reservation.ReservationAttendees.ClientId,
            reservation.ReservationAttendees.Name,
            reservation.ReservationAttendees.NumberOfAttendees);

        ReservationResponse reservationResponse = new(reservation.Id.Value,
            reservationInformation,
            reservation.RestaurantId.Value,
            reservationAttendees,
            reservation.ReservationStatus.Value,
            reservation.MenuIds.ConvertAll(r => r.Value));

        return reservationResponse;
    }
}
