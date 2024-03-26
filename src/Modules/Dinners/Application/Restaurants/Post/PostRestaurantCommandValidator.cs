using Dinners.Application.Restaurants.Post.Requests;
using FluentValidation;

namespace Dinners.Application.Restaurants.Post;

internal sealed class PostRestaurantCommandValidator : AbstractValidator<PostRestaurantCommand>
{
    public PostRestaurantCommandValidator()
    {
        RuleFor(r => r.RestaurantInformation)
            .NotEmpty().NotNull();

        RuleFor(r => r.RestaurantLocalization)
            .NotEmpty().NotNull();

        RuleForEach(r => r.RestaurantSchedules)
            .NotEmpty().WithMessage("Restaurant schedules list cannot be empty")
            .NotNull()
            .SetValidator(new RestaurantScheduleRequestValidator());

        RuleFor(r => r.RestaurantSchedules)
            .Must(r => r.Count == 7).WithMessage("Restaurant schedules list must contain exactly 7 entries")
            .Must(HaveAllDaysOfWeek).WithMessage("Restaurant schedules list must include every day of the week");

        RuleFor(r => r.RestaurantTables)
            .NotEmpty().NotNull();
        
        RuleFor(r => r.RestaurantAdministrations)
            .NotEmpty().NotNull();  

        RuleFor(r => r.RestaurantContact)
            .NotEmpty().NotNull();
    }

    private bool HaveAllDaysOfWeek(List<RestaurantScheduleRequest> scheduleRequests)
    {
        var allDaysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();

        var distinctDays = scheduleRequests.Select(s => s.Day).Distinct().ToList();

        return allDaysOfWeek.All(day => distinctDays.Contains(day));
    }
}
