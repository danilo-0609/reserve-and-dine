using BuildingBlocks.Domain.Rules;
using ErrorOr;

namespace Dinners.Domain.Menus.MenuReviews.Rules;

internal sealed class MenuReviewMustBeANumberBetweenZeroToFiveRule : IBusinessRule
{
    private readonly decimal _rate;

    public MenuReviewMustBeANumberBetweenZeroToFiveRule(decimal rate)
    {
        _rate = rate;
    }

    public Error Error => throw new NotImplementedException();

    public bool IsBroken() => _rate > 5 || _rate < 0;

    public static string Message => "Menu review must be a number between zero to five";
}
