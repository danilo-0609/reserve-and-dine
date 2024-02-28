using BuildingBlocks.Domain.Rules;
using Dinners.Domain.Menus.MenuReviews.Errors;
using ErrorOr; 

namespace Dinners.Domain.Menus.MenuReviews.Rules;

internal sealed class MenuCannotBeReviewedWhenUserHasNotConsumedItRule : IBusinessRule
{
    private readonly List<MenuConsumer> _menuConsumers;
    private readonly Guid _clientId;

    public MenuCannotBeReviewedWhenUserHasNotConsumedItRule(List<MenuConsumer> menuConsumers, Guid clientId)
    {
        _menuConsumers = menuConsumers;
        _clientId = clientId;
    }


    public Error Error => MenuReviewsErrorCodes.CannotReviewWhenUserHasNotConsumedTheMenu;

    public bool IsBroken() => !_menuConsumers.Any(r => r.ClientId == _clientId);

    public static string Message => "Menu cannot be reviewed when user has not consumed the menu";
}
