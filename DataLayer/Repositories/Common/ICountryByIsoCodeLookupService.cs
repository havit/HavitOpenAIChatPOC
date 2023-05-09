
using Havit.OpenAIChatPOC.Model.Common;

namespace Havit.OpenAIChatPOC.DataLayer.Repositories.Common;

public interface ICountryByIsoCodeLookupService
{
	Country GetCountryByIsoCode(string isoCode);
}
