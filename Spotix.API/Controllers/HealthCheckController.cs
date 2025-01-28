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
		private readonly AppDbContext _dbContext; // EF Core 的資料庫上下文

		public HealthCheckController(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpHead]
		public async Task<IActionResult> CheckHealth()
		{
			try
			{
				// 執行簡單的 SQL 查詢以測試資料庫連線
				await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
				HttpContext.Items["message"] = "資料庫連線成功";

				return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					Status = "Unhealthy",
					Message = $"資料庫連線失敗: {ex.Message}"
				});
			}
		}
	}
}
