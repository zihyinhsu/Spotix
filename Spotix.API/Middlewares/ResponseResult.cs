namespace Spotix.API.Middlewares
{
	public class ResponseResult
	{
		public int Status { get; set; }
		public bool IsSuccess { get; set; }
		public string Message { get; set; }
		public object? Data { get; set; }

		public ResponseResult(int status, bool isSuccess, string message, object? data)
		{
			Status = status;
			IsSuccess = isSuccess;
			Message = message;
			Data = data;
		}
	}
}
