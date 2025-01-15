using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Spotix.Utilities.Models.EFModels;
using Spotix.Utilities.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Spotix.Utilities.Models.DTOs;
using Spotix.Utilities.Models.Repositories;
using Spotix.Utilities.Models.Services;
using Microsoft.AspNetCore.Authorization;

namespace Spotix.MVC.Controllers
{
	public class AuthController : Controller
	{
		private readonly UserManager<User> userManager;

		public AuthController(UserManager<User> userManager)
		{
			this.userManager = userManager;
		}

		public IActionResult Register()
		{
			var model = new RegisterVM
			{
				Roles = new List<string> { "Admin", "User" } // 根據您的需求設置角色
			};
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterVM model)
		{
			if (!ModelState.IsValid) return View(model);
			var user = new User
			{
				UserName = model.UserName,
				Email = model.Email,
				Gender = model.Gender,
				Birthday = model.Birthday,
				Address = model.Address,
				PhoneNumber = model.PhoneNumber,



			};

			var identityResult = await userManager.CreateAsync(user, model.Password);

			if (identityResult.Succeeded)
			{
				// Add roles to user
				if (model.Roles != null && model.Roles.Any())
				{
					identityResult = await userManager.AddToRolesAsync(user, model.Roles);
					if (identityResult.Succeeded)
					{
						TempData["message"] = "註冊成功! 請登入帳號";
						return RedirectToAction("Login");
					}
					else
					{
						var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
						throw new ArgumentException($"{errors}, 請聯繫系統管理員");
						//return View();
					}
				}
			}
			else
			{
				var errors = identityResult.Errors.Select(e => e.Description).ToArray();

				//string.Join: 這個方法會將 errors 中的每個錯誤訊息用逗號和空格（, ）連接起來，形成一個單一的字串。
				//errorMessage: 最後，合併後的錯誤訊息會被存儲在這個變數中
				var errorMessage = string.Join(", ", errors);
				throw new ArgumentException($"註冊失敗: {errorMessage}");
			}

			return View();

		}

		public ActionResult Logout()
		{
			HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login");
		}

		public IActionResult Login(string returnUrl = "/")
		{
			ViewBag.ReturnUrl = returnUrl;
			ViewBag.Message = TempData["message"];
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

        // 顯示修改個人資料的頁面

        //[Authorize(Roles = "Admin, User")]
        [HttpGet]
		public async Task<IActionResult> EditProfile()
		{
            
            var email = User?.FindFirst(ClaimTypes.Name)?.Value;

            var user = await userManager.FindByEmailAsync(email);

			if (user == null)
			{
				TempData["error"] = "找不到使用者!";
				return RedirectToAction("Login");
			}

			var model = new EditProfileVM
			{
				UserName = user.UserName,
				Email = user.Email,
				Gender = user.Gender,
				Birthday = user.Birthday,
				Address = user.Address,
				PhoneNumber = user.PhoneNumber
			};

			return View(model);
		}

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditProfile(EditProfileVM model)
		{
			if (!ModelState.IsValid) return View(model);

			var user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				TempData["error"] = "找不到使用者!";
				return RedirectToAction("Login");
			}

			user.UserName = model.UserName;
			user.Email = model.Email;
			user.Gender = model.Gender;
			user.Birthday = model.Birthday;
			user.Address = model.Address;
			user.PhoneNumber = model.PhoneNumber;

			var result = await userManager.UpdateAsync(user);
			if (result.Succeeded)
			{
				TempData["message"] = "個人資料已成功更新!";
				return RedirectToAction("Index", "Home");
			}

			var errors = string.Join(", ", result.Errors.Select(e => e.Description));
			TempData["error"] = $"更新失敗: {errors}";
			return View(model);
		}
	}
}
