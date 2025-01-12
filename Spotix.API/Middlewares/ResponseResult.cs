namespace Spotix.API.Middlewares
{
	public class ResponseResult
	{
		public int status { get; set; }
		public bool isSuccess { get; set; }
		public string message { get; set; }
		public List<object> data { get; set; }

		public ResponseResult(int status, bool isSuccess, string message, List<object> data)
		{
			this.status = status;
			this.isSuccess = isSuccess;
			this.message = message;
			this.data = data;
		}
	}
}
