using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotix.Utilities.Models.ViewModels;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Interfaces;
using Spotix.API.Exceptions;
using Spotix.Utilities.Models.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<User> userManager;
		private readonly ITokenRepository tokenRepository;
		private readonly ImageService imageService;

		public AuthController(UserManager<User> userManager, ITokenRepository tokenRepository, ImageService imageService)
		{
			this.userManager = userManager;
			this.tokenRepository = tokenRepository;
			this.imageService = imageService;
		}
		// POST: api/Auth/Register
		[HttpPost]
		[Route("Register")]
		public async Task<IActionResult> Register([FromBody] RegisterVM registerRequestDto)
		{

			var user = new User
			{
				UserName = registerRequestDto.UserName,
				Email = registerRequestDto.Email
			};

			var identityResult = await userManager.CreateAsync(user, registerRequestDto.Password);

			if (identityResult.Succeeded)
			{
				// Add roles to user
				if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
				{
					identityResult = await userManager.AddToRolesAsync(user, registerRequestDto.Roles);
					if (identityResult.Succeeded)
					{
						HttpContext.Items["message"] = "註冊成功! 請登入帳號";

						return CreatedAtAction(nameof(Register), new List<object>());
					}
				}
			}
			var errors = identityResult.Errors.Select(e => e.Description).ToArray()[1];
			throw new ArgumentException($"{errors}, 請聯繫系統管理員");
		}

		[HttpPost]
		[Route("Login")]
		public async Task<IActionResult> Login([FromBody] LoginVM loginRequestDto)
		{
			var user = await userManager.FindByEmailAsync(loginRequestDto.Email);
			if (user != null)
			{
				var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
				if (checkPasswordResult)
				{
					var roles = await userManager.GetRolesAsync(user);
					if (roles != null)
					{
						// Create Token
						var jwtToken = tokenRepository.CeateJwtToken(user, roles.ToList());

						var response = new LoginResponseDto
						{
							UserName = user.UserName,
							Email = user.Email,
							JwtToken = jwtToken
						};

						HttpContext.Items["message"] = "登入成功";

						return Ok(response);
					}
				}
			}
			throw new ResourceNotFoundException("登入失敗");
		}


		[HttpPost]
		[Route("UploadAvatar")]
		[Authorize]
		public async Task<ActionResult<Image>> UploadAvatar([FromForm] AddImageVM request)
		{
			// 取得使用者資料
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			var user = await userManager.FindByEmailAsync(userEmail);
			
			// 從Images資料庫找 user.avatarUrl 是否存在
			var ExistingImage = await imageService.GetImageByUrlAsync(user.AvatarUrl);
			if (ExistingImage != null)
			{
				// 若存在就取其id並刪掉
				var deleteResult = await imageService.DeleteImageAsync(ExistingImage.Id);
			}
			var image = await imageService.AddImageAsync(request.File);

			// 更改使用者的大頭貼 Url
			user.AvatarUrl = image.ImageUrl;

			// 更新使用者資料
			var result = await userManager.UpdateAsync(user);
			if (result.Succeeded)
			{
				HttpContext.Items["message"] = "上傳大頭貼成功";

				return Ok(new { ImageUrl = image.ImageUrl });

			}
			return BadRequest("上傳失敗");
		}

		// 修改使用者資料
		//[HttpPut]
		//[Route("UpdateProfile")]
		//public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
		//{
		//	var user = await userManager.FindByIdAsync(updateProfileDto.UserId.ToString());
		//	if (user == null)
		//	{
		//		return NotFound("User not found");
		//	}

		//	user.Email = updateProfileDto.Email;
		//	user.UserName = updateProfileDto.UserName;

		//	var result = await userManager.UpdateAsync(user);
		//	if (result.Succeeded)
		//	{
		//		return Ok("Profile updated successfully");
		//	}

		//	return BadRequest(result.Errors);
		//}

	}
}
