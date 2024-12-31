using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{
	public class RegisterVM
	{

		[Required(ErrorMessage = "{0} 為必填")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "{0} 為必填")]
		[EmailAddress]
		public string Email { get; set; }


		[Required(ErrorMessage = "{0} 為必填")]
		[StringLength(40, MinimumLength = 8, ErrorMessage = "{0} 最少應為 {2} 個字元")]
		[DataType(DataType.Password)]
		[Display(Name = "確認密碼")]
		[Compare("ConfirmPassword", ErrorMessage = "密碼不一致")]
		public string Password { get; set; }

		[Required(ErrorMessage = "請確認密碼")]
		[DataType(DataType.Password)]
		[Display(Name = "確認密碼")]
		public string ConfirmPassword { get; set; }

		public string[] Roles { get; set; }

	}
}
