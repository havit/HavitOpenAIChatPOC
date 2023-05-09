using Blazored.LocalStorage;
using Havit.OpenAIChatPOC.Contracts.Infrastructure;
using Havit.OpenAIChatPOC.Web.Client.Resources;
using Havit.OpenAIChatPOC.Web.Client.Resources.Pages.Admin;
using Microsoft.AspNetCore.Components;

namespace Havit.OpenAIChatPOC.Web.Client.Pages.Admin;

public partial class AdminIndex : ComponentBase
{
	[Inject] protected IMaintenanceFacade MaintenanceFacade { get; set; }
	[Inject] protected IHxMessengerService Messenger { get; set; }
	[Inject] protected IHxMessageBoxService MessageBox { get; set; }
	[Inject] protected ILocalStorageService LocalStorageService { get; set; }
	[Inject] protected INavigationLocalizer NavigationLocalizer { get; set; }
	[Inject] protected IAdminIndexLocalizer AdmninIndexLocalizer { get; set; }
	[Inject] protected NavigationManager NavigationManager { get; set; }

	private async Task RemoveCultureFromLocalStorage()
	{
		if (await MessageBox.ConfirmAsync("Do you really want to remove culture cache?"))
		{
			await LocalStorageService.RemoveItemAsync("culture");
			Messenger.AddInformation(AdmninIndexLocalizer["CultureRemoved"]); // TODO Just a demo
		}
	}

	private async Task HandleClearCache()
	{
		if (await MessageBox.ConfirmAsync("Do you really want to clear server cache?"))
		{
			await MaintenanceFacade.ClearCache();

			if (await MessageBox.ConfirmAsync($"Server cache cleared. Do you want to reload the Blazor client?"))
			{
				NavigationManager.NavigateTo("", forceLoad: true);
			}
		}
	}
}
