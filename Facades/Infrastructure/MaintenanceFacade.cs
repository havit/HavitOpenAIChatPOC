using Havit.Extensions.DependencyInjection.Abstractions;
using Havit.OpenAIChatPOC.Contracts.Infrastructure;
using Havit.Services.Caching;

namespace Havit.OpenAIChatPOC.Facades.Infrastructure;

[Service]
public class MaintenanceFacade : IMaintenanceFacade
{
	private readonly ICacheService cacheService;

	public MaintenanceFacade(ICacheService cacheService)
	{
		this.cacheService = cacheService;
	}

	public Task ClearCache(CancellationToken cancellationToken = default)
	{
		cacheService.Clear();

		return Task.CompletedTask;
	}
}
