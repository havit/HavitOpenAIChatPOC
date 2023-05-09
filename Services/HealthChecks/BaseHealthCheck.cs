using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Havit.OpenAIChatPOC.Services.HealthChecks;

public abstract class BaseHealthCheck : IHealthCheck
{
	async Task<HealthCheckResult> IHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
	{
		try
		{
			return await this.CheckHealthAsync(cancellationToken);
		}
		catch (Exception exception)
		{
			return HealthCheckResult.Unhealthy(exception: exception);
		}
	}

	protected abstract Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken);
}
