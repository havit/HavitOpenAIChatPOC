using System.ComponentModel.DataAnnotations;
using Havit.OpenAIChatPOC.Contracts.Chat;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Havit.OpenAIChatPOC.Web.Client.Pages;

public partial class HomeIndex
{
	[Inject] protected IChatFacade ChatFacade { get; set; }
	[Inject] protected IJSRuntime JSRuntime { get; set; }

	private List<ChatMessageDto> messages = new();
	private FormModel userMessageFormModel = new();
	private HxInputText userMessageInputTextComponent;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (messages.Any())
		{
			await JSRuntime.InvokeVoidAsync("scrollToElementId", $"message-{messages.Count - 1}");
		}
		await userMessageInputTextComponent.FocusAsync();
	}

	private async Task HandleValidSubmit()
	{
		messages.Add(new ChatMessageDto()
		{
			Role = ChatRoleStub.User,
			Content = userMessageFormModel.MessageText,
		});
		userMessageFormModel = new();

		var request = new ChatRequestDto();
		request.Messages.AddRange(messages.TakeLast(10));

		try
		{

			var response = await ChatFacade.GetChatResponseNonStreamingAsync(request);

			messages.Add(response.Message);
		}
		catch (OperationFailedException)
		{
			// NOOP - try again (HxMessenger populated with error message)
		}
	}

	private record FormModel
	{
		[Required(ErrorMessage = "Napište svou zprávu.")]
		public string MessageText { get; set; }
	}
}