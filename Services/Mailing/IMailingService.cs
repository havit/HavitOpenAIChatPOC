using MimeKit;

namespace Havit.OpenAIChatPOC.Services.Mailing;

public interface IMailingService
{
	Task VerifyHealthAsync(CancellationToken cancellationToken = default);

	Task SendAsync(MimeMessage mailMessage, CancellationToken cancellationToken = default);
}
