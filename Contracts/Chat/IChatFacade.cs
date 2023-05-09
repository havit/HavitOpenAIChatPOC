namespace Havit.OpenAIChatPOC.Contracts.Chat;

[ApiContract(RequireAuthorization = false)]
public interface IChatFacade
{
	Task<ChatResponseDto> GetChatResponseNonStreamingAsync(ChatRequestDto requestDto, CancellationToken cancellationToken = default);
}