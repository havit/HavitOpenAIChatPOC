using Havit.OpenAIChatPOC.Model.Security;

namespace Havit.OpenAIChatPOC.DataLayer.Repositories.Security;

public partial interface IUserRepository
{
	Task<User> GetByIdentityProviderIdAsync(string identityProviderId, CancellationToken cancellationToken = default);
}
