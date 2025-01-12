using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Spotix.Utilities.Models;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.Interfaces;
using Spotix.Utilities.Models.ViewModels;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Spotix.MVC.Controllers
{

	public class EventsController : Controller
	{
		private readonly AppDbContext _context;



		public EventsController(AppDbContext context)
		{
			_context = context;

		}
		public async Task<IActionResult> Index(
			string sortOrder,
			string currentFilterEvent,
			DateTime currentFilterBefore,
			DateTime currentFilterAfter,
			//改為Date >>>
			string searchStringEvent,
			DateTime? searchTimeBefore,
			DateTime? searchTimeAfter,
			int? goToPageNumber,
			int pageSize,
			int? pageNumber)
		{

			//if (searchTimeBefore == null || searchTimeBefore == DateTime.MinValue)
			//{
			//	searchTimeBefore = DateTime.Now; // 或者設置為其他合理的預設值
			//}
			//if (searchTimeAfter == null || searchTimeAfter == DateTime.MinValue)
			//{
			//	searchTimeAfter = DateTime.Now; // 或者設置為其他合理的預設值
			//}


			//搜尋邏輯:
			var query = from e in _context.Events
						join p in _context.Places on e.PlaceId equals p.Id  //inner join
						join s in _context.Sessions on e.Id equals s.EventId into result1
						from es in result1.DefaultIfEmpty()     //left join
						select new EventVM
						{
							Id = e.Id,
							Name = e.Name,
							Info = e.Info,
							CoverUrl = e.CoverUrl,
							ImgUrl = e.ImgUrl,
							PlaceName = p.Name, //這裡將Place的Name取出來
												//FirstSessionTime = es != null ? ((IEnumerable<Session>)result1).DefaultIfEmpty().OrderBy(st => st.SessionTime).Select(st => st.SessionTime).FirstOrDefault() : (DateTime?)null, //這裡將Session的最小sessionTime取出來	//沒有sessionTime的event也會找出來
												//FirstSessionTime= result1.OrderBy(st => st.SessionTime).Select(st => (DateTime?)st.SessionTime).FirstOrDefault(),
							FirstSessionTime = es.SessionTime, //這裡將Session的sessionTime取出來	//沒有ssessionTime的event也會找出來 //改為只找出第一筆 >>>
							PlaceId = p.Id,
							Host = e.Host,
							Published = e.Published,
						};

			//條件過濾:
			if (searchStringEvent != null || searchTimeBefore.HasValue || searchTimeAfter.HasValue)
			{
				pageNumber = 1;
			}
			else
			{
				searchStringEvent = currentFilterEvent;
				searchTimeBefore = currentFilterBefore != DateTime.MinValue ? currentFilterBefore : (DateTime?)null;
				searchTimeAfter = currentFilterAfter != DateTime.MinValue ? currentFilterAfter : (DateTime?)null;
			}
			ViewData["CurrentFilterEvent"] = searchStringEvent;
			ViewData["CurrentFilterBefore"] = searchTimeBefore;
			ViewData["CurrentFilterAfter"] = searchTimeAfter;

			if (!string.IsNullOrEmpty(searchStringEvent))
			{
				query = query.Where(e => e.Name.Contains(searchStringEvent));
			}
			if (searchTimeBefore.HasValue || searchTimeAfter.HasValue)
			{
				query = query.Where(e => e.FirstSessionTime <= searchTimeAfter && e.FirstSessionTime >= searchTimeBefore);   //
			}


			//排序邏輼:
			ViewData["CurrentSort"] = sortOrder;

			switch (sortOrder)
			{
				case "1":
					query = query.OrderBy(e => e.FirstSessionTime); break;
				case "2":
					query = query.OrderByDescending(e => e.FirstSessionTime); break;
				case "3":
					query = query.OrderByDescending(e => e.Published); break;
				case "4":
					query = query.OrderBy(e => e.Published); break;
				default:
					query = query.OrderByDescending(e => e.FirstSessionTime); break;
			}

			//分頁邏輯:
			if (!string.IsNullOrEmpty(searchStringEvent) && (searchTimeBefore.HasValue || searchTimeAfter.HasValue) && goToPageNumber != null)
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
			return View(await PaginatedList<EventVM>.CreateAsyncP(query.AsNoTracking(), pageNumber ?? 1, pageSize));


		}

		public IActionResult Create()
		{
			ViewData["PlaceList"] = new SelectList(_context.Places, "Id", "Name");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Create([Bind(include: "Id,Name,Info,CoverUrl,ImgUrl,PlaceName,Host,Published,FirstSessionTime")] EventVM model)
		{
			if (ModelState.IsValid)
			{
				int placeId;
				Place place = null;

				// 嘗試將 PlaceName 轉換為 int
				if (int.TryParse(model.PlaceName, out placeId))
				{
					// 如果轉換成功，根據 placeId 查找對應的 Place
					place = await _context.Places.FirstOrDefaultAsync(p => p.Id == placeId);
				}
				//else
				//{
				//	// 如果轉換失敗，根據 PlaceName 查找對應的 Place
				//	place = await _context.Places.FirstOrDefaultAsync(p => p.Name == model.PlaceName);
				//}

				if (place == null)
				{
					ModelState.AddModelError("PlaceName", "The specified place does not exist.");
					ViewData["PlaceList"] = new SelectList(_context.Places, "Id", "Name");
					return View(model);
				}

				// 創建 Event 實體
				var eventEntity = new Event
				{
					Id = model.Id,
					Name = model.Name,
					Info = model.Info,
					CoverUrl = model.CoverUrl,
					ImgUrl = model.ImgUrl,
					PlaceId = place.Id,
					Host = model.Host,
					Published = model.Published
				};

				_context.Add(eventEntity);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			ViewData["PlaceList"] = new SelectList(_context.Places, "Id", "Name");
			return View(model);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			//var eventEntity = await _context.Events.FindAsync(id);
			var eventEntity = await _context.Events.AsNoTracking().Include(e => e.Place).FirstOrDefaultAsync(e => e.Id == id);

			if (eventEntity == null)
			{
				return NotFound();
			}
			// 將 Event 實體轉換為 EventVM
			var eventVM = new EventVM
			{
				Id = eventEntity.Id,
				Name = eventEntity.Name,
				Info = eventEntity.Info,
				CoverUrl = eventEntity.CoverUrl,
				ImgUrl = eventEntity.ImgUrl,
				PlaceId = eventEntity.Place.Id,
				Host = eventEntity.Host,
				Published = eventEntity.Published,
			};

			ViewData["PlaceList"] = new SelectList(_context.Places, "Id", "Name", eventVM.PlaceId); //, eventEntity.PlaceId
			return View(eventVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Edit([Bind(include: "Id,Name,Info,CoverUrl,ImgUrl, PlaceId,Host,Published,FirstSessionTime")] EventVM model, int? id)
		{


			//if (id == null)
			//{
			//	return NotFound();
			//}

			//if (ModelState.IsValid)
			//{
			//	int placeId;
			//	Place place = null;
			//	// 嘗試將 PlaceName 轉換為 int
			//	if (int.TryParse(model.PlaceName, out placeId))
			//	{
			//		// 如果轉換成功，根據 placeId 查找對應的 Place
			//		place = await _context.Places.FirstOrDefaultAsync(p => p.Id == placeId);
			//	}
			//else
			//{
			//	// 如果轉換失敗，根據 PlaceName 查找對應的 Place
			//	place = await _context.Places.FirstOrDefaultAsync(p => p.Name == model.PlaceName);
			//}
			//	if (place == null)
			//	{
			//		ModelState.AddModelError("PlaceName", "The specified place does not exist.");
			//		ViewData["PlaceList"] = new SelectList(_context.Places, "Id", "Name");
			//		return View(model);
			//	}
			//	// 創建 Event 實體
			//	var eventEntity = new Event
			//	{
			//		Id = model.Id,
			//		Name = model.Name,
			//		Info = model.Info,
			//		CoverUrl = model.CoverUrl,
			//		ImgUrl = model.ImgUrl,
			//		PlaceId = place.Id,
			//		Host = model.Host,
			//		Published = model.Published
			//	};
			//	_context.Update(eventEntity);
			//	await _context.SaveChangesAsync();
			//	return RedirectToAction(nameof(Index));
			//}
			//ViewData["PlaceList"] = new SelectList(_context.Places, "Id", "Name");
			//return View(model);
			//

			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var eventEntity = await _context.Events.AsNoTracking().Include(e => e.Place).FirstOrDefaultAsync(e => e.Id == id);
			//eventEntity.Name = model.Name;
			//eventEntity.Info = model.Info;
			//eventEntity.CoverUrl = model.CoverUrl;
			//eventEntity.ImgUrl = model.ImgUrl;
			//eventEntity.PlaceId = model.PlaceName;
			//eventEntity.Host = model.Host;
			//eventEntity.Published = model.Published;


			// 創建 Event 實體
			var eventEdit = new Event
			{
				Id = model.Id,
				Name = model.Name,
				Info = model.Info,
				CoverUrl = model.CoverUrl,
				ImgUrl = model.ImgUrl,
				PlaceId = model.PlaceId,
				Host = model.Host,
				Published = model.Published
			};
			_context.Update(eventEdit);
			await _context.SaveChangesAsync();
			ViewData["PlaceList"] = new SelectList(_context.Places, "Id", "Name", eventEdit.PlaceId);
			return RedirectToAction(nameof(Index));

		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var eventEntity = await _context.Events
				.Include(e => e.Place)
				.FirstOrDefaultAsync(e => e.Id == id);
			if (eventEntity == null)
			{
				return NotFound();
			}
			string htmlContent = eventEntity.Info;
			//string plainText = ConvertHtmlToPlainText(htmlContent);
			//string plainText = htmlContent;
			var eventVM = new EventVM
			{
				Id = eventEntity.Id,
				Name = eventEntity.Name,
				Info = htmlContent,
				CoverUrl = eventEntity.CoverUrl,
				ImgUrl = eventEntity.ImgUrl,
				PlaceName = eventEntity.Place.Name,
				PlaceId = eventEntity.PlaceId,
				Host = eventEntity.Host,
				Published = eventEntity.Published,
			};
			ViewBag.Content = htmlContent;
			return View(eventVM);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var eventEntity = await _context.Events.FindAsync(id);
			if (eventEntity != null)
			{
				_context.Events.Remove(eventEntity);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(Index));
		}






		//public static DateTime TruncateToHours(DateTime dateTime)
		//{
		//	return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
		//}
		//public static string ConvertHtmlToPlainText(string html)
		//{
		//	if (string.IsNullOrEmpty(html))
		//		return string.Empty;

		//	// 移除 HTML 標籤
		//	string plainText = Regex.Replace(html, "<.*?>", string.Empty);

		//	// 解碼 HTML 實體（例如 &nbsp;, &lt; 等）
		//	return System.Net.WebUtility.HtmlDecode(plainText);
		//}


	}
}




