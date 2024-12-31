using Spotix.Utilities.Models.EFModels;

namespace Spotix.Utilities.Interfaces
{
	public interface ITokenRepository
	{
		string CeateJwtToken(User user, List<string> roles);

	}
}
