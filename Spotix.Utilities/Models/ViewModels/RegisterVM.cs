using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{
	public class RegisterVM
	{

		[Display(Name = "姓名")]
		[Required(ErrorMessage = "{0} 為必填")]
		public string UserName { get; set; }

		[Display(Name = "電子郵件")]
		[Required(ErrorMessage = "{0} 為必填")]
		[EmailAddress]
		public string Email { get; set; }


		[Display(Name = "密碼")]
		[Required(ErrorMessage = "{0} 為必填")]
		[StringLength(40, MinimumLength = 8, ErrorMessage = "{0} 最少應為 {2} 個字元")]
		[DataType(DataType.Password)]
		public string Password { get; set; }


		[Display(Name = "確認密碼")]
		[Required(ErrorMessage = "請確認密碼")]
		[Compare("Password", ErrorMessage = "密碼不一致")]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }

		[Display(Name = "LineID")]
		public string? LineId { get; set; } = null;

		[Display(Name = "性別")]
		[Required(ErrorMessage = "請選擇您的性別")]
		public bool Gender { get; set; }

		[Display(Name = "居住地址")]
		public string? Address { get; set; } = null;

		[Display(Name = "大頭貼")]
		public string? AvatarUrl { get; set; } = null;


		[Required(ErrorMessage = "請選擇一個角色")]
		public string SelectedRole { get; set; }

		[Display(Name = "角色")]
		public List<string> Roles { get; set; } = new List<string> { "Admin", "User" };

		[Display(Name = "手機號碼")]
		[Required(ErrorMessage = "{0} 為必填")]
		public string PhoneNumber { get; set; }
	}
}
