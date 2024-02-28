namespace Dinners.Domain.Restaurants.RestaurantTables;

public sealed record RestaurantTable
{
    public int Number { get; private set; }

    public int Seats { get; private set; }

    public bool IsPremium { get; private set; }

    public bool IsReserved { get; private set; }


    public static RestaurantTable Create(int number,
        int seats,
        bool isPremium,
        bool isReserved)
    {
        return new RestaurantTable(number, seats, isPremium, isReserved);
    }

    public void Reserve()
    {
        IsReserved = true;
    }

    public void FreeTable()
    {
        IsReserved = false;
    }


    private RestaurantTable(int number, int seats, bool isPremium, bool isReserved)
    {
        Number = number;
        Seats = seats;
        IsPremium = isPremium;
        IsReserved = isReserved;
    }

    private RestaurantTable() { }
}
