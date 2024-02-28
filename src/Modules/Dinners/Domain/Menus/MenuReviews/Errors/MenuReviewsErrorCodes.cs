using Dinners.Domain.Menus.MenuReviews.Rules;
using ErrorOr;

namespace Dinners.Domain.Menus.MenuReviews.Errors;

public static class MenuReviewsErrorCodes
{
    public static Error CannotReviewWhenUserHasNotConsumedTheMenu =>
        Error.Validation("MenuReviews.CannotReviewWhenUserHasNotConsumedTheMenu", MenuCannotBeReviewedWhenUserHasNotConsumedItRule.Message);

    public static Error RateIsNotAValidNumber =>
        Error.Validation("MenuReviews.RateIsNotAValidNumber", MenuReviewMustBeANumberBetweenZeroToFiveRule.Message);
}
