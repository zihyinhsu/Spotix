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
		public async Task<IActionResult> Create([Bind(include:"Id,Name,SessionTime,AvailableTime,PublishTime,Published,EventId")] Session session)
		{
			if (ModelState.IsValid)
			{
				_context.Add(session);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["EventList"] = new SelectList(_context.Events, "Id", "Name", session.EventId);
			return View(session);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var session = await _context.Sessions.FindAsync(id);
			if (session == null)
			{
				return NotFound();
			}
			ViewData["EventList"] = new SelectList(_context.Events, "Id", "Name", session.EventId);
			return View(session);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<IActionResult> Edit(int id, [Bind(include: "Id,Name,SessionTime,AvailableTime,PublishTime,Published,EventId")] Session session)
		{
			if (id != session.Id)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(session);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!SessionExists(session.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["EventList"] = new SelectList(_context.Events, "Id", "Name", session.EventId);
			return View(session);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var session = await _context.Sessions
				.FirstOrDefaultAsync(m => m.Id == id);
			if (session == null)
			{
				return NotFound();
			}
			return View(session);
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

		private bool SessionExists(int id)
		{
			return _context.Sessions.Any(e => e.Id == id);
		}
	}
}
