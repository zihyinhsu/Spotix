using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotix.Utilities.Models.ViewModels;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.EFModels;
using Spotix.API.Exceptions;
using Spotix.Utilities.Models.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using Spotix.Utilities.Models.Interfaces;
using Spotix.API.CustomActionFilter;
using Spotix.Utilities.Models;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<User> userManager;
		private readonly ITokenRepository tokenRepository;
		private readonly IMapper mapper;
		private readonly ImageService imageService;
		private readonly OrderService orderService;


		public AuthController(UserManager<User> userManager, ITokenRepository tokenRepository, IMapper mapper, ImageService imageService, OrderService orderService)
		{
			this.userManager = userManager;
			this.tokenRepository = tokenRepository;
			this.mapper = mapper;
			this.imageService = imageService;
			this.orderService = orderService;

		}
		// POST: api/Auth/Register
		[HttpPost]
		[Route("Register")]
		[ValidateModel]
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

						return CreatedAtAction(nameof(Register), null);
					}
				}
			}
			var errors = identityResult.Errors.Select(e => e.Description).ToArray()[1];
			throw new ArgumentException($"{errors}, 請聯繫系統管理員");
		}

		[HttpPost]
		[Route("Login")]
		[ValidateModel]
		public async Task<IActionResult> Login([FromBody] LoginVM model)
		{
			var user = await userManager.FindByEmailAsync(model.Email);
			if (user != null)
			{
				var checkPasswordResult = await userManager.CheckPasswordAsync(user, model.Password);
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
							JwtToken = jwtToken,
							AvatarUrl = user.AvatarUrl
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
		[ValidateModel]
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
		[HttpPatch]
		[Route("Profile")]
		[Authorize]
		[ValidateModel]
		public async Task<IActionResult> UpdateProfile([FromBody] ProfileVM model)
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			var user = await userManager.FindByEmailAsync(userEmail);

			// 使用 AutoMapper 將 model 映射到 user
			mapper.Map(model, user);

			var result = await userManager.UpdateAsync(user);
			if (result.Succeeded)
			{
				var profile = mapper.Map<ProfileVM>(user);
				HttpContext.Items["message"] = "更新成功";
				return Ok(profile);
			}

			return BadRequest(result.Errors);
		}

		// 取得使用者資料
		[HttpGet]
		[Route("Profile")]
		[Authorize]
		public async Task<IActionResult> GetProfile()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			var user = await userManager.FindByEmailAsync(userEmail);

			if (user == null) throw new ResourceNotFoundException("使用者不存在");

			var roles = await userManager.GetRolesAsync(user);

			user.Orders = await orderService.GetByUserId(user.Id);

			user.Roles = roles.ToList();

			var profile = mapper.Map<UserDto>(user);

			HttpContext.Items["message"] = "成功取得使用者資料";
			return Ok(profile);
		}

	}
}
