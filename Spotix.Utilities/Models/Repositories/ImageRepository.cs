using Microsoft.EntityFrameworkCore;
using Spotix.Utilities.Interfaces;
using Spotix.Utilities.Models.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.Repositories
{
	public class ImageRepository : IImageRepository
	{
		private readonly AppDbContext _dbContext;

		public ImageRepository(AppDbContext dBContext)
		{
			_dbContext = dBContext;
		}

		public async Task<Image> CreateAsync(Image image)
		{
			await _dbContext.Images.AddAsync(image);
			await _dbContext.SaveChangesAsync();
			return image;
		}
		public async Task<Image?> GetByIdAsync(string id)
		{
			return await _dbContext.Images.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<Image?> GetByUrlAsync(string url)
		{
			return await _dbContext.Images.FirstOrDefaultAsync(x => x.ImageUrl == url);
		}

		public async Task<Image?> DeleteAsync(string id)
		{
			var existingImage = await GetByIdAsync(id);

			if (existingImage == null)
			{
				return null;
			}

			_dbContext.Remove(existingImage);
			await _dbContext.SaveChangesAsync();

			return existingImage;
		}
	}
}
