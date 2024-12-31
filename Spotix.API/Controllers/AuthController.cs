using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotix.Utilities.Models.ViewModels;
using Spotix.Utilities.Models.Dtos;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Interfaces;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<User> userManager;
		private readonly ITokenRepository tokenRepository;

		public AuthController(UserManager<User> userManager, ITokenRepository tokenRepository)
		{
			this.userManager = userManager;
			this.tokenRepository = tokenRepository;
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
						return Ok("註冊成功! 請登入帳號");
					}
				}
			}

			return BadRequest($"{identityResult.Errors}, 請聯繫系統管理員");
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
						return Ok(response);
					}
				}
			}

			return BadRequest("登入失敗");
		}
	}
}
