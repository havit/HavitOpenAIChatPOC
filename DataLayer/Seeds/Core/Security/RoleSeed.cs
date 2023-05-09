using Havit.Data.Patterns.DataSeeds;
using Havit.OpenAIChatPOC.Model.Security;
using Havit.OpenAIChatPOC.Primitives.Model.Security;

namespace Havit.OpenAIChatPOC.DataLayer.Seeds.Core.Security;

public class RoleSeed : DataSeed<CoreProfile>
{
	public override void SeedData()
	{
		var roles = Enum.GetValues<RoleEntry>().Select(entry => new Role { Id = (int)entry, Name = entry.ToString() }).ToArray();

		Seed(For(roles).PairBy(r => r.Id));
	}
}
