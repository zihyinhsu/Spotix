using Spotix.Utilities.Models.DTOs.Messages;
using Spotix.Utilities.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.Services
{
	public class LineBotService
	{

		public string channelAccessToken = "bS8WEAghMpJhkH88AfgGpx0pm+VD9E7gElr3TquYb12l3X4O5j0umX/eZEhXmOb/iJ1VQGs4rlDzg9WFkqJb9oiGiHVW5dBI8iJ2wAd+fxiDiTiWqACyz1m5i3oDuskvHMqpqrC9KyJALE21PVdphAdB04t89/1O/w1cDnyilFU=";
		private readonly string pushMessageUri = "https://api.line.me/v2/bot/message/push";

		private static HttpClient client = new HttpClient();
		private readonly JsonProvider _jsonProvider = new JsonProvider();

		public async Task PushMessageAsync(string userId, IEnumerable<TextMessageDto> messages)
		{
			var request = new
			{
				to = userId,
				messages = messages
			};

			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
			var json = _jsonProvider.Serialize(request);

			var requestMessage = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri(pushMessageUri),
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};

			var response = await client.SendAsync(requestMessage);

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Error sending message: {response.ReasonPhrase}");
			}
		}
	}
}
