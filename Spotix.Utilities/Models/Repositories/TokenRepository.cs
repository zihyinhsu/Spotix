using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Spotix.Utilities.Models.Repositories
{
	public class TokenRepository : ITokenRepository
	{
		private readonly IConfiguration configuration;

		public TokenRepository(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		public string CeateJwtToken(User user, List<string> roles)
		{
			// 創建一個包含用戶聲明（Claims）的列表，這些聲明將用於生成 JWT
			var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.Email, user.Email));

			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			// 創建一個對稱式加密金鑰
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

			// 創建一個簽名憑證
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				issuer: configuration["Jwt:Issuer"],
				audience: configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
