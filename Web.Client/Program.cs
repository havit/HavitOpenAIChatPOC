using System.Globalization;
using BlazorApplicationInsights;
using Blazored.LocalStorage;
using FluentValidation;
using Havit.Blazor.Grpc.Client;
using Havit.Blazor.Grpc.Client.ServerExceptions;
using Havit.OpenAIChatPOC.Contracts;
using Havit.OpenAIChatPOC.Contracts.Infrastructure;
using Havit.OpenAIChatPOC.Web.Client.Infrastructure.Grpc;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Havit.OpenAIChatPOC.Web.Client;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);

		builder.RootComponents.Add<App>("app");

		AddLoggingAndApplicationInsights(builder);
		AddHttpClient(builder);

		builder.Services.AddBlazoredLocalStorage();
		builder.Services.AddValidatorsFromAssemblyContaining<Dto<object>>();

		builder.Services.AddHxServices();
		builder.Services.AddHxMessenger();
		builder.Services.AddHxMessageBoxHost();
		Havit.OpenAIChatPOC.Web.Client.Resources.ResourcesServiceCollectionInstaller.AddGeneratedResourceWrappers(builder.Services);
		Havit.OpenAIChatPOC.Resources.ResourcesServiceCollectionInstaller.AddGeneratedResourceWrappers(builder.Services);
		SetHxComponents();

		AddGrpcClient(builder);

		WebAssemblyHost webAssemblyHost = builder.Build();

		await SetLanguage(webAssemblyHost);

		await webAssemblyHost.RunAsync();
	}
	private static void SetHxComponents()
	{
		HxOffcanvas.Defaults.Backdrop = OffcanvasBackdrop.Static;
		HxModal.Defaults.Backdrop = ModalBackdrop.Static;
		HxInputDate.Defaults.CalendarIcon = BootstrapIcon.Calendar3;

		// TODO [OPTIONAL] Setup HxInputDateRange.Defaults.PredefinedRanges here
		//DateTime today = DateTime.Today;
		//DateTime thisMonthStart = new DateTime(today.Year, today.Month, 1);
		//DateTime thisMonthEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
		//DateTime thisYearStart = new DateTime(today.Year, 1, 1);
		//DateTime thisYearEnd = new DateTime(today.Year, 12, 31);

		//HxInputDateRange.Defaults.PredefinedDateRanges = new InputDateRangePredefinedRangesItem[]
		//{
		//	new() { Label = "TTM", DateRange = new DateTimeRange(today.AddMonths(-12).AddDays(1), today) },
		//	new() { Label = "ThisYear", DateRange = new DateTimeRange(thisYearStart, thisYearEnd), ResourceType = typeof(HxInputDateRangePredefinedRanges) },
		//	new() { Label = "ThisMonth", DateRange = new DateTimeRange(thisMonthStart, thisMonthEnd), ResourceType = typeof(HxInputDateRangePredefinedRanges) },
		//};
	}

	public static void AddHttpClient(WebAssemblyHostBuilder builder)
	{
		builder.Services.AddHttpClient("Web.Server", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
		builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Web.Server"));

	}

	private static void AddGrpcClient(WebAssemblyHostBuilder builder)
	{
		builder.Services.AddTransient<IOperationFailedExceptionGrpcClientListener, HxMessengerOperationFailedExceptionGrpcClientListener>();

		builder.Services.AddGrpcClientInfrastructure(assemblyToScanForDataContracts: typeof(Dto).Assembly);

		builder.Services.AddGrpcClientsByApiContractAttributes(typeof(IMaintenanceFacade).Assembly);
	}

	private static async ValueTask SetLanguage(WebAssemblyHost webAssemblyHost)
	{
		var localStorageService = webAssemblyHost.Services.GetService<ILocalStorageService>();

		var culture = await localStorageService.GetItemAsStringAsync("culture");
		if (!String.IsNullOrWhiteSpace(culture))
		{
			var cultureInfo = new CultureInfo(culture);
			CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
			CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
		}
	}

	private static void AddLoggingAndApplicationInsights(WebAssemblyHostBuilder builder)
	{
		builder.Services.AddBlazorApplicationInsights();

		builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(level => (level == LogLevel.Error) || (level == LogLevel.Critical));

#if DEBUG
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif
	}
}
