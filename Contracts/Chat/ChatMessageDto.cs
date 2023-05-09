namespace Havit.OpenAIChatPOC.Contracts.Chat;

public class ChatMessageDto
{
	public ChatRoleStub Role { get; set; }
	public string Content { get; set; }
}