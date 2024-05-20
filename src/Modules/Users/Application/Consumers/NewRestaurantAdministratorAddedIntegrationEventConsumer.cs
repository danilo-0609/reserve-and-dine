using Dinners.IntegrationEvents;
using MassTransit;
using Users.Application.Common;
using Users.Domain.Users;

namespace Users.Application.Consumers;

internal sealed class NewRestaurantAdministratorAddedIntegrationEventConsumer : IConsumer<NewRestaurantAdministratorAddedIntegrationEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NewRestaurantAdministratorAddedIntegrationEventConsumer(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public Task Consume(ConsumeContext<NewRestaurantAdministratorAddedIntegrationEvent> context)
    {
        return Task.CompletedTask;
    }
}
