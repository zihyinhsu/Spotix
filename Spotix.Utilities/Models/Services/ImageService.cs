using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spotix.Utilities.Interfaces;
using Spotix.Utilities.Models.EFModels;

namespace Spotix.Utilities.Models.Services
{
	public class ImageService
	{
		private readonly IImageRepository _imageRepository;
		private Cloudinary _cloudinary { get; set; }
		private CloudinarySettings _cloudinarySettings { get; set; }

		public ImageService(IConfiguration Configuration, IImageRepository imageRepository)
		{
			_cloudinarySettings = Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
			var acc = new Account(
				_cloudinarySettings.CloudName,
				_cloudinarySettings.ApiKey,
				_cloudinarySettings.ApiSecret);
			_cloudinary = new Cloudinary(acc);
			_imageRepository = imageRepository;
		}
		public async Task<Image> AddImageAsync(IFormFile file)
		{

			var uploadResult = new ImageUploadResult();
			if (file.Length > 0)
			{
				using var stream = file.OpenReadStream();
				var uploadParams = new ImageUploadParams
				{
					File = new FileDescription(file.FileName, stream),
					Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
				};
				uploadResult = await _cloudinary.UploadAsync(uploadParams);
			}
			var image = new Image
			{
				Id = uploadResult.PublicId,
				ImageUrl = uploadResult.SecureUrl.AbsoluteUri
			};

			// 存進資料庫
			await _imageRepository.CreateAsync(image);

			return image;
		}

		public async Task<DeletionResult> DeleteImageAsync(string id)
		{
			var existingImage = await _imageRepository.GetByIdAsync(id);

			var deleteParams = new DeletionParams(existingImage.Id);

			var result = await _cloudinary.DestroyAsync(deleteParams);

			// 從資料庫刪除
			await _imageRepository.DeleteAsync(id);

			return result;
		}

		public async Task<Image?> GetImageByUrlAsync(string url)
		{
			return await _imageRepository.GetByUrlAsync(url);
		}
	}
}
