using DotNetLineBotSdk.Helpers;
using DotNetLineBotSdk.Message;
using DotNetLineBotSdk.MessageEvent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spotix.Utilities.Models.DTOs.Messages;
using Spotix.Utilities.Models.Services;
using Spotix.Utilities.Providers;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LineBotController : ControllerBase
	{
		private readonly LineBotService _lineBotService;
		private readonly JsonProvider _jsonProvider;
		// constructor
		public LineBotController()
		{
			_lineBotService = new LineBotService();
			_jsonProvider = new JsonProvider();
		}

		/// <summary>
		/// 取得 userId 並回傳
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> Post()
		{
			var replyEvent = new ReplyEvent(_lineBotService.channelAccessToken);

			try
			{
				// Get Post Raw Data(json format)
				var req = this.HttpContext.Request;
				using (var bodyReader = new StreamReader(
					stream: req.Body,
					encoding: Encoding.UTF8,
					detectEncodingFromByteOrderMarks: false,
					bufferSize: 1024,
					leaveOpen: true))
				{
					var body = await bodyReader.ReadToEndAsync();
					var lineReceMsg = ReceivedMessageConvert.ReceivedMessage(body);

					if (lineReceMsg != null && lineReceMsg.Events[0].Type == WebhookEventType.message.ToString())
					{

						if (lineReceMsg.Events[0].Message.Type == MessageType.text.ToString())
						{
							var userId = lineReceMsg.Events[0].Source.UserId;
							var txtMessage = new TextMessage($"您的Id為 : {userId} ");
							await replyEvent.ReplyAsync(lineReceMsg.Events[0].ReplyToken, new List<IMessage>() {
						txtMessage
					});
						}
					}
				}
			}
			catch (Exception ex)
			{
				return Ok();
			}
			return Ok();
		}

		[HttpPost("SendMessage/User")]
		public async Task<IActionResult> SendMessageToUser([Required] string userId, IEnumerable<TextMessageDto> message)
		{
			try
			{				
				await _lineBotService.PushMessageAsync(userId, message);
				HttpContext.Items["message"] = "傳送成功";
				return Ok(nameof(SendMessageToUser));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
