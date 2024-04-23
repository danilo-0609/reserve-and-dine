using Dinners.Application.Common;
using Dinners.Infrastructure;
using Dinners.Infrastructure.Blobs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Dinners.Tests.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly DinnersDbContext DbContext;
    protected readonly IRestaurantBlobService RestaurantBlobService;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();    
    
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<DinnersDbContext>();
        RestaurantBlobService = _scope.ServiceProvider.GetRequiredService<IRestaurantBlobService>();
    }
}
