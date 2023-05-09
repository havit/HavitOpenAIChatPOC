using Havit.OpenAIChatPOC.Contracts.Infrastructure;
using Havit.OpenAIChatPOC.Resources;
using Microsoft.AspNetCore.Components;

namespace Havit.OpenAIChatPOC.Web.Client;

public partial class App
{
	[Inject] protected IFluentValidationDefaultMessagesLocalizer ValidationMessagesLocalizer { get; set; }

	protected override void OnInitialized()
	{
		// we cannot use IStringLocalizer in application startup class (locks the culture to the invariant one)
		FluentValidationLocalizationHelper.RegisterDefaultValidationMessages(this.ValidationMessagesLocalizer);
	}
}