using Dinners.Domain.Restaurants.RestaurantInformations;
using Dinners.Domain.Restaurants.RestaurantSchedules;
using Dinners.Domain.Restaurants.RestaurantTables;
using Dinners.Domain.Restaurants.RestaurantUsers;
using Domain.Restaurants;
using MediatR;

namespace Dinners.Tests.UnitTests.Domain;

public sealed class RestaurantTests
{
    private RestaurantInformation RestaurantInformation = RestaurantInformation.Create("La sazón de la negra",
        "Sancochos y comida de mar deliciosas",
        "Restaurante de mar",
        new List<string>() { "Juan Carlos González" },
        new List<string>() { "Bagre", "Sancocho de pescado", "Tilapia" },
        new List<Uri>());

    private RestaurantLocalization RestaurantLocalization = RestaurantLocalization.Create("Colombia",
        "Medellin",
        "Antioquia",
        "La Milagrosa",
        "Carrera 69 N°76 - 89",
        "Al frente de la iglesia del parque");

    private RestaurantSchedule RestaurantSchedule = RestaurantSchedule.Create(
        new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday, DayOfWeek.Wednesday, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday },
        TimeSpan.FromHours(7),
        TimeSpan.FromHours(19));

    private RestaurantContact RestaurantContact => RestaurantContact.Create("lasazondelanegra@gmail.com",
        "whatsapp del restaurant",
        "link de facebook del restaurante",
        "número de teléfono del restaurante",
        "link del instagram del restaurante",
        "link del twitter del restaurante",
        "link del tiktok del restaurante",
        "link del sitio web del restaurante");

    private List<RestaurantTable> RestaurantTables = new List<RestaurantTable>()
    {
        RestaurantTable.Create(1, 4, false, new Dictionary<DateTime, Dinners.Domain.Common.TimeRange>()),
        RestaurantTable.Create(2, 5, false, new Dictionary<DateTime, Dinners.Domain.Common.TimeRange>())
    };

    private List<RestaurantAdministration> RestaurantAdministrations = new List<RestaurantAdministration>()
    {
        RestaurantAdministration.Create("Juan Camilo Orozco", Guid.NewGuid(), "Director de ventas"),
        RestaurantAdministration.Create("Ana María Soto", Guid.NewGuid(), "Director de marketing"),
    };

    private Restaurant CreateRestaurant()
    {
        var restaurant = Restaurant.Post(RestaurantInformation, 
            RestaurantLocalization,
            RestaurantSchedule,
            RestaurantContact,
            RestaurantTables,
            RestaurantAdministrations,
            DateTime.UtcNow);

        return restaurant;
    }

    [Fact]
    public void AddRestaurantClient_Should_AddANewClient_WhenClientIsNew()
    {
        Restaurant restaurant = CreateRestaurant();

        var clientId = Guid.NewGuid();

        restaurant.AddRestaurantClient(clientId);

        bool clientIsAddedToRestaurant = restaurant.RestaurantClients.Any(r => r.ClientId == clientId);
    
        Assert.True(clientIsAddedToRestaurant);
    }
}
