using Havit.Blazor.Grpc.Server;
using Havit.OpenAIChatPOC.Contracts;
using Havit.OpenAIChatPOC.Contracts.Infrastructure;
using Havit.OpenAIChatPOC.DependencyInjection;
using Havit.OpenAIChatPOC.Services.HealthChecks;
using Havit.OpenAIChatPOC.Web.Server.Infrastructure.ApplicationInsights;
using Havit.OpenAIChatPOC.Web.Server.Infrastructure.ConfigurationExtensions;
using Havit.OpenAIChatPOC.Web.Server.Infrastructure.HealthChecks;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using ProtoBuf.Grpc.Server;

namespace Havit.OpenAIChatPOC.Web.Server;

public class Startup
{
	private readonly IConfiguration configuration;

	public Startup(IConfiguration configuration)
	{
		this.configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.ConfigureForWebServer(configuration);

		services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

		services.AddDatabaseDeveloperPageExceptionFilter();

		services.AddOptions();

		services.AddCustomizedMailing(configuration);

		// SmtpExceptionMonitoring to errors@havit.cz
		services.AddExceptionMonitoring(configuration);

		// Application Insights
		services.AddApplicationInsightsTelemetry(configuration);
		services.AddSingleton<ITelemetryInitializer, GrpcRequestStatusTelemetryInitializer>();
		services.AddSingleton<ITelemetryInitializer, EnrichmentTelemetryInitializer>();
		services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });

		// server-side UI
		services.AddControllersWithViews();
		services.AddRazorPages();

		// gRPC
		services.AddGrpcServerInfrastructure(assemblyToScanForDataContracts: typeof(Dto).Assembly);
		services.AddCodeFirstGrpcReflection();

		// Health checks
		TimeSpan defaultHealthCheckTimeout = TimeSpan.FromSeconds(10);
		services.AddHealthChecks()
			.AddCheck<MailServiceHealthCheck>("SMTP", timeout: defaultHealthCheckTimeout);
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseMigrationsEndPoint();
			app.UseWebAssemblyDebugging();
		}
		else
		{
			app.UseExceptionHandler("/error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			// TODO app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseBlazorFrameworkFiles();
		app.UseStaticFiles();

		app.UseExceptionMonitoring();

		app.UseRouting();

		app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapRazorPages();
			endpoints.MapControllers();
			endpoints.MapFallbackToPage("/_Host");

			endpoints.MapGrpcServicesByApiContractAttributes(typeof(IMaintenanceFacade).Assembly);
			endpoints.MapCodeFirstGrpcReflectionService();

			endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
			{
				AllowCachingResponses = false,
				ResponseWriter = HealthCheckWriter.WriteResponse
			});
		});
	}
}
