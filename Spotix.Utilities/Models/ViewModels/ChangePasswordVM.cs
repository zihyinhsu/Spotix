using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotix.Utilities.Models.ViewModels
{
	public class ChangePasswordVM
	{
		[Required(ErrorMessage = "{0} 為必填")]
		[EmailAddress]
		public string Email { get; set; }

		[Required(ErrorMessage = "{0} 為必填")]
		[StringLength(40, MinimumLength = 8, ErrorMessage = "{0} 最少應為 {2} 個字元")]
		[DataType(DataType.Password)]
		[Display(Name = "確認密碼")]
		[Compare("ConfirmNewPassword", ErrorMessage = "密碼不一致")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "請確認密碼")]
		[DataType(DataType.Password)]
		[Display(Name = "確認新密碼")]
		public string ConfirmNewPassword { get; set; }
	}
}
