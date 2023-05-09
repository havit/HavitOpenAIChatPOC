namespace Havit.OpenAIChatPOC.Contracts.Chat;

public class ChatRequestDto
{
	public List<ChatMessageDto> Messages { get; set; } = new();
}