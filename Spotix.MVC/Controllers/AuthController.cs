using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.Repositories;
using Spotix.Utilities.Interfaces;
using Spotix.Utilities.Models.Services;

namespace Spotix.MVC.Controllers
{
	public class AuthController : Controller
	{
		private readonly UserManager<User> userManager;
		private readonly ImageService imageService;

		public AuthController(UserManager<User> userManager, ImageService imageService)
		{
			this.userManager = userManager;
			this.imageService = imageService;
		}
		public IActionResult Login(string returnUrl = "/")
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginVM model, string returnUrl = "/")
		{

			ViewBag.ReturnUrl = returnUrl;

			var user = await userManager.FindByEmailAsync(model.Email);

			// 判斷帳密
			if (user == null)
			{
				ModelState.AddModelError("Email", "帳號或密碼有誤"); // 決定 error Message show 在哪裡
				return View(model);
			}

			// 帳密正確，儲存認證資訊，先建立List<Claim>，再建立ClaimsIdentity，最後建立ClaimsPrincipal
			var claims = new List<Claim>
			{
				// "Name" 不能用其他值，未來寫 User.Identity.Name 會取不到值
				new Claim(ClaimTypes.Name, user.Email),
				// 可視需要增加其他聲明
			};

			var roles = await userManager.GetRolesAsync(user);

			// Add roles to claims
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			// 第一個參數是此使用者所有的 claims (聲明)，第二個參數是表示目前是哪一個認證方案，值要與 program.cs 裡面的相對應
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

			var principal = new ClaimsPrincipal(identity);

			// 進行登入，第一個參數是表示目前是哪一個認證方案，值要與 program.cs 裡面的相對應
			// 由於這個認證方案是 cookie 認證，所以這支 action 成功執行之後，browser 裡面會有 cookie，名稱就視 program.cs 裡有沒有指定
			HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

			return Redirect(returnUrl);
		}
	}
}
