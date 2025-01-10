using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.ViewModels;

namespace Spotix.MVC.Controllers
{
    public class PlaceController : Controller
    {
        private readonly AppDbContext _context;

        public PlaceController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public IActionResult Index(string searchTerm = null)
        {
            // 原始清單
            var places = _context.Places.Select(p => new PlaceVM
            {
                Id = p.Id,
                Name = p.Name,
                DisplayOrder = p.DisplayOrder,
                Enabled = p.Enabled
            }).ToList();

            // 查詢過濾清單
            var filteredPlaces = string.IsNullOrWhiteSpace(searchTerm)
                ? places // 如果沒有輸入查詢條件，顯示完整清單
                : places.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            // 傳遞到 View：兩份清單
            ViewBag.SearchTerm = searchTerm; // 保留查詢條件供前端使用
            return View(filteredPlaces);
        }


        // GET: Place
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Places.ToListAsync());
        //}

        // GET: Place/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Place/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DisplayOrder,Enabled")] Place place)
        {
            if (!ModelState.IsValid)
            {
                // 如果模型驗證失敗，返回表單並顯示錯誤訊息
                return View(place);
            }

            try
            {
                _context.Add(place);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 捕獲例外情況，並顯示一條友好的錯誤訊息
                ModelState.AddModelError(string.Empty, "儲存資料時發生錯誤，請確認輸入內容是否正確。");
                return View(place);
            }
        }

        // GET: Place/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }
            return View(place);
        }

        // POST: Place/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DisplayOrder,Enabled")] Place place)
        {
            if (id != place.Id)
            {
                return NotFound();
            }

            // 確保 ModelState 驗證
            if (!ModelState.IsValid)
            {
                return View(place);
            }

            try
            {
                _context.Update(place);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // 捕捉資料庫更新例外，停留在當前頁面
                Console.WriteLine(ex.Message); // 可記錄日誌（選擇性）
                ModelState.AddModelError(string.Empty, "儲存失敗，請檢查輸入值是否正確。");
                return View(place);
            }
            catch (Exception ex)
            {
                // 捕捉其他例外，停留在當前頁面
                Console.WriteLine(ex.Message); // 可記錄日誌（選擇性）
                ModelState.AddModelError(string.Empty, "發生未知錯誤，請稍後再試。");
                return View(place);
            }
        }

        // GET: Place/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await _context.Places
                .FirstOrDefaultAsync(m => m.Id == id);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
        }

        // POST: Place/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var place = await _context.Places.FindAsync(id);
            if (place != null)
            {
                _context.Places.Remove(place);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaceExists(int id)
        {
            return _context.Places.Any(e => e.Id == id);
        }
    }
}
