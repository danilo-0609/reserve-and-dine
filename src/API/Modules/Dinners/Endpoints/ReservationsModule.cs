﻿using API.Configuration;
using API.Modules.Dinners.Requests.Reservations;
using Carter;
using Dinners.Application.Reservations.Cancel;
using Dinners.Application.Reservations.Finish;
using Dinners.Application.Reservations.GetById;
using Dinners.Application.Reservations.Request;
using Dinners.Application.Reservations.Visit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Modules.Dinners.Endpoints;

public sealed class ReservationsModule : CarterModule
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReservationsModule(IHttpContextAccessor httpContextAccessor)
        : base("/reservations")
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", async (
            Guid id,
            [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetReservationByIdQuery(id));

            return result.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPost("/request", async (
            [FromBody] RequestReservationRequest request,
            [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new RequestReservationCommand(request.ReservedTable,
                request.Start,
                request.End,
                request.ReservationDateTime,
                request.RestaurantId,
                request.Name,
                request.NumberOfAttendees,
                request.MenuIds));

            return result.Match(
                onValue => Results.Created(onValue.ToString(), onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPut("/cancel/{id}", async (
            Guid id,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var result = await sender.Send(new CancelReservationCommand(id));

            return result.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPatch("/assist/{id}", async (
            Guid id,
            [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new VisitReservationCommand(id));

            return result.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPut("/finish/{id}", async (
            Guid id,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var result = await sender.Send(new FinishReservationCommand(id));

            return result.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();
    }
}
