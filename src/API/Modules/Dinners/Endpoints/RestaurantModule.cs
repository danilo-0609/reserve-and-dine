using API.Configuration;
using API.Modules.Dinners.Requests.Restaurants;
using Carter;
using Dinners.Application.Restaurants.Administration.Add;
using Dinners.Application.Restaurants.Administration.Delete;
using Dinners.Application.Restaurants.Administration.GetRestaurantAdministrationById;
using Dinners.Application.Restaurants.Administration.Update;
using Dinners.Application.Restaurants.AvailableTables;
using Dinners.Application.Restaurants.ChangeLocalization;
using Dinners.Application.Restaurants.Close;
using Dinners.Application.Restaurants.Delete;
using Dinners.Application.Restaurants.GetById;
using Dinners.Application.Restaurants.GetByLocalization;
using Dinners.Application.Restaurants.GetByName;
using Dinners.Application.Restaurants.ModifySchedule;
using Dinners.Application.Restaurants.Open;
using Dinners.Application.Restaurants.Post;
using Dinners.Application.Restaurants.Rate.Delete;
using Dinners.Application.Restaurants.Rate.GetByRestaurantId;
using Dinners.Application.Restaurants.Rate.Publish;
using Dinners.Application.Restaurants.Rate.Upgrade;
using Dinners.Application.Restaurants.RestaurantImages.Get;
using Dinners.Application.Restaurants.RestaurantImages.Insert;
using Dinners.Application.Restaurants.RestaurantImages.Remove;
using Dinners.Application.Restaurants.Tables.Add;
using Dinners.Application.Restaurants.Tables.Delete;
using Dinners.Application.Restaurants.Tables.Get;
using Dinners.Application.Restaurants.Tables.Update;
using Dinners.Application.Restaurants.UpdateContact;
using Dinners.Application.Restaurants.UpdateInformation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Modules.Dinners.Endpoints;

public sealed class RestaurantModule : CarterModule
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RestaurantModule(IHttpContextAccessor contextAccessor)
        : base("/restaurants")
    {
        _httpContextAccessor = contextAccessor;
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/administrations/{id}", async (
            Guid id,
            [FromBody] AddRestaurantAdminRequest request,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new AddAdministrationCommand(id,
                request.Name,
                request.AdministratorTitle,
                request.NewAdministratorId));

            return command.Match(
                onValue => Results.Created(onValue.ToString(), onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapDelete("/administrations/{id}", async (
            Guid id,
            [FromQuery] Guid administratorId,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new DeleteAdministrationCommand(id, administratorId));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPut("/administrations/{id}", async (
            Guid id,
            [FromBody] UpdateRestaurantAdminRequest request,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new UpdateAdministrationCommand(id,
                request.Name,
                request.AdministratorTitle,
                request.AdministratorId));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapGet("/administrations/{id}", async (
            Guid id,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var query = await sender.Send(new GetRestaurantAdministrationByIdQuery(id));

            return query.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPut("/tables/available/{id}", async (
            Guid id,
            [FromBody] string availableTablesStatus,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new ModifyAvailableTableStatusCommand(id, availableTablesStatus));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPut("/localization/{id}", async (
            Guid id,
            [FromBody] ChangeRestaurantLocalizationRequest request,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new ChangeRestaurantLocalizationCommand(id,
                request.Country,
                request.City,
                request.Region,
                request.Neighborhood,
                request.Address,
                request.LocalizationDetails));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPut("/close/{id}", async (
            Guid id,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new CloseRestaurantCommand(id));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapDelete("/{id}", async (
            Guid id,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new DeleteRestaurantCommand(id));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapGet("/{id}", async (
            Guid id,
            [FromServices] ISender sender) =>
        {
            var query = await sender.Send(new GetRestaurantByIdQuery(id));

            return query.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapGet("/localization", async (
            [FromQuery] string country,
            [FromQuery] string city,
            [FromQuery] string region,
            [FromQuery] string? neighborhood,
            [FromServices] ISender sender) =>
        {
            var query = await sender.Send(new GetRestaurantsByLocalizationQuery(country,
                city,
                region,
                neighborhood));

            return query.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapGet("/", async (
            [FromQuery] string name,
            [FromServices] ISender sender) =>
        {
            var query = await sender.Send(new GetRestaurantsByNameQuery(name));

            return query.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPut("/schedule/{id}", async (
            Guid id,
            [FromBody] ModifyRestaurantScheduleRequest request,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new ModifyRestaurantScheduleCommand(id, request.Day, request.Start, request.End));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPut("/open/{id}", async (
            Guid id,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new OpenRestaurantCommand(id));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPut("/contact/{id}", async (
            Guid id,
            [FromBody] UpdateRestaurantContactRequest request,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new UpdateRestaurantContactCommand(id,
                request.Email,
                request.Whatsapp,
                request.Facebook,
                request.PhoneNumber,
                request.Instagram,
                request.Twitter,
                request.TikTok,
                request.Website));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPut("/information/{id}", async (
            Guid id,
            [FromBody] UpdateRestaurantInformationRequest request,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new UpdateInformationCommand(id,
                request.Title,
                request.Description,
                request.Type));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPost("/", async (
            [FromBody] PostRestaurantRequest request,
            [FromServices] ISender sender) =>
        {
            var command = await sender.Send(new PostRestaurantCommand(request.RestaurantInformation,
                request.RestaurantLocalization,
                request.RestaurantSchedules,
                request.RestaurantTables,
                request.RestaurantAdministrations,
                request.RestaurantContact,
                request.Chefs,
                request.Specialties));

            return command.Match(
                onValue => Results.Created(onValue.ToString(), onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapDelete("/rate/{id}", async (
            Guid id,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new DeleteRateCommand(id));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapGet("rate/{id}", async (
            Guid id,
            [FromServices] ISender sender) =>
        {
            var query = await sender.Send(new GetRateByRestaurantIdQuery(id));

            return query.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPost("{id}/rate", async (
            Guid id,
            [FromBody] RateRestaurantRequest request,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new RateRestaurantCommand(id,
                request.Stars,
                request.Comment));

            return command.Match(
                onValue => Results.Created(onValue.ToString(), onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPut("/rate/{id}", async (
            Guid id,
            [FromBody] UpdateRestaurantRateRequest request,
            [FromServices] ISender sender) =>
        {
            var command = await sender.Send(new UpgradeRateCommand(id,
                request.Stars,
                request.Comment));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapGet("/images/{restaurantId}/{imageId}", async (
            Guid restaurantId,
            Guid imageId,
            [FromServices] ISender sender) =>
        {
            var query = await sender.Send(new GetRestaurantImageQuery(restaurantId, imageId));

            return query.Match(
                onValue => Results.File(onValue.Content!, onValue.ContentType),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPost("/images/{id}", async (
            [FromForm] IFormFile file,
            Guid id,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var filePath = await GetFilePath(file);

            var command = await sender.Send(new InsertRestaurantImagesCommand(id,
                file,
                filePath));

            return command.Match(
                onValue => Results.Created(onValue.ToString(), onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapDelete("/images/{restaurantId}/{imageId}", async (
            Guid restaurantId,
            Guid imageId,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new RemoveRestaurantImageCommand(restaurantId, imageId));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapPost("{id}/tables", async (
            Guid id,
            [FromBody] AddTableRequest request,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new AddTableCommand(id,
                request.Number,
                request.Seats,
                request.IsPremium));

            return command.Match(
                onValue => Results.Created(onValue.ToString(), onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapDelete("{id}/tables/{number}", async (
            Guid id,
            int number,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new DeleteTableCommand(id, number));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        app.MapGet("{id}/tables", async (
            Guid id,
            [FromServices] ISender sender) =>
        {
            var query = await sender.Send(new GetTablesByRestaurantIdQuery(id));

            return query.Match(
                onValue => Results.Ok(onValue),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        });

        app.MapPut("{id}/tables", async (
            Guid id,
            Guid tableId,
            [FromBody] UpdateTableRequest request,
            [FromServices] ISender sender,
            [FromServices] IAuthorizationService authorizationService) =>
        {
            var command = await sender.Send(new UpgradeTableCommand(id,
                request.Number,
                request.Seats,
                request.IsPremium));

            return command.Match(
                onValue => Results.NoContent(),
                onError => new ProblemError(_httpContextAccessor).Errors(onError));
        })
            .RequireAuthorization();

        static async Task<string> GetFilePath(IFormFile file)
        {
            string filePath = Path.GetTempFileName();

            using (var stream = File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }
    }
}
