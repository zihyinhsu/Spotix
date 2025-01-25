using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spotix.Utilities.Models;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Spotix.MVC.Controllers
{
	public class SessionsController : Controller
	{

		private readonly AppDbContext _context;

		public SessionsController(AppDbContext context)
		{
			_context = context;

		}
		public async Task<IActionResult> Index(
			string sortOrder,
			string currentFilterEvent,
			string searchStringEvent,
			string currentFilterSession,
			string searchStringSession,
			int? goToPageNumber,
			int pageSize,
			int? pageNumber)
		{
			//搜尋邏輯:
			var query = from s in _context.Sessions
						join e in _context.Events on s.EventId equals e.Id
						into result1
						from se in result1.DefaultIfEmpty()
						select new SessionVM
						{
							Id = s.Id,
							Name = s.Name,
							SessionTime = s.SessionTime,
							AvailableTime = s.AvailableTime,
							PublishTime = s.PublishTime,
							Published = s.Published,
							EventName = se.Name //這裡將Event的Name取出來
						};

			//條件過濾:
			if (searchStringEvent != null || searchStringSession != null)
			{
				pageNumber = 1;
			}
			else
			{
				searchStringEvent = currentFilterEvent;
				searchStringSession = currentFilterSession;
			}

			ViewData["CurrentFilterEvent"] = searchStringEvent;
			ViewData["CurrentFilterSession"] = searchStringSession;

			if (!string.IsNullOrEmpty(searchStringEvent))
			{
				query = query.Where(e => e.EventName.Contains(searchStringEvent));
			}
			if (!string.IsNullOrEmpty(searchStringSession))
			{
				query = query.Where(s => s.Name.Contains(searchStringSession));
			}

			//排序邏輯:
			ViewData["CurrentSort"] = sortOrder;

			switch (sortOrder)
			{
				case "1":
					query = query.OrderByDescending(s => s.SessionTime); break;
				case "2":
					query = query.OrderByDescending(s => s.AvailableTime); break;
				case "3":
					query = query.OrderByDescending(s => s.PublishTime); break;
				default:
					query = query.OrderByDescending(s => s.SessionTime); break;

			}

			//分頁邏輯:
			if (goToPageNumber != null)
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
			return View(await PaginatedList<SessionVM>.CreateAsyncP(query.AsNoTracking(), pageNumber ?? 1, pageSize));


		}

		public IActionResult Create()
		{
			ViewData["EventList"] = new SelectList(_context.Events, "Id", "Name");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Create([Bind(include: "Id,Name,SessionTime,AvailableTime,PublishTime,Published,EventName, EventId")] SessionVM model)
		{
			if (ModelState.IsValid)
			{
				var eventSpotix = await _context.Events.FirstOrDefaultAsync(e => e.Id == model.EventId);
				if (eventSpotix != null)
				{
					var session = new Session
					{
						Name = model.Name,
						SessionTime = model.SessionTime,
						AvailableTime = model.AvailableTime,
						PublishTime = model.PublishTime,
						Published = model.Published,
						EventId = (int)model.EventId
					};


					_context.Sessions.Add(session);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}

				ModelState.AddModelError("", "Invalid event.");

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

			ViewData["EventList"] = new SelectList(_context.Events, "Id", "Name");
			return View(model);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var session = await _context.Sessions.AsNoTracking().Include(s => s.Event).FirstOrDefaultAsync(s => s.Id == id);
			if (session == null)
			{
				return NotFound();
			}
			var sessionVM = new SessionVM
			{
				Id = session.Id,
				Name = session.Name,
				SessionTime = session.SessionTime,
				AvailableTime = session.AvailableTime,
				PublishTime = session.PublishTime,
				Published = session.Published,
				EventId = session.EventId

			};
			ViewData["EventList"] = new SelectList(_context.Events, "Id", "Name", session.EventId);
			return View(sessionVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Edit([Bind(include: "Id,Name,SessionTime,AvailableTime,PublishTime,Published,EventName, EventId")] SessionVM model, int? id)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var session = await _context.Sessions.AsNoTracking().Include(s => s.Event).FirstOrDefaultAsync(s => s.Id == id);
			
			var sessionEdit = new Session
			{
				Id = model.Id,
				Name = model.Name,
				SessionTime = model.SessionTime,
				AvailableTime = model.AvailableTime,
				PublishTime = model.PublishTime,
				Published = model.Published,
				EventId = (int)model.EventId
			};
			_context.Update(sessionEdit);
			await _context.SaveChangesAsync();
			ViewData["EventList"] = new SelectList(_context.Events, "Id", "Name", sessionEdit.EventId);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var session = await _context.Sessions.Include(s => s.Event).FirstOrDefaultAsync(s => s.Id == id);
			if (session == null)
			{
				return NotFound();
			}
			var sessionVM = new SessionVM
			{
				Id = session.Id,
				Name = session.Name,
				SessionTime = session.SessionTime,
				AvailableTime = session.AvailableTime,
				PublishTime = session.PublishTime,
				Published = session.Published,
				EventName = session.Event.Name,
				EventId = session.EventId
			};
			return View(sessionVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var session = await _context.Sessions.FindAsync(id);
			if (session != null)
			{
				_context.Sessions.Remove(session);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(Index));
		}

	}
}
			



