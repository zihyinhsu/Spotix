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
	public class AreasController : Controller
	{
		private readonly AppDbContext _context;
		private readonly AreaService _areaService;

		public AreasController(AppDbContext context, AreaService areaService)
		{
			_context = context;
			_areaService = areaService;
		}
		public async Task<IActionResult> Index(
			string sortOrder,
			string currentFilterSession,
			string searchStringSession,     //Aarea名稱沒有鑑別度，ex. 2樓c1區, 身心障礙者專區...
											//sessionName則有包含活動名稱，ex. 2025聖誕音樂會_上午場

			int? goToPageNumber,
			int pageSize,
			int? pageNumber)
		{
			var query = from a in _context.Areas
						join s in _context.Sessions on a.SessionId equals s.Id // 內連接
						select new AreaVM
					   {
						   Id = a.Id,
						   Name = a.Name,
						   Price = a.Price,
						   SessionName = s.Name,
							SessionId = a.SessionId,
							Qty = a.Qty,
						   DisplayOrder = a.DisplayOrder,
						};
			//條件過濾:
			if (searchStringSession != null)
			{
				pageNumber = 1;
			}
			else
			{
				searchStringSession = currentFilterSession;
			}
			ViewData["CurrentFilterSession"] = searchStringSession;

			if (!string.IsNullOrEmpty(searchStringSession))
			{
				query = query.Where(a => a.SessionName.Contains(searchStringSession));
			}

			//排序邏輼:
			ViewData["CurrentSort"] = sortOrder;

			switch (sortOrder)
			{
				case "1":
					query = query.OrderByDescending(a => a.DisplayOrder); break;
				case "2":
					query = query.OrderBy(a => a.DisplayOrder); break;
				case "3":
					query = query.OrderByDescending(a => a.Price); break;
				case "4":
					query = query.OrderBy(a => a.Price); break;
				default:
					query = query.OrderByDescending(a => a.DisplayOrder); break;

			}

			//分頁邏輯:
			if (!string.IsNullOrEmpty(searchStringSession) && goToPageNumber != null)
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
			return View(await PaginatedList<AreaVM>.CreateAsyncP(query.AsNoTracking(), pageNumber ?? 1, pageSize));

		}

		public IActionResult Create()
		{
			ViewData["SessionList"] = new SelectList(_context.Sessions, "Id", "Name");
			return View();
		}

		// POST: AreasController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Create([Bind(include: "Id,Name,Price,SessionId,Qty,DisplayOrder")] AreaVM model)
		{
			if (ModelState.IsValid)
			{
				var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == model.SessionId);
				if (session != null)
				{
					//model.SessionName = (string)session.Name; // 手動設置 SessionName
					var areaEntity = new Area
					{
						Name = model.Name,
						Price = model.Price,
						SessionId = (int)model.SessionId,
						Qty = model.Qty,
						DisplayOrder = model.DisplayOrder,
					};

					// 使用 AreaService 的 CreateAsync 方法
					var createdArea = await _areaService.CreateAsync(areaEntity);

					//_context.Add(areaEntity);
					//await _context.SaveChangesAsync();

					return RedirectToAction(nameof(Index));
				}
				ModelState.AddModelError("", "Invalid session.");
			}

			else
			{
				// 遍歷 ModelState 並顯示具體錯誤訊息
				foreach (var state in ModelState)
				{
					foreach (var error in state.Value.Errors)
					{
						// 這裡可以將錯誤訊息記錄到日誌或顯示在視圖中
						Console.WriteLine($"Property: {state.Key}, Error: {error.ErrorMessage}");
					}
				}
			}

			ViewData["SessionList"] = new SelectList(_context.Sessions, "Id", "Name");
			return View(model);
		}



		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var areaEntity = await _context.Areas.AsNoTracking().Include(a => a.Session).FirstOrDefaultAsync(a => a.Id == id);

			if (areaEntity == null)
			{
				return NotFound();
			}
			
			var areaVM = new AreaVM
			{
				Id = areaEntity.Id,
				Name = areaEntity.Name,
				Price = areaEntity.Price,
				Qty = areaEntity.Qty,
				SessionId = areaEntity.SessionId,
				DisplayOrder = areaEntity.DisplayOrder,

			};

			ViewData["SessionList"] = new SelectList(_context.Sessions, "Id", "Name");
			return View(areaVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task <IActionResult> Edit([Bind(include: "Id,Name,Price,SessionId,Qty,DisplayOrder")] AreaVM model, int? id)
		{
			//if (id != model.Id)
			//{
			//	return NotFound();
			//}
			if (!ModelState.IsValid)
			{
				return View(model);		
			}

			var areaEntity = await _context.Areas.AsNoTracking().Include(a => a.Session).FirstOrDefaultAsync(a => a.Id == id);

			var areaEdit = new Area
			{
				Id = model.Id,
				Name = model.Name,
				Price = model.Price,
				SessionId = (int)model.SessionId,
				Qty = model.Qty,
				DisplayOrder = model.DisplayOrder,
			};

			_context.Update(areaEdit);
			await _context.SaveChangesAsync();
			ViewData["SessionList"] = new SelectList(_context.Sessions, "Id", "Name");
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var areaEntity = await _context.Areas
				.Include(a => a.Session)
				.FirstOrDefaultAsync(a => a.Id == id);

			if (areaEntity == null)
			{
				return NotFound();
			}
			//string htmlContent = areaEntity.Info;
			////string plainText = ConvertHtmlToPlainText(htmlContent);
			////string plainText = htmlContent;
			var areaVM = new AreaVM
			{
				Id = areaEntity.Id,
				Name = areaEntity.Name,
				Price = areaEntity.Price,
				Qty = areaEntity.Qty,
				SessionId = areaEntity.SessionId,
				DisplayOrder = areaEntity.DisplayOrder,
				SessionName = areaEntity.Session.Name
			};
			return View(areaVM);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var areaEntity = await _context.Areas.Include(a => a.Tickets).FirstOrDefaultAsync(a => a.Id == id);

			if (areaEntity != null)
			{
				_context.Tickets.RemoveRange(areaEntity.Tickets);
				_context.Areas.Remove(areaEntity);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(Index));
		}

	}
}
