using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Spotix.API.Exceptions;
using System.Net.Mime;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json.Linq;

namespace Spotix.API.Middlewares
{
	public class ResponseHandleMiddleware
	{
		//private readonly ILogger<ExceptionHandlerMiddleware> _logger;
		private readonly RequestDelegate _next;

		public ResponseHandleMiddleware(
			//ILogger<ExceptionHandlerMiddleware> logger,
			RequestDelegate next)
		{
			//_logger = logger;
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var originalBodyStream = context.Response.Body;

			using (var responseBody = new MemoryStream())
			{
				context.Response.Body = responseBody;
				try
				{
					await _next(context);

					context.Response.Body = originalBodyStream; // 將 HttpContext.Response.Body 恢復為原始的 Stream。這樣做的目的是確保在中間件處理完請求後，將回應流重置為原始的流，以便後續的中間件或框架能夠正常處理回應。
					responseBody.Seek(0, SeekOrigin.Begin); // 將 responseBody 流的位置設置為流的開始位置。這樣做的目的是在讀取或寫入流之前，確保從流的開始位置進行操作

					var responseText = await new StreamReader(responseBody).ReadToEndAsync();

					var data = new List<object>();

					if (!string.IsNullOrEmpty(responseText))
					{
						try
						{
							// 嘗試反序列化為 object
							var deserializedObject = JsonConvert.DeserializeObject(responseText);

							if (deserializedObject is JArray jArray)
							{
								data = jArray.ToObject<List<object>>();
							}
							else if (deserializedObject != null)
							{

								data = new List<object> { deserializedObject };
							}
						}
						catch (JsonException)
						{
							// 如果反序列化失敗，將資料設置為原始值
							data = new List<object>();
						}
					}
					var message = context.Items["message"] as string ?? "";

					var response = new ResponseResult(context.Response.StatusCode, true, message, data);

					var jsonResponse = JsonConvert.SerializeObject(response);
					await context.Response.WriteAsync(jsonResponse);
				}
				catch (Exception ex)
				{
					//logger.LogError(ex, ex.Message);
					await HandleCustomExceptionResponseAsync(context, ex);

					context.Response.Body = originalBodyStream;
					responseBody.Seek(0, SeekOrigin.Begin);

					var response = new ResponseResult(context.Response.StatusCode, false, ex.Message, new List<object>());
					var jsonResponse = JsonConvert.SerializeObject(response);
					await context.Response.WriteAsync(jsonResponse);
				}
			}
		}



		private async Task HandleCustomExceptionResponseAsync(HttpContext context, Exception ex)
		{
			context.Response.ContentType = MediaTypeNames.Application.Json;

			// 根據例外類型設置狀態碼
			if (ex is ResourceNotFoundException)
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			}
			else if (ex is KeyNotFoundException)
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			}
			else if (ex is ArgumentException)
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			}
			else if (ex is UnauthorizedAccessException)
			{
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
			}
			else
			{
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			}
		}
	}
}
