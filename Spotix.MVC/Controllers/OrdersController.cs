using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spotix.Utilities.Models;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Services;
using Spotix.Utilities.Models.ViewModels;

namespace Spotix.MVC.Controllers
{
	public class OrdersController : Controller
	{
		private readonly AppDbContext _context;

		public OrdersController(AppDbContext context)
		{
			_context = context;

		}

		public async Task<IActionResult> Index(
			string sortOrder,
			string currentFilterUserName,
			string searchStringUserName,     

			int? goToPageNumber,
			int pageSize,
			int? pageNumber)
		{
			var query = from o in _context.Orders
						join u in _context.Users on o.UserId equals u.Id // 內連接
						join t in _context.Tickets on o.Id equals t.OrderId into result1// 內連接
						from ot in result1.OrderBy(t => t.SessionName).Take(1).DefaultIfEmpty()

						select new OrderVM
						{
							CreatedTime = o.CreatedTime,
							Total = o.Total,
							OrderSession = ot.SessionName,
							UserName = u.UserName,
							OrderNumber = o.OrderNumber,
							Id = o.Id
						};
			//var query = from o in _context.Orders
			//			join u in _context.Users on o.UserId equals u.Id // 內連接
			//			join t in _context.Tickets on o.Id equals t.OrderId // 內連接
			//			join s in _context.Sessions on t.SessionName equals s.Name
			//			group new { o, u, s } by new { o.Id, o.CreatedTime, o.Total, u.UserName } into g
			//			select new OrderVM
			//			{
			//				Id = g.Key.Id,
			//				CreatedTime = g.Key.CreatedTime,
			//				Total = g.Key.Total,
			//				UserName = g.Key.UserName,
			//				OrderSession = g.Select(x => x.s.Name).FirstOrDefault()
			//			};


			//條件過濾:
			if (searchStringUserName != null)
			{
				pageNumber = 1;
			}
			else
			{
				searchStringUserName = currentFilterUserName;
			}
			ViewData["CurrentFilterUserName"] = searchStringUserName;

			if (!string.IsNullOrEmpty(searchStringUserName))
			{
				query = query.Where(o => o.UserName.Contains(searchStringUserName));
			}

			//排序邏輼:
			ViewData["CurrentSort"] = sortOrder;

			switch (sortOrder)
			{
				case "1":
					query = query.OrderByDescending(o => o.CreatedTime); break;
				case "2":
					query = query.OrderBy(o => o.CreatedTime); break;
				case "3":
					query = query.OrderByDescending(o => o.Total); break;
				case "4":
					query = query.OrderBy(o => o.Total); break;
				default:
					query = query.OrderByDescending(o => o.CreatedTime); break;

			}

			//分頁邏輯:
			if (!string.IsNullOrEmpty(searchStringUserName) && goToPageNumber != null)
			{
				pageNumber = 1;
			}
			else if (goToPageNumber != null)
			{
				pageNumber = goToPageNumber;
			}

			//每頁顯示幾筆資料
			if (pageSize == 0)
			{
				pageSize = 10;
			}
			ViewData["pageSize"] = pageSize;
			//返回結果
			return View(await PaginatedList<OrderVM>.CreateAsyncP(query.AsNoTracking(), pageNumber ?? 1, pageSize));

		}
	}
}
