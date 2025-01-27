using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HealthCheckController : ControllerBase
	{
		[HttpHead]
		public IActionResult Get()
		{
			return Ok();
		}
	}
}
