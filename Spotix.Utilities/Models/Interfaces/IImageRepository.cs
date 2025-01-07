using Spotix.Utilities.Models.EFModels;

namespace Spotix.Utilities.Models.Interfaces
{
	public interface IImageRepository
	{
		Task<Image> CreateAsync(Image image);
		Task<Image?> GetByIdAsync(string id);
		Task<Image?> GetByUrlAsync(string url);
		Task<Image?> DeleteAsync(string id);

	}
}
