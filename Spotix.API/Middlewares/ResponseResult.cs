namespace Spotix.API.Middlewares
{
	public class ResponseResult
	{
		public int Status { get; set; }
		public bool IsSuccess { get; set; }
		public string Message { get; set; }
		public List<object> Data { get; set; }

		public ResponseResult(int status, bool isSuccess, string message, List<object> data)
		{
			Status = status;
			IsSuccess = isSuccess;
			Message = message;
			Data = data;
		}
	}
}
