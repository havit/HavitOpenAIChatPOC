using Havit.Data.Patterns.DataSeeds;
using Havit.OpenAIChatPOC.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.NewProjectTemplate.TestHelpers;

public class IntegrationTestBase
{
	protected IServiceProvider ServiceProvider { get; private set; }

	protected virtual bool UseLocalDb => false;
	protected virtual bool DeleteDbData => true;

	protected virtual bool SeedData => true;

	private ServiceProvider serviceProvider;

	[TestInitialize]
	public virtual void TestInitialize()
	{
		IServiceCollection services = CreateServiceCollection();
		serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions
		{
			ValidateOnBuild = true,
			ValidateScopes = true
		});

		this.ServiceProvider = serviceProvider.CreateScope().ServiceProvider;
	}

	[TestCleanup]
	public virtual void TestCleanup()
	{
		((IDisposable)ServiceProvider)?.Dispose();
		serviceProvider?.Dispose();
	}

	protected virtual IServiceCollection CreateServiceCollection()
	{
		IServiceCollection services = new ServiceCollection();
		services.ConfigureForTests(useInMemoryDb: !UseLocalDb);

		return services;
	}
}
