namespace Dinners.Application.Restaurants.Post.Requests;

public sealed record RestaurantTableRequest(int Number,
        int Seats,
        bool IsPremium);
