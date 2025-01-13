using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotix.API.Exceptions;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.DTOs.LineLogin;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using Spotix.Utilities.Models.Repositories;
using Spotix.Utilities.Models.Services;
using Spotix.Utilities.Models.ViewModels;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LineLoginController : ControllerBase
	{
		private readonly LineLoginService lineLoginService;
		private readonly UserManager<User> userManager;
		private readonly ITokenRepository tokenRepository;

		public LineLoginController(UserManager<User> userManager, ITokenRepository tokenRepository)
		{
			this.lineLoginService = new LineLoginService();
			this.userManager = userManager;
			this.tokenRepository = tokenRepository;

		}

		[HttpGet("Url")]
		public IActionResult GetLoginUrl([FromQuery] string redirectUrl)
		{
			var uri = lineLoginService.GetLoginUrl(redirectUrl);

			HttpContext.Items["message"] = "取得回傳 uri";

			var response = new
			{
				returnUri = uri
			};

			return Ok(response);
		}

		// 使用 authToken 取回登入資訊
		[HttpGet("Tokens")]
		public async Task<IActionResult> GetTokensByAuthToken([FromQuery] string authToken, [FromQuery] string callbackUrl)
		{
			var userProfileByIdToken = await lineLoginService.GetTokensByAuthToken(authToken, callbackUrl);
			// 查找或創建用戶
			var user = await userManager.FindByEmailAsync(userProfileByIdToken.Email);
			// 如果用戶不存在，創建新用戶
			if (user == null)
			{
				user = new User
				{
					UserName = userProfileByIdToken.Name,
					Email = userProfileByIdToken.Email,
					AvatarUrl = userProfileByIdToken.Picture,
					Roles = new List<string> { "User" }, // 預設角色為 User
					LineId = userProfileByIdToken.Sub,
				};
				// 生成隨機密碼
				var randomPassword = Guid.NewGuid().ToString();
				var identityResult = await userManager.CreateAsync(user, randomPassword);
				if (identityResult.Succeeded)
				{
					// Add roles to user
					if (user.Roles != null && user.Roles.Any())
					{
						identityResult = await userManager.AddToRolesAsync(user, user.Roles);
						if (identityResult.Succeeded)
						{
							HttpContext.Items["message"] = "註冊成功! 請登入帳號";

							return CreatedAtAction(nameof(GetTokensByAuthToken), null);
						}
					}
				}
				var errors = identityResult.Errors.Select(e => e.Description).ToArray()[1];
				throw new ArgumentException($"{errors}, 請聯繫系統管理員");
			}

			if (user != null)
			{
				// 登入
				var roles = await userManager.GetRolesAsync(user);

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

			throw new ResourceNotFoundException("登入失敗");
		}


		// 使用 access token 取得 user profile
		//[HttpGet("Profile/{accessToken}")]
		//public async Task<LineUserProfileDto> GetUserProfileByAccessToken(string accessToken)
		//{
		//	return await _lineLoginService.GetUserProfileByAccessToken(accessToken);
		//}

		// 使用 id token 取得 user profile
		//[HttpGet("Profile/IdToken")]
		//public async Task<UserIdTokenProfileDto> GetUserProfileByIdToken(string idToken)
		//{
		//	return await lineLoginService.GetUserProfileByIdToken(idToken);
		//}
	}
}
