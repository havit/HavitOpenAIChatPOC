using Havit.ComponentModel;

namespace Havit.OpenAIChatPOC.Contracts.Infrastructure;

[ApiContract]
public interface IMaintenanceFacade
{
	Task ClearCache(CancellationToken cancellationToken = default);
}
