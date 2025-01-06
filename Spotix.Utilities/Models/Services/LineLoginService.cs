using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Spotix.Utilities.Models.DTOs.LineLogin;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Web;

namespace Spotix.Utilities.Models.Services
{
	public class LineLoginService
	{
		private static HttpClient client = new HttpClient();
		private readonly JsonProvider _jsonProvider = new JsonProvider();

		public LineLoginService() {
		}

		private readonly string loginUrl = "https://access.line.me/oauth2/v2.1/authorize?response_type={0}&client_id={1}&redirect_uri={2}&state={3}&scope={4}";
		private readonly string clientId = "2006710278";
		private readonly string clientSecret = "5070df0d9e79711960d422773779d309";
		private readonly string tokenUrl = "https://api.line.me/oauth2/v2.1/token";
		//private readonly string profileUrl = "https://api.line.me/v2/profile";

		private readonly string idTokenProfileUrl = "https://api.line.me/oauth2/v2.1/verify/?id_token={0}&client_id={1}";

		// 回傳 line authorization url
		public string GetLoginUrl(string redirectUrl)
		{
			// 根據想要得到的資訊填寫 scope
			var scope = "profile%20openid%20email";
			// 每次登入 request 都帶入隨機的 state
			var state = Guid.NewGuid().ToString();
			var uri = string.Format(loginUrl, "code", clientId, HttpUtility.UrlEncode(redirectUrl), state, scope);
			return uri;
		}

		// 取得 access token 等資料
		public async Task<UserIdTokenProfileDto> GetTokensByAuthToken(string authToken, string callbackUri)
		{
			var formContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("grant_type", "authorization_code"),
				new KeyValuePair<string, string>("code", authToken),
				new KeyValuePair<string, string>("redirect_uri",callbackUri),
				new KeyValuePair<string, string>("client_id", clientId),
				new KeyValuePair<string, string>("client_secret", clientSecret),
			});

			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //添加 accept header
			var response = await client.PostAsync(tokenUrl, formContent); // 送出 post request
			var dto = _jsonProvider.Deserialize<TokensResponseDto>(await response.Content.ReadAsStringAsync()); //將 json response 轉成 dto

			// 使用 idToken 取得 user profile
			UserIdTokenProfileDto userProfileByIdToken = await GetUserProfileByIdToken(dto.Id_token);

			
			return userProfileByIdToken;
		}

		/// <summary>
		/// 使用 access token 取得 user profile
		/// </summary>
		/// <param name="accessToken"></param>
		/// <returns></returns>
		//public async Task<LineUserProfileDto> GetUserProfileByAccessToken(string accessToken)
		//{
		//	//取得 UserProfile
		//	var request = new HttpRequestMessage(HttpMethod.Get, profileUrl);
		//	client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		//	var response = await client.SendAsync(request);
		//	var profile = _jsonProvider.Deserialize<LineUserProfileDto>(await response.Content.ReadAsStringAsync());

		//	return profile;
		//}

		/// <summary>
		/// 使用 idToken 取得 user profile
		/// </summary>
		/// <param name="idToken"></param>
		/// <returns></returns>
		public async Task<UserIdTokenProfileDto> GetUserProfileByIdToken(string idToken)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, string.Format(idTokenProfileUrl, idToken, clientId));
			var response = await client.SendAsync(request);
			var dto = _jsonProvider.Deserialize<UserIdTokenProfileDto>(await response.Content.ReadAsStringAsync());

			return dto;
		}
	}
}
