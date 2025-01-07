using Spotix.Utilities.Models.EFModels;

namespace Spotix.Utilities.Models.Interfaces
{
	public interface ITokenRepository
	{
		string CeateJwtToken(User user, List<string> roles);

	}
}
