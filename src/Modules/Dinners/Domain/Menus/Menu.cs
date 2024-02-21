using BuildingBlocks.Domain.AggregateRoots;

namespace Dinners.Domain.Menus;

public sealed class Menu : AggregateRoot<MenuId, Guid>
{
    private readonly List<MenuReviews> _menuReviews = new();

    public new MenuId Id { get; private set; }

    public MenuSpecification MenuSpecification { get; private set; }

    public DishSpecification DishSpecification { get; private set; }

    public IReadOnlyList<MenuReviews> MenuReviews => _menuReviews.AsReadOnly();
    
    public MenuSchedule MenuSchedule { get; private set; }

    public DateTime CreatedOn { get; private set; }

    public DateTime UpdatedOn { get; private set; }

    public Menu(List<MenuReviews> menuReviews, 
        MenuId id, 
        MenuSpecification menuSpecification, 
        DishSpecification dishSpecification, 
        MenuSchedule menuSchedule, 
        DateTime createdOn, 
        DateTime updatedOn)
    {
        _menuReviews = menuReviews;
        Id = id;
        MenuSpecification = menuSpecification;
        DishSpecification = dishSpecification;
        MenuSchedule = menuSchedule;
        CreatedOn = createdOn;
        UpdatedOn = updatedOn;
    }

    private Menu() { }
}
