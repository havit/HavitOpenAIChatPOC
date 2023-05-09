using System.IO;
using System.Runtime.CompilerServices;
using Azure.Identity;
using Havit.Extensions.DependencyInjection;
using Havit.Extensions.DependencyInjection.Abstractions;
using Havit.OpenAIChatPOC.DependencyInjection.ConfigrationOptions;
using Havit.OpenAIChatPOC.Services.Infrastructure;
using Havit.OpenAIChatPOC.Services.TimeServices;
using Havit.Services.Azure.FileStorage;
using Havit.Services.Caching;
using Havit.Services.FileStorage;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.OpenAIChatPOC.DependencyInjection;

public static class ServiceCollectionExtensions
{
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static IServiceCollection ConfigureForWebServer(this IServiceCollection services, IConfiguration configuration)
	{
		FileStorageOptions fileStorageOptions = configuration.GetSection(FileStorageOptions.FileStorageOptionsKey).Get<FileStorageOptions>();

		InstallConfiguration installConfiguration = new InstallConfiguration
		{
			DatabaseConnectionString = configuration.GetConnectionString("Database"),
			AzureStorageConnectionString = configuration.GetConnectionString("AzureStorageConnectionString"),
			FileStoragePathOrContainerName = fileStorageOptions.PathOrContainerName,
			ServiceProfiles = new[] { ServiceAttribute.DefaultProfile, ServiceProfiles.WebServer },
		};

		return services.ConfigureForAll(installConfiguration);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static IServiceCollection ConfigureForJobsRunner(this IServiceCollection services, IConfiguration configuration)
	{
		InstallConfiguration installConfiguration = new InstallConfiguration
		{
			DatabaseConnectionString = configuration.GetConnectionString("Database"),
			AzureStorageConnectionString = configuration.GetConnectionString("AzureStorageConnectionString"),
			ServiceProfiles = new[] { ServiceAttribute.DefaultProfile, ServiceProfiles.JobsRunner }
		};

		return services.ConfigureForAll(installConfiguration);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static IServiceCollection ConfigureForTests(this IServiceCollection services, bool useInMemoryDb = true)
	{
		string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Developement";

		IConfigurationRoot configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json")
			.AddJsonFile($"appsettings.{environment}.json", true)
			.AddJsonFile($"appsettings.{environment}.local.json", true) // .gitignored
			.Build();

		InstallConfiguration installConfiguration = new InstallConfiguration
		{
			DatabaseConnectionString = configuration.GetConnectionString("Database"),
			ServiceProfiles = new[] { ServiceAttribute.DefaultProfile },
			UseInMemoryDb = useInMemoryDb,
		};

		services.AddSingleton<IConfiguration>(configuration);
		return services.ConfigureForAll(installConfiguration);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static IServiceCollection ConfigureForAll(this IServiceCollection services, InstallConfiguration installConfiguration)
	{
		InstallHavitServices(services);
		InstallByServiceAttribute(services, installConfiguration);

		services.AddMemoryCache();

		return services;
	}

	private static void InstallHavitServices(IServiceCollection services)
	{
		// HAVIT .NET Framework Extensions
		services.AddSingleton<ITimeService, ApplicationTimeService>();
		services.AddSingleton<ICacheService, MemoryCacheService>();
		services.AddSingleton(new MemoryCacheServiceOptions { UseCacheDependenciesSupport = false });
	}

	private static void InstallByServiceAttribute(IServiceCollection services, InstallConfiguration configuration)
	{
		services.AddByServiceAttribute(typeof(Havit.OpenAIChatPOC.Services.Properties.AssemblyInfo).Assembly, configuration.ServiceProfiles);
		services.AddByServiceAttribute(typeof(Havit.OpenAIChatPOC.Facades.Properties.AssemblyInfo).Assembly, configuration.ServiceProfiles);
	}

	internal static void InstallFileStorageService<TFileStorageService, TFileStorageImplementation, TFileStorageContext>(IServiceCollection services, string azureStorageConnectionString, string storagePath)
		where TFileStorageService : class, IFileStorageService<TFileStorageContext> // class zde znamená i interface! // např. IDocumentStorageService
		where TFileStorageImplementation : FileStorageWrappingService<TFileStorageContext>, TFileStorageService // např. DocumentStorageService
		where TFileStorageContext : FileStorageContext // např. DocumentStorage
	{
		services.AddFileStorageWrappingService<TFileStorageService, TFileStorageImplementation, TFileStorageContext>();

		if (!String.IsNullOrEmpty(azureStorageConnectionString))
		{
			services.AddAzureBlobStorageService<TFileStorageContext>(new AzureBlobStorageServiceOptions<TFileStorageContext>
			{
				BlobStorage = azureStorageConnectionString,
				ContainerName = storagePath,
				TokenCredential = new DefaultAzureCredential()
			});
		}
		else
		{
			services.AddFileSystemStorageService<TFileStorageContext>(storagePath);
		}
	}
}
