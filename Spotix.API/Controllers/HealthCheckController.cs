using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spotix.Utilities.Models.EFModels;

namespace Spotix.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HealthCheckController : ControllerBase
	{
		private readonly AppDbContext _dbContext;

		public HealthCheckController(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpHead]
		public async Task<IActionResult> CheckHealth()
		{
			try
			{
				var currentTime = DateTime.Now.TimeOfDay;

				// 定義允許執行的時間範圍
				var startTime = new TimeSpan(8, 0, 0); 
				var endTime = new TimeSpan(19, 0, 0);

				// 檢查當前時間是否在允許的時間範圍內
				if (currentTime >= startTime && currentTime <= endTime)
				{
					// 執行簡單的 SQL 查詢以測試資料庫連線
					await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
					return Ok();
				}
				else
				{
					return StatusCode(403, "SQL 查詢僅允許在每天的 8:00 到 19:00 之間執行。");
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500);
			}
		}
	}
}
